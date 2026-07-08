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
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.CraftsmanId);
        builder.HasIndex(r => r.Status);
        builder.Property(r => r.EstimatedPrice).HasPrecision(18, 2);
        builder.Property(r => r.FinalPrice).HasPrecision(18, 2);

        builder.HasOne(r => r.Customer)
            .WithMany(u => u.CustomerRequests)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Craftsman)
            .WithMany(u => u.AssignedRequests)
            .HasForeignKey(r => r.CraftsmanId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Service)
            .WithMany(s => s.Requests)
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Address)
            .WithMany()
            .HasForeignKey(r => r.AddressId)
            .OnDelete(DeleteBehavior.SetNull);
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
