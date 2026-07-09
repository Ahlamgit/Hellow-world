namespace Khadamati.Application.DTOs.Admin;

public class AdminRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public int PermissionCount { get; set; }
}

public class AdminPermissionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
}

public class RolePermissionMatrixDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public IReadOnlyList<AdminPermissionDto> AllPermissions { get; set; } = Array.Empty<AdminPermissionDto>();
    public IReadOnlyList<Guid> AssignedPermissionIds { get; set; } = Array.Empty<Guid>();
}

public class UpdateRolePermissionsDto
{
    public IReadOnlyList<Guid> PermissionIds { get; set; } = Array.Empty<Guid>();
}
