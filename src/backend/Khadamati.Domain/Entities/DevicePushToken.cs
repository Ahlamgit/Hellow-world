using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class DevicePushToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
