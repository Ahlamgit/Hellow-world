using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities.Identity;

public class UserRoleAssignment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string? AssignedBy { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
