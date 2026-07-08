using Khadamati.Application.Common;
using Khadamati.Application.Interfaces;

namespace Khadamati.Application.Authorization;

public static class AdminAuthorization
{
    public static async Task EnsurePortalAccessAsync(
        IPermissionService permissionService,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        foreach (var permission in AdminPermissionMap.PortalPermissions)
        {
            if (await permissionService.UserHasPermissionAsync(userId, permission, cancellationToken))
                return;
        }

        throw new UnauthorizedException("Admin access denied.");
    }

    public static async Task EnsurePermissionAsync(
        IPermissionService permissionService,
        Guid userId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        if (!await permissionService.UserHasPermissionAsync(userId, permission, cancellationToken))
            throw new UnauthorizedException($"Missing permission: {permission}");
    }
}
