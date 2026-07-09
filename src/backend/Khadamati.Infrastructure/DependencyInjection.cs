using Khadamati.Application;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services;
using Khadamati.Infrastructure.Services.Identity;
using Khadamati.Infrastructure.Services.Identity.Email;
using Khadamati.Infrastructure.Services.Identity.Sms;
using Khadamati.Infrastructure.Services.Payments;
using Khadamati.Infrastructure.Services.Push;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Khadamati.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ILoginHistoryService, LoginHistoryService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
        services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IAdminExportService, AdminExportService>();
        services.AddScoped<ICraftsmanService, CraftsmanPortalService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<ISupportService, SupportService>();
        services.AddScoped<IDeviceTokenService, DeviceTokenService>();
        services.AddScoped<IChatService, ChatService>();
        RegisterPushProvider(services, configuration);
        RegisterPaymentProvider(services, configuration);
        services.AddScoped<IPaymentWebhookService, PaymentWebhookService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<DatabaseSeeder>();
        services.AddHostedService<TokenCleanupService>();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        RegisterEmailProvider(services, configuration);
        RegisterSmsProvider(services, configuration);

        return services;
    }

    private static void RegisterPaymentProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Payment:Provider"] ?? "Development";
        switch (provider.ToLowerInvariant())
        {
            case "moyasar":
                services.AddHttpClient(nameof(MoyasarPaymentGateway));
                services.AddScoped<IPaymentGateway, MoyasarPaymentGateway>();
                break;
            default:
                services.AddScoped<IPaymentGateway, DevelopmentPaymentGateway>();
                break;
        }
    }

    private static void RegisterPushProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Push:Provider"] ?? "Development";
        switch (provider.ToLowerInvariant())
        {
            case "firebase":
                services.AddHttpClient(nameof(FirebasePushNotificationService));
                services.AddHttpClient(nameof(ApnsPushNotificationSender))
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                    {
                        EnableMultipleHttp2Connections = true,
                    });
                services.AddSingleton<ApnsPushNotificationSender>();
                services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();
                break;
            default:
                services.AddScoped<IPushNotificationService, DevelopmentPushNotificationService>();
                break;
        }
    }

    private static void RegisterEmailProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Email:Provider"] ?? "Development";
        switch (provider.ToLowerInvariant())
        {
            case "smtp":
                services.AddScoped<IEmailProvider, SmtpEmailProvider>();
                break;
            case "sendgrid":
                services.AddHttpClient<SendGridEmailProvider>();
                services.AddScoped<IEmailProvider, SendGridEmailProvider>();
                break;
            default:
                services.AddScoped<IEmailProvider, DevelopmentEmailProvider>();
                break;
        }
    }

    private static void RegisterSmsProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Sms:Provider"] ?? "Development";
        switch (provider.ToLowerInvariant())
        {
            case "twilio":
                services.AddHttpClient<TwilioSmsProvider>();
                services.AddScoped<ISmsProvider, TwilioSmsProvider>();
                break;
            default:
                services.AddScoped<ISmsProvider, DevelopmentSmsProvider>();
                break;
        }
    }
}
