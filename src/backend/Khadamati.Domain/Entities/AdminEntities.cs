using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class Advertisement : BaseEntity
{
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public string Placement { get; set; } = "HomePage";
    public Guid? TargetUserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Impressions { get; set; }
    public int Clicks { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Complaint : BaseEntity
{
    public Guid ComplainantUserId { get; set; }
    public Guid? AgainstUserId { get; set; }
    public Guid? BookingId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Normal";
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public User Complainant { get; set; } = null!;
}

public class SupportTicket : BaseEntity
{
    public Guid UserId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Normal";
    public string Category { get; set; } = "General";
    public Guid? AssignedToUserId { get; set; }
    public DateTime? ClosedAt { get; set; }
    public User User { get; set; } = null!;
}

public class Region : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<City> Cities { get; set; } = new List<City>();
}

public class City : BaseEntity
{
    public Guid RegionId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Region Region { get; set; } = null!;
}

public class SystemSetting : BaseEntity
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string? Description { get; set; }
    public bool IsEncrypted { get; set; }
}

public class Permission : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool RequiresEmailVerification { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<Identity.UserPermission> UserPermissions { get; set; } = new List<Identity.UserPermission>();
}

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public Identity.Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

public class ActivityLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}

public class BackupJob : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public long SizeBytes { get; set; }
    public string? FilePath { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? CompletedAt { get; set; }
}
