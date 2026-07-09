using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        var environment = scope.ServiceProvider.GetService<IHostEnvironment>();

        if (context.Database.IsRelational())
            await context.Database.MigrateAsync(cancellationToken);
        else
            await context.Database.EnsureCreatedAsync(cancellationToken);

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
        await SeedSubscriptionPlansAsync(context, cancellationToken);
        await SeedCraftsmenAndStoresAsync(context, passwordHasher, cancellationToken);
        await SeedVerificationDocumentsAsync(context, cancellationToken);
        await SeedCraftsmanAddressesAsync(context, cancellationToken);
        await IdentitySeeder.SeedAsync(context, _logger, cancellationToken);

        if (environment?.IsEnvironment("Testing") == true)
            await EnsureIntegrationTestAccountsAsync(context, passwordHasher, cancellationToken);
    }

    private static async Task EnsureIntegrationTestAccountsAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken)
    {
        if (!await context.Users.AnyAsync(u => u.Email == "admin@khadamati.com", cancellationToken))
        {
            context.Users.Add(new User
            {
                Email = "admin@khadamati.com",
                Phone = "+966500000001",
                PasswordHash = passwordHasher.Hash("Admin@123456"),
                Role = UserRole.Administrator,
                Status = UserStatus.Active,
                VerificationStatus = VerificationStatus.Verified,
                SubscriptionStatus = SubscriptionStatus.Active,
                Profile = new UserProfile { FirstName = "System", LastName = "Administrator", PreferredLanguage = "en" },
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.ServiceCategories.AnyAsync(cancellationToken))
        {
            var category = new ServiceCategory { NameEn = "Plumbing", NameAr = "سباكة", DisplayOrder = 1, IsActive = true };
            context.ServiceCategories.Add(category);
            context.Services.Add(new Service
            {
                Category = category,
                NameEn = "Leak Repair",
                NameAr = "إصلاح تسرب",
                BasePrice = 150,
                EstimatedDurationMinutes = 60,
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Users.AnyAsync(u => u.Email == "craftsman1@khadamati.com", cancellationToken))
        {
            var service = await context.Services.FirstAsync(cancellationToken);
            var user = new User
            {
                Email = "craftsman1@khadamati.com",
                Phone = "+966500000101",
                PasswordHash = passwordHasher.Hash("Craftsman@123"),
                Role = UserRole.Craftsman,
                Status = UserStatus.Active,
                VerificationStatus = VerificationStatus.Verified,
                SubscriptionStatus = SubscriptionStatus.Active,
                Profile = new UserProfile { FirstName = "Ahmed", LastName = "Al-Otaibi", PreferredLanguage = "ar" },
            };
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            var profile = new CraftsmanProfile
            {
                UserId = user.Id,
                Specialization = "Plumbing",
                YearsOfExperience = 5,
                Rating = 4.5m,
                TotalReviews = 10,
                CompletedJobs = 25,
                IsAvailable = true,
                ServiceRadiusKm = 30,
            };
            context.CraftsmanProfiles.Add(profile);
            await context.SaveChangesAsync(cancellationToken);

            context.CraftsmanServices.Add(new CraftsmanService
            {
                CraftsmanProfileId = profile.Id,
                ServiceId = service.Id,
                CustomPrice = service.BasePrice,
                IsAvailable = true,
            });
            context.Addresses.Add(new Address
            {
                UserId = user.Id,
                Label = "Work",
                Street = "King Fahd Road",
                City = "Riyadh",
                District = "Al Olaya",
                Country = "SA",
                Latitude = 24.7136m,
                Longitude = 46.6753m,
                IsDefault = true,
            });
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task SeedCraftsmenAndStoresAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken)
    {
        if (await context.CraftsmanProfiles.AnyAsync(cancellationToken))
            return;

        var services = await context.Services.OrderBy(s => s.NameEn).ToListAsync(cancellationToken);
        if (services.Count == 0) return;

        var craftsmanRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Domain.Constants.RoleNames.Craftsman, cancellationToken);
        var storeRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Domain.Constants.RoleNames.StoreOwner, cancellationToken);

        var craftsmenData = new[]
        {
            ("craftsman1@khadamati.com", "+966500000101", "Ahmed", "Al-Otaibi", "Plumbing", 0, 1),
            ("craftsman2@khadamati.com", "+966500000102", "Khalid", "Al-Harbi", "Electrical", 1, 2),
            ("craftsman3@khadamati.com", "+966500000103", "Faisal", "Al-Qahtani", "HVAC", 2, 3),
        };

        foreach (var (email, phone, first, last, spec, svcIdx, phoneSuffix) in craftsmenData)
        {
            var user = new User
            {
                Email = email,
                Phone = phone,
                PasswordHash = passwordHasher.Hash("Craftsman@123"),
                Role = UserRole.Craftsman,
                Status = UserStatus.Active,
                VerificationStatus = email == "craftsman1@khadamati.com"
                    ? VerificationStatus.PendingReview
                    : VerificationStatus.Verified,
                SubscriptionStatus = SubscriptionStatus.Active,
                EmailVerifiedAt = DateTime.UtcNow,
                Profile = new UserProfile { FirstName = first, LastName = last, PreferredLanguage = "ar" },
            };
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            if (craftsmanRole is not null)
            {
                context.UserRoles.Add(new Domain.Entities.Identity.UserRoleAssignment
                {
                    UserId = user.Id,
                    RoleId = craftsmanRole.Id,
                    IsPrimary = true,
                    AssignedBy = "Seeder",
                });
                user.PrimaryRoleId = craftsmanRole.Id;
            }

            var profile = new CraftsmanProfile
            {
                UserId = user.Id,
                Specialization = spec,
                YearsOfExperience = 5 + phoneSuffix,
                Rating = 4.5m,
                TotalReviews = 10,
                CompletedJobs = 25,
                IsAvailable = true,
                ServiceRadiusKm = 30,
            };
            context.CraftsmanProfiles.Add(profile);
            await context.SaveChangesAsync(cancellationToken);

            var service = services[Math.Min(svcIdx, services.Count - 1)];
            context.CraftsmanServices.Add(new Domain.Entities.CraftsmanService
            {
                CraftsmanProfileId = profile.Id,
                ServiceId = service.Id,
                CustomPrice = service.BasePrice,
                IsAvailable = true,
            });

            for (var day = DayOfWeek.Sunday; day <= DayOfWeek.Thursday; day++)
            {
                context.CraftsmanWorkingHours.Add(new CraftsmanWorkingHour
                {
                    CraftsmanId = user.Id,
                    DayOfWeek = day,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(18, 0),
                    IsActive = true,
                });
            }

            var (lat, lng) = GetCraftsmanSeedCoordinates(phoneSuffix);
            context.Addresses.Add(new Address
            {
                UserId = user.Id,
                Label = "Work",
                Street = "King Fahd Road",
                City = "Riyadh",
                District = "Al Olaya",
                Country = "SA",
                Latitude = lat,
                Longitude = lng,
                IsDefault = true,
            });
        }

        var storeUser = new User
        {
            Email = "store@khadamati.com",
            Phone = "+966500000201",
            PasswordHash = passwordHasher.Hash("Store@123456"),
            Role = UserRole.Store,
            Status = UserStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            SubscriptionStatus = SubscriptionStatus.Active,
            EmailVerifiedAt = DateTime.UtcNow,
            Profile = new UserProfile { FirstName = "Demo", LastName = "Store", PreferredLanguage = "ar" },
        };
        context.Users.Add(storeUser);
        await context.SaveChangesAsync(cancellationToken);

        if (storeRole is not null)
        {
            context.UserRoles.Add(new Domain.Entities.Identity.UserRoleAssignment
            {
                UserId = storeUser.Id,
                RoleId = storeRole.Id,
                IsPrimary = true,
                AssignedBy = "Seeder",
            });
            storeUser.PrimaryRoleId = storeRole.Id;
        }

        var storeProfile = new StoreProfile
        {
            UserId = storeUser.Id,
            StoreName = "Khadamati Hardware",
            Description = "Demo store for tools and supplies",
            IsOpen = true,
            OpeningTime = new TimeOnly(8, 0),
            ClosingTime = new TimeOnly(22, 0),
        };
        context.StoreProfiles.Add(storeProfile);
        await context.SaveChangesAsync(cancellationToken);

        context.StoreProducts.AddRange(
            new StoreProduct
            {
                StoreProfileId = storeProfile.Id,
                NameEn = "Pipe Wrench",
                NameAr = "مفتاح أنابيب",
                Price = 85,
                StockQuantity = 50,
                Sku = "TOOL-001",
                IsActive = true,
            },
            new StoreProduct
            {
                StoreProfileId = storeProfile.Id,
                NameEn = "Electrical Tape",
                NameAr = "شريط عازل",
                Price = 15,
                StockQuantity = 200,
                Sku = "TOOL-002",
                IsActive = true,
            });

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedVerificationDocumentsAsync(
        ApplicationDbContext context, CancellationToken cancellationToken)
    {
        if (await context.VerificationDocuments.AnyAsync(cancellationToken))
            return;

        var craftsman = await context.Users.FirstOrDefaultAsync(
            u => u.Email == "craftsman1@khadamati.com", cancellationToken);
        if (craftsman is null) return;

        context.VerificationDocuments.Add(new VerificationDocument
        {
            UserId = craftsman.Id,
            DocumentType = "National ID",
            DocumentUrl = "https://storage.khadamati.com/demo/craftsman1-national-id.pdf",
            Status = VerificationStatus.PendingReview,
        });
        await context.SaveChangesAsync(cancellationToken);
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
            // Permissions seeded by IdentitySeeder
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

    private async Task SeedSubscriptionPlansAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        if (await context.SubscriptionPlans.AnyAsync(cancellationToken))
            return;

        var craftsmanBasic = new SubscriptionPlan
        {
            PlanCode = "CRAFTSMAN_BASIC",
            NameEn = "Craftsman Basic",
            NameAr = "حرفي - أساسي",
            DescriptionEn = "Monthly basic plan for craftsmen",
            DescriptionAr = "خطة شهرية أساسية للحرفيين",
            Currency = "SAR",
            TargetRole = UserRole.Craftsman,
            Status = PlanStatus.Active,
            DisplayPriority = 10,
            SearchPriority = 5,
            HomePageVisible = true,
            MaxServices = 5,
            MaxPhotos = 10,
            TrialDays = 7,
            VatRate = 15.00m,
            PaymentMethods = "[\"Card\",\"Mada\"]",
            PlanColor = "#4CAF50",
            CreatedBy = "Seeder",
        };
        craftsmanBasic.BillingOptions.Add(new PlanBillingOption
        {
            Cycle = BillingCycle.Monthly,
            Price = 99.00m,
            DurationDays = 30,
            CreatedBy = "Seeder",
        });
        craftsmanBasic.BillingOptions.Add(new PlanBillingOption
        {
            Cycle = BillingCycle.Quarterly,
            Price = 279.00m,
            DurationDays = 90,
            CreatedBy = "Seeder",
        });
        craftsmanBasic.BillingOptions.Add(new PlanBillingOption
        {
            Cycle = BillingCycle.Annual,
            Price = 999.00m,
            DurationDays = 365,
            CreatedBy = "Seeder",
        });

        var storePro = new SubscriptionPlan
        {
            PlanCode = "STORE_PRO",
            NameEn = "Store Pro",
            NameAr = "متجر - احترافي",
            DescriptionEn = "Premium store plan with analytics and priority support",
            DescriptionAr = "خطة متجر مميزة مع التحليلات والدعم الأولوي",
            Currency = "SAR",
            TargetRole = UserRole.Store,
            Status = PlanStatus.Active,
            DisplayPriority = 20,
            SearchPriority = 15,
            IsFeatured = true,
            HomePageVisible = true,
            BannerVisible = true,
            MaxServices = 50,
            MaxPhotos = 100,
            MaxVideos = 20,
            MaxAdvertisements = 10,
            AdvertisementCredits = 500,
            VerificationBadge = true,
            PremiumBadge = true,
            StatisticsDashboard = true,
            Analytics = true,
            PriorityCustomerSupport = true,
            AutoRenewal = true,
            TrialDays = 14,
            CouponSupport = true,
            VatRate = 15.00m,
            PaymentMethods = "[\"Card\",\"Mada\",\"ApplePay\"]",
            PlanColor = "#FF9800",
            PlanIcon = "store-pro",
            CreatedBy = "Seeder",
        };
        storePro.BillingOptions.Add(new PlanBillingOption
        {
            Cycle = BillingCycle.Monthly,
            Price = 199.00m,
            DurationDays = 30,
            CreatedBy = "Seeder",
        });
        storePro.BillingOptions.Add(new PlanBillingOption
        {
            Cycle = BillingCycle.SemiAnnual,
            Price = 999.00m,
            DurationDays = 180,
            CreatedBy = "Seeder",
        });
        storePro.BillingOptions.Add(new PlanBillingOption
        {
            Cycle = BillingCycle.Annual,
            Price = 1799.00m,
            DurationDays = 365,
            CreatedBy = "Seeder",
        });
        storePro.BillingOptions.Add(new PlanBillingOption
        {
            Cycle = BillingCycle.Lifetime,
            Price = 4999.00m,
            DurationDays = 0,
            CreatedBy = "Seeder",
        });

        context.SubscriptionPlans.AddRange(craftsmanBasic, storePro);
        await context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Seeded subscription plans.");
    }

    private static async Task SeedCraftsmanAddressesAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var craftsmen = await context.Users
            .Where(u => u.Role == UserRole.Craftsman)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        if (craftsmen.Count == 0)
            return;

        var craftsmanIds = craftsmen.Select(c => c.Id).ToList();
        var usersWithDefaultAddress = await context.Addresses
            .Where(a => craftsmanIds.Contains(a.UserId) && a.IsDefault)
            .Select(a => a.UserId)
            .ToListAsync(cancellationToken);

        var index = 0;
        foreach (var craftsman in craftsmen)
        {
            if (usersWithDefaultAddress.Contains(craftsman.Id))
                continue;

            index++;
            var (lat, lng) = GetCraftsmanSeedCoordinates(index);
            context.Addresses.Add(new Address
            {
                UserId = craftsman.Id,
                Label = "Work",
                Street = "King Fahd Road",
                City = "Riyadh",
                District = "Al Olaya",
                Country = "SA",
                Latitude = lat,
                Longitude = lng,
                IsDefault = true,
            });
        }

        if (context.ChangeTracker.HasChanges())
            await context.SaveChangesAsync(cancellationToken);
    }

    private static (decimal Latitude, decimal Longitude) GetCraftsmanSeedCoordinates(int index) => index switch
    {
        1 => (24.7136m, 46.6753m),
        2 => (24.7200m, 46.6850m),
        3 => (24.7080m, 46.6700m),
        _ => (24.7136m + (index * 0.002m), 46.6753m + (index * 0.002m)),
    };
}
