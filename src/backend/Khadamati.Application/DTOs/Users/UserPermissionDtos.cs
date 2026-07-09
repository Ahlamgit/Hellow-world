namespace Khadamati.Application.DTOs.Users;

public class UserPermissionEntryDto
{
    public Guid PermissionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public bool FromRole { get; set; }
    /// <summary>null = inherit from role, true = explicit grant, false = explicit deny.</summary>
    public bool? Override { get; set; }
    public bool Effective { get; set; }
}

public class UserPermissionMatrixDto
{
    public Guid UserId { get; set; }
    public IReadOnlyList<UserPermissionEntryDto> Permissions { get; set; } = Array.Empty<UserPermissionEntryDto>();
}

public class UserPermissionOverrideDto
{
    public Guid PermissionId { get; set; }
    public bool IsGranted { get; set; }
}

public class UpdateUserPermissionsDto
{
    public IReadOnlyList<UserPermissionOverrideDto> Overrides { get; set; } = Array.Empty<UserPermissionOverrideDto>();
}
