using Microsoft.AspNetCore.Authorization;

namespace Khadamati.Application.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public bool RequireEmailVerification { get; }

    public PermissionRequirement(string permission, bool requireEmailVerification = false)
    {
        Permission = permission;
        RequireEmailVerification = requireEmailVerification;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly Interfaces.ICurrentUserService _currentUser;
    private readonly Interfaces.IPermissionService _permissionService;

    public PermissionAuthorizationHandler(
        Interfaces.ICurrentUserService currentUser,
        Interfaces.IPermissionService permissionService)
    {
        _currentUser = currentUser;
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (_currentUser.UserId is not Guid userId)
            return;

        if (requirement.RequireEmailVerification && !_currentUser.EmailVerified)
            return;

        if (await _permissionService.UserHasPermissionAsync(userId, requirement.Permission))
            context.Succeed(requirement);
    }
}

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}
