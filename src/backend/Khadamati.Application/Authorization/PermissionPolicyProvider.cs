using Khadamati.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Khadamati.Application.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallback = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName["Permission:".Length..];
            var requireVerified = false;
            if (permission.EndsWith(":Verified", StringComparison.OrdinalIgnoreCase))
            {
                requireVerified = true;
                permission = permission[..^":Verified".Length];
            }

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission, requireVerified))
                .RequireAuthenticatedUser()
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }
}
