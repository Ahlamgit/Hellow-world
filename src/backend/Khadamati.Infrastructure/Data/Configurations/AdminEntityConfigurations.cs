using Khadamati.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khadamati.Infrastructure.Data.Configurations;

public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.ToTable("Advertisements");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.TitleEn).HasMaxLength(200);
        builder.Property(a => a.Placement).HasMaxLength(50);
    }
}

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Code).IsUnique();
        builder.Property(c => c.Code).HasMaxLength(50);
        builder.Property(c => c.DiscountPercentage).HasPrecision(5, 2);
    }
}

public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.ToTable("Complaints");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Subject).HasMaxLength(300);
        builder.Property(c => c.Status).HasMaxLength(50);
        builder.HasOne(c => c.Complainant).WithMany().HasForeignKey(c => c.ComplainantUserId);
    }
}

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTickets");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.TicketNumber).IsUnique();
        builder.Property(t => t.TicketNumber).HasMaxLength(30);
        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
    }
}

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("Regions");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.Code).IsUnique();
    }
}

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.HasKey(c => c.Id);
        builder.HasOne(c => c.Region).WithMany(r => r.Cities).HasForeignKey(c => c.RegionId);
    }
}

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.SettingKey).IsUnique();
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Code).IsUnique();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.RoleId, r.PermissionId }).IsUnique();
        builder.HasOne(r => r.Role).WithMany(role => role.RolePermissions).HasForeignKey(r => r.RoleId);
        builder.HasOne(r => r.Permission).WithMany(p => p.RolePermissions).HasForeignKey(r => r.PermissionId);
    }
}

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.CreatedAt);
    }
}

public class BackupJobConfiguration : IEntityTypeConfiguration<BackupJob>
{
    public void Configure(EntityTypeBuilder<BackupJob> builder)
    {
        builder.ToTable("BackupJobs");
        builder.HasKey(b => b.Id);
    }
}
