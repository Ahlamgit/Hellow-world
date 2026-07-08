using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? RequestedFromIp { get; set; }
    public bool IsUsed => UsedAt != null;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsUsed && !IsExpired && !IsDeleted;

    public User User { get; set; } = null!;
}
