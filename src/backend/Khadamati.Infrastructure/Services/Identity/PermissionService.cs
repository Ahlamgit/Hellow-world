using Khadamati.Application.Interfaces;
using Khadamati.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Khadamati.Infrastructure.Services.Identity;

public class PermissionService : IPermissionService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public PermissionService(IIdentityRepository identityRepository, IMemoryCache cache)
    {
        _identityRepository = identityRepository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"permissions:{userId}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<string>? cached) && cached != null)
            return cached;

        var permissions = await _identityRepository.GetUserPermissionCodesAsync(userId, cancellationToken);
        _cache.Set(cacheKey, permissions, CacheDuration);
        return permissions;
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(permissionCode, StringComparer.OrdinalIgnoreCase);
    }

    public void InvalidateCache(Guid userId) => _cache.Remove($"permissions:{userId}");
}
