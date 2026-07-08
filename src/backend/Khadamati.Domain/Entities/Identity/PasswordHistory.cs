using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities.Identity;

public class PasswordHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? ChangedByIp { get; set; }
    public string ChangeReason { get; set; } = "Change";

    public User User { get; set; } = null!;
}
