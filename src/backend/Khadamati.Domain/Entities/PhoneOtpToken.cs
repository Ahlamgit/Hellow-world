using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class PhoneOtpToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int AttemptCount { get; set; }
    public string? RequestedFromIp { get; set; }
    public bool IsUsed => VerifiedAt != null;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsUsed && !IsExpired && !IsDeleted;

    public User User { get; set; } = null!;
}
