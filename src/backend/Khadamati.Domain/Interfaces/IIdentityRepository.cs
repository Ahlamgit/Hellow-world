using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;

namespace Khadamati.Domain.Interfaces;

public interface IIdentityRepository
{
    Task<User?> GetUserByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetUserIdsByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(Guid userId, Guid roleId, bool isPrimary, string? assignedBy, CancellationToken cancellationToken = default);
    Task SetUserRolesAsync(Guid userId, IReadOnlyList<string> roleNames, string primaryRoleName, string? assignedBy, CancellationToken cancellationToken = default);
    Task AddLoginHistoryAsync(LoginHistory entry, CancellationToken cancellationToken = default);
    Task AddSecurityLogAsync(SecurityLog entry, CancellationToken cancellationToken = default);
    Task AddPasswordHistoryAsync(PasswordHistory entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PasswordHistory>> GetRecentPasswordHistoryAsync(Guid userId, int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoginHistory>> GetLoginHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountLoginHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface ISessionRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task RevokeSessionAsync(Guid sessionId, string? ipAddress, CancellationToken cancellationToken = default);
    Task RevokeAllExceptAsync(Guid userId, Guid exceptSessionId, string? ipAddress, CancellationToken cancellationToken = default);
    Task RevokeAllAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default);
    Task UpdateLastActivityAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
