using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities.Identity;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public ICollection<UserRoleAssignment> UserRoles { get; set; } = new List<UserRoleAssignment>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
