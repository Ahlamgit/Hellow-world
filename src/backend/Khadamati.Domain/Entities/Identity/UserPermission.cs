using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities.Identity;

public class UserPermission : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;
    public string? GrantedBy { get; set; }
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
