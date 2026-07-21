using System.Text;
using AspNetCoreRateLimit;
using Khadamati.API.Configuration;
using Khadamati.API.Middleware;
using Khadamati.Application;
using Khadamati.Infrastructure;
using Khadamati.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

ProductionStartupValidator.ValidateJwtSecret(builder.Configuration, builder.Environment);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/khadamati-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Legacy role policies for backward compatibility with existing modules
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("CraftsmanOnly", policy => policy.RequireRole("Craftsman"));
    options.AddPolicy("StoreOnly", policy => policy.RequireRole("StoreOwner", "Store"));
    options.AddPolicy("CraftsmanOrStore", policy => policy.RequireRole("Craftsman", "StoreOwner", "Store"));
    options.AddPolicy("VerifiedUser", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KHADAMATI API",
        Version = "v1",
        Description = "Maintenance and Home Services Marketplace REST API. Authentication uses JWT Bearer tokens with SQL Server + BCrypt."
    });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("KhadamatiCors", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(static origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
                return uri.Host is "localhost" or "127.0.0.1";
            });
        }
        else
        {
            policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:3000"]);
        }

        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KHADAMATI API v1"));
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseCors("KhadamatiCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();

    var readiness = scope.ServiceProvider.GetRequiredService<Khadamati.Application.Interfaces.IIntegrationReadinessService>();
    ProductionStartupValidator.EnsureIntegrationsReadyIfRequired(
        app.Configuration,
        app.Environment,
        readiness);

    var report = readiness.GetReport();
    if (report.ProductionReady)
    {
        Log.Information("All production integrations are configured and ready.");
    }
    else
    {
        foreach (var provider in report.Providers.Where(p => !p.IsProductionReady))
        {
            if (provider.Status == "Development")
            {
                Log.Warning(
                    "{Category} is using development provider '{SelectedProvider}'. Configure production credentials before launch.",
                    provider.Category,
                    provider.SelectedProvider);
            }
            else
            {
                Log.Warning(
                    "{Category} provider '{SelectedProvider}' is misconfigured. Missing: {Missing}",
                    provider.Category,
                    provider.SelectedProvider,
                    string.Join(", ", provider.MissingSettings));
            }
        }
    }
}

Log.Information("KHADAMATI API starting...");
app.Run();

public partial class Program;
