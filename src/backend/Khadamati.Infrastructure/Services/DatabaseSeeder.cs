using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services;

public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync(cancellationToken);

        if (!await context.Users.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Seeding initial data...");

            var admin = new User
            {
                Email = "admin@khadamati.com",
                Phone = "+966500000001",
                PasswordHash = passwordHasher.Hash("Admin@123456"),
                Role = UserRole.Administrator,
                Status = UserStatus.Active,
                VerificationStatus = VerificationStatus.Verified,
                SubscriptionStatus = SubscriptionStatus.Active,
                Profile = new UserProfile
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    PreferredLanguage = "en"
                }
            };
            context.Users.Add(admin);

            var categories = new List<ServiceCategory>
            {
                new() { NameAr = "سباكة", NameEn = "Plumbing", DisplayOrder = 1, IsActive = true },
                new() { NameAr = "كهرباء", NameEn = "Electrical", DisplayOrder = 2, IsActive = true },
                new() { NameAr = "تكييف", NameEn = "HVAC", DisplayOrder = 3, IsActive = true },
                new() { NameAr = "دهان", NameEn = "Painting", DisplayOrder = 4, IsActive = true },
                new() { NameAr = "تنظيف", NameEn = "Cleaning", DisplayOrder = 5, IsActive = true }
            };
            context.ServiceCategories.AddRange(categories);
            await context.SaveChangesAsync(cancellationToken);

            var services = new List<Service>
            {
                new() { CategoryId = categories[0].Id, NameAr = "إصلاح تسرب", NameEn = "Leak Repair", BasePrice = 150, EstimatedDurationMinutes = 60 },
                new() { CategoryId = categories[0].Id, NameAr = "تركيب صنبور", NameEn = "Faucet Installation", BasePrice = 100, EstimatedDurationMinutes = 45 },
                new() { CategoryId = categories[1].Id, NameAr = "إصلاح كهربائي", NameEn = "Electrical Repair", BasePrice = 200, EstimatedDurationMinutes = 90 },
                new() { CategoryId = categories[2].Id, NameAr = "صيانة مكيف", NameEn = "AC Maintenance", BasePrice = 250, EstimatedDurationMinutes = 120 },
                new() { CategoryId = categories[4].Id, NameAr = "تنظيف منزل", NameEn = "Home Cleaning", BasePrice = 300, EstimatedDurationMinutes = 180 }
            };
            context.Services.AddRange(services);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Seed data created successfully.");
        }
    }
}
