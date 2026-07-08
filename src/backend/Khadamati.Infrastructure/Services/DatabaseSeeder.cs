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

        await SeedAdminDataAsync(context, cancellationToken);
    }

    private async Task SeedAdminDataAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        if (!await context.Regions.AnyAsync(cancellationToken))
        {
            var riyadh = new Region { NameEn = "Riyadh Region", NameAr = "منطقة الرياض", Code = "RYD", IsActive = true };
            var makkah = new Region { NameEn = "Makkah Region", NameAr = "منطقة مكة", Code = "MKK", IsActive = true };
            context.Regions.AddRange(riyadh, makkah);
            await context.SaveChangesAsync(cancellationToken);

            context.Cities.AddRange(
                new City { RegionId = riyadh.Id, NameEn = "Riyadh", NameAr = "الرياض", Code = "RYD-01", IsActive = true },
                new City { RegionId = riyadh.Id, NameEn = "Diriyah", NameAr = "الدرعية", Code = "RYD-02", IsActive = true },
                new City { RegionId = makkah.Id, NameEn = "Jeddah", NameAr = "جدة", Code = "MKK-01", IsActive = true },
                new City { RegionId = makkah.Id, NameEn = "Makkah", NameAr = "مكة", Code = "MKK-02", IsActive = true });
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Permissions.AnyAsync(cancellationToken))
        {
            var modules = new[] { "users", "bookings", "subscriptions", "payments", "settings", "reports" };
            foreach (var mod in modules)
            {
                context.Permissions.Add(new Permission
                {
                    Code = $"{mod}.manage",
                    NameEn = $"Manage {mod}",
                    NameAr = $"إدارة {mod}",
                    Module = mod,
                });
            }
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.SystemSettings.AnyAsync(cancellationToken))
        {
            context.SystemSettings.AddRange(
                new SystemSetting { SettingKey = "platform.name", SettingValue = "Khadamati", Category = "General" },
                new SystemSetting { SettingKey = "booking.payment_timeout_minutes", SettingValue = "30", Category = "Booking" },
                new SystemSetting { SettingKey = "notification.email_enabled", SettingValue = "true", Category = "Notifications" });
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Advertisements.AnyAsync(cancellationToken))
        {
            context.Advertisements.Add(new Advertisement
            {
                TitleEn = "Summer Promotion",
                TitleAr = "عرض الصيف",
                Placement = "HomePage",
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow.AddDays(30),
                IsActive = true,
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Coupons.AnyAsync(cancellationToken))
        {
            context.Coupons.Add(new Coupon
            {
                Code = "WELCOME10",
                DescriptionEn = "10% off first booking",
                DescriptionAr = "خصم 10% على أول حجز",
                DiscountPercentage = 10,
                MaxUses = 1000,
                ValidFrom = DateTime.UtcNow.AddDays(-30),
                ValidTo = DateTime.UtcNow.AddDays(90),
                IsActive = true,
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Administrator, cancellationToken);
        if (adminUser != null && !await context.Complaints.AnyAsync(cancellationToken))
        {
            context.Complaints.Add(new Complaint
            {
                ComplainantUserId = adminUser.Id,
                Subject = "Sample complaint for testing",
                Description = "This is a seeded complaint record.",
                Status = "Open",
                Priority = "Normal",
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        if (adminUser != null && !await context.SupportTickets.AnyAsync(cancellationToken))
        {
            context.SupportTickets.Add(new SupportTicket
            {
                UserId = adminUser.Id,
                TicketNumber = "TKT-00001",
                Subject = "Sample support ticket",
                Description = "Seeded support ticket for admin dashboard.",
                Status = "Open",
                Priority = "Normal",
                Category = "General",
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.BackupJobs.AnyAsync(cancellationToken))
        {
            context.BackupJobs.Add(new BackupJob
            {
                Name = "initial-backup",
                Status = "Completed",
                SizeBytes = 512000,
                CompletedAt = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
