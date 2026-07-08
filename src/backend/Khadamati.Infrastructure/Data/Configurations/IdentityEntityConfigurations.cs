using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khadamati.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.Name).IsUnique();
        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();
        builder.Property(r => r.NameAr).HasMaxLength(100).IsRequired();
    }
}

public class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(ur => ur.Id);
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
        builder.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
        builder.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
    }
}

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermissions");
        builder.HasKey(up => up.Id);
        builder.HasIndex(up => new { up.UserId, up.PermissionId }).IsUnique();
        builder.HasOne(up => up.User).WithMany(u => u.DirectPermissions).HasForeignKey(up => up.UserId);
        builder.HasOne(up => up.Permission).WithMany(p => p.UserPermissions).HasForeignKey(up => up.PermissionId);
    }
}

public class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.ToTable("LoginHistory");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();
        builder.HasIndex(l => new { l.UserId, l.LoginAt });
        builder.HasIndex(l => l.Email);
        builder.HasOne(l => l.User).WithMany(u => u.LoginHistories).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class SecurityLogConfiguration : IEntityTypeConfiguration<SecurityLog>
{
    public void Configure(EntityTypeBuilder<SecurityLog> builder)
    {
        builder.ToTable("SecurityLogs");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.HasIndex(s => s.CreatedAt);
        builder.HasIndex(s => s.EventType);
        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable("PasswordHistory");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.UserId, p.ChangedAt });
        builder.HasOne(p => p.User).WithMany(u => u.PasswordHistories).HasForeignKey(p => p.UserId);
    }
}
