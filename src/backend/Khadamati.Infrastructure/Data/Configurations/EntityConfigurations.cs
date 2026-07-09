using Khadamati.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khadamati.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Phone).IsUnique();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Phone).HasMaxLength(20).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(u => u.Role).HasConversion<int>();
        builder.Property(u => u.Status).HasConversion<int>();
        builder.Property(u => u.VerificationStatus).HasConversion<int>();
        builder.Property(u => u.SubscriptionStatus).HasConversion<int>();
        builder.Property(u => u.EmailVerifiedAt);
        builder.Property(u => u.PhoneVerifiedAt);
        builder.Property(u => u.PasswordChangedAt);
        builder.HasOne(u => u.PrimaryRole).WithMany().HasForeignKey(u => u.PrimaryRoleId).OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Profile).WithOne(p => p.User).HasForeignKey<UserProfile>(p => p.UserId);
        builder.HasOne(u => u.CraftsmanProfile).WithOne(c => c.User).HasForeignKey<CraftsmanProfile>(c => c.UserId);
        builder.HasOne(u => u.StoreProfile).WithOne(s => s.User).HasForeignKey<StoreProfile>(s => s.UserId);
    }
}

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.UserId).IsUnique();
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.PreferredLanguage).HasMaxLength(5).HasDefaultValue("ar");
        builder.Property(p => p.Timezone).HasMaxLength(50).HasDefaultValue("Asia/Riyadh");
        builder.Property(p => p.Gender).HasMaxLength(20);
        builder.Property(p => p.Nationality).HasMaxLength(100);
        builder.Property(p => p.Country).HasMaxLength(100);
        builder.Property(p => p.City).HasMaxLength(100);
        builder.Property(p => p.Region).HasMaxLength(100);
        builder.Property(p => p.AddressLine).HasMaxLength(500);
        builder.Property(p => p.Latitude).HasPrecision(10, 7);
        builder.Property(p => p.Longitude).HasPrecision(10, 7);
        builder.Ignore(p => p.FullName);
    }
}

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.UserId);
        builder.Property(a => a.Latitude).HasPrecision(10, 7);
        builder.Property(a => a.Longitude).HasPrecision(10, 7);
        builder.HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.Token).IsUnique();
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.DeviceId);
        builder.Property(r => r.RememberMe).HasDefaultValue(false);
        builder.Property(r => r.DeviceName).HasMaxLength(200);
        builder.Property(r => r.Platform).HasMaxLength(50);
        builder.Property(r => r.Browser).HasMaxLength(100);
        builder.Property(r => r.UserAgent).HasMaxLength(500);
        builder.Property(r => r.DeviceId).HasMaxLength(100);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.TableName);
    }
}

public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.ToTable("ServiceCategories");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.ParentCategoryId);
        builder.HasOne(c => c.ParentCategory).WithMany(c => c.SubCategories).HasForeignKey(c => c.ParentCategoryId);
    }
}

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.CategoryId);
        builder.Property(s => s.BasePrice).HasPrecision(18, 2);
    }
}

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.ToTable("ServiceRequests");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.BookingReference).IsUnique();
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.CraftsmanId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.ScheduledAt);
        builder.Property(r => r.BookingReference).HasMaxLength(30).IsRequired();
        builder.Property(r => r.Status).HasConversion<int>();
        builder.Property(r => r.EstimatedPrice).HasPrecision(18, 2);
        builder.Property(r => r.FinalPrice).HasPrecision(18, 2);
        builder.Property(r => r.RejectionReason).HasMaxLength(500);
        builder.Property(r => r.CancellationReason).HasMaxLength(500);

        builder.HasOne(r => r.Customer)
            .WithMany(u => u.CustomerRequests)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Craftsman)
            .WithMany(u => u.AssignedRequests)
            .HasForeignKey(r => r.CraftsmanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Service)
            .WithMany(s => s.Requests)
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Address)
            .WithMany()
            .HasForeignKey(r => r.AddressId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.RescheduledFrom)
            .WithMany()
            .HasForeignKey(r => r.RescheduledFromId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Payment)
            .WithOne(p => p.ServiceRequest)
            .HasForeignKey<BookingPayment>(p => p.ServiceRequestId);

        builder.HasOne(r => r.SlotReservation)
            .WithOne(s => s.ServiceRequest)
            .HasForeignKey<BookingSlotReservation>(s => s.ServiceRequestId);
    }
}

public class CraftsmanProfileConfiguration : IEntityTypeConfiguration<CraftsmanProfile>
{
    public void Configure(EntityTypeBuilder<CraftsmanProfile> builder)
    {
        builder.ToTable("CraftsmanProfiles");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.UserId).IsUnique();
        builder.Property(c => c.Rating).HasPrecision(3, 2);
        builder.Property(c => c.ServiceRadiusKm).HasPrecision(10, 2);
    }
}

public class StoreProfileConfiguration : IEntityTypeConfiguration<StoreProfile>
{
    public void Configure(EntityTypeBuilder<StoreProfile> builder)
    {
        builder.ToTable("StoreProfiles");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.UserId).IsUnique();
        builder.Property(s => s.Rating).HasPrecision(3, 2);
    }
}

public class StoreProductConfiguration : IEntityTypeConfiguration<StoreProduct>
{
    public void Configure(EntityTypeBuilder<StoreProduct> builder)
    {
        builder.ToTable("StoreProducts");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.StoreProfileId);
        builder.Property(p => p.Price).HasPrecision(18, 2);
    }
}

public class CraftsmanServiceConfiguration : IEntityTypeConfiguration<CraftsmanService>
{
    public void Configure(EntityTypeBuilder<CraftsmanService> builder)
    {
        builder.ToTable("CraftsmanServices");
        builder.HasKey(cs => cs.Id);
        builder.HasIndex(cs => new { cs.CraftsmanProfileId, cs.ServiceId }).IsUnique();
        builder.Property(cs => cs.CustomPrice).HasPrecision(18, 2);
    }
}

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.ToTable("EmailVerificationTokens");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.ExpiresAt);
        builder.Property(t => t.TokenHash).HasMaxLength(512).IsRequired();
        builder.HasOne(t => t.User).WithMany(u => u.EmailVerificationTokens).HasForeignKey(t => t.UserId);
    }
}

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.ExpiresAt);
        builder.Property(t => t.TokenHash).HasMaxLength(512).IsRequired();
        builder.HasOne(t => t.User).WithMany(u => u.PasswordResetTokens).HasForeignKey(t => t.UserId);
    }
}

public class PhoneOtpTokenConfiguration : IEntityTypeConfiguration<PhoneOtpToken>
{
    public void Configure(EntityTypeBuilder<PhoneOtpToken> builder)
    {
        builder.ToTable("PhoneOtpTokens");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => new { t.UserId, t.Phone });
        builder.HasIndex(t => t.ExpiresAt);
        builder.Property(t => t.OtpHash).HasMaxLength(512).IsRequired();
        builder.Property(t => t.Phone).HasMaxLength(20).IsRequired();
        builder.HasOne(t => t.User).WithMany(u => u.PhoneOtpTokens).HasForeignKey(t => t.UserId);
    }
}

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.PlanCode).IsUnique();
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.TargetRole);
        builder.HasIndex(p => p.DisplayPriority);
        builder.Property(p => p.PlanCode).HasMaxLength(50).IsRequired();
        builder.Property(p => p.NameEn).HasMaxLength(150).IsRequired();
        builder.Property(p => p.NameAr).HasMaxLength(150).IsRequired();
        builder.Property(p => p.DescriptionEn).HasMaxLength(2000);
        builder.Property(p => p.DescriptionAr).HasMaxLength(2000);
        builder.Property(p => p.Currency).HasMaxLength(3).HasDefaultValue("SAR");
        builder.Property(p => p.TargetRole).HasConversion<int>();
        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.DiscountPercentage).HasPrecision(5, 2);
        builder.Property(p => p.TaxRate).HasPrecision(5, 2);
        builder.Property(p => p.VatRate).HasPrecision(5, 2);
        builder.Property(p => p.PaymentMethods).HasMaxLength(1000);
        builder.Property(p => p.PlanColor).HasMaxLength(20);
        builder.Property(p => p.PlanIcon).HasMaxLength(500);
        builder.HasMany(p => p.BillingOptions).WithOne(b => b.Plan).HasForeignKey(b => b.PlanId);
        builder.HasMany(p => p.UserSubscriptions).WithOne(s => s.Plan).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PlanBillingOptionConfiguration : IEntityTypeConfiguration<PlanBillingOption>
{
    public void Configure(EntityTypeBuilder<PlanBillingOption> builder)
    {
        builder.ToTable("PlanBillingOptions");
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => new { b.PlanId, b.Cycle }).IsUnique();
        builder.Property(b => b.Cycle).HasConversion<int>();
        builder.Property(b => b.Price).HasPrecision(18, 2);
    }
}

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.PlanId);
        builder.Property(s => s.Status).HasConversion<int>();
        builder.Property(s => s.AmountPaid).HasPrecision(18, 2);
        builder.Property(s => s.Currency).HasMaxLength(3);
        builder.Property(s => s.CouponCode).HasMaxLength(50);
        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Plan).WithMany(p => p.UserSubscriptions).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class BookingSlotReservationConfiguration : IEntityTypeConfiguration<BookingSlotReservation>
{
    public void Configure(EntityTypeBuilder<BookingSlotReservation> builder)
    {
        builder.ToTable("BookingSlotReservations");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.CraftsmanId, r.SlotStart }).IsUnique()
            .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0");
        builder.HasIndex(r => r.ServiceRequestId).IsUnique();
    }
}

public class CraftsmanWorkingHourConfiguration : IEntityTypeConfiguration<CraftsmanWorkingHour>
{
    public void Configure(EntityTypeBuilder<CraftsmanWorkingHour> builder)
    {
        builder.ToTable("CraftsmanWorkingHours");
        builder.HasKey(w => w.Id);
        builder.HasIndex(w => new { w.CraftsmanId, w.DayOfWeek });
        builder.HasOne(w => w.Craftsman).WithMany().HasForeignKey(w => w.CraftsmanId);
    }
}

public class BookingPaymentConfiguration : IEntityTypeConfiguration<BookingPayment>
{
    public void Configure(EntityTypeBuilder<BookingPayment> builder)
    {
        builder.ToTable("BookingPayments");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.ServiceRequestId).IsUnique();
        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.Currency).HasMaxLength(3);
        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.PaymentMethod).HasMaxLength(50);
        builder.Property(p => p.TransactionReference).HasMaxLength(200);
        builder.HasOne(p => p.Payer).WithMany().HasForeignKey(p => p.PayerUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Payee).WithMany().HasForeignKey(p => p.PayeeUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        builder.HasIndex(n => new { n.UserId, n.IsRead });
        builder.Property(n => n.TitleEn).HasMaxLength(200).IsRequired();
        builder.Property(n => n.TitleAr).HasMaxLength(200).IsRequired();
        builder.Property(n => n.NotificationType).HasMaxLength(50);
        builder.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId);
    }
}

public class ServiceRequestStatusHistoryConfiguration : IEntityTypeConfiguration<ServiceRequestStatusHistory>
{
    public void Configure(EntityTypeBuilder<ServiceRequestStatusHistory> builder)
    {
        builder.ToTable("ServiceRequestStatusHistories");
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.ServiceRequestId);
        builder.Property(h => h.OldStatus).HasConversion<int>();
        builder.Property(h => h.NewStatus).HasConversion<int>();
        builder.HasOne(h => h.ServiceRequest).WithMany(r => r.StatusHistory).HasForeignKey(h => h.ServiceRequestId);
        builder.HasOne(h => h.ChangedByUser).WithMany().HasForeignKey(h => h.ChangedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
