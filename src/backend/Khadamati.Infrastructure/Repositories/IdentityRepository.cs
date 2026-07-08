using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class IdentityRepository : IIdentityRepository
{
    private readonly ApplicationDbContext _context;

    public IdentityRepository(ApplicationDbContext context) => _context = context;

    public Task<User?> GetUserByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Users
            .Include(u => u.Profile)
            .Include(u => u.PrimaryRole)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public Task<User?> GetUserByIdWithRolesAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.Users
            .Include(u => u.Profile)
            .Include(u => u.PrimaryRole)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default) =>
        _context.Set<Role>().FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public async Task<IReadOnlyList<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roleIds = await _context.Set<UserRoleAssignment>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        var rolePermissions = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission.Code)
            .ToListAsync(cancellationToken);

        var directGrants = await _context.Set<UserPermission>()
            .Where(up => up.UserId == userId && up.IsGranted && (up.ExpiresAt == null || up.ExpiresAt > DateTime.UtcNow))
            .Include(up => up.Permission)
            .Select(up => up.Permission.Code)
            .ToListAsync(cancellationToken);

        var directDenies = await _context.Set<UserPermission>()
            .Where(up => up.UserId == userId && !up.IsGranted && (up.ExpiresAt == null || up.ExpiresAt > DateTime.UtcNow))
            .Include(up => up.Permission)
            .Select(up => up.Permission.Code)
            .ToListAsync(cancellationToken);

        return rolePermissions.Concat(directGrants).Distinct(StringComparer.OrdinalIgnoreCase)
            .Except(directDenies, StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p)
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRoleAssignment>()
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, bool isPrimary, string? assignedBy, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Set<UserRoleAssignment>()
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

        if (!exists)
        {
            await _context.Set<UserRoleAssignment>().AddAsync(new UserRoleAssignment
            {
                UserId = userId,
                RoleId = roleId,
                IsPrimary = isPrimary,
                AssignedBy = assignedBy,
            }, cancellationToken);
        }

        if (isPrimary)
        {
            var user = await _context.Users.FindAsync([userId], cancellationToken);
            if (user != null) user.PrimaryRoleId = roleId;
        }
    }

    public async Task SetUserRolesAsync(Guid userId, IReadOnlyList<string> roleNames, string primaryRoleName, string? assignedBy, CancellationToken cancellationToken = default)
    {
        var normalized = roleNames.Select(r => r.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var roles = await _context.Roles.Where(r => normalized.Contains(r.Name)).ToListAsync(cancellationToken);
        if (roles.Count != normalized.Count)
            throw new InvalidOperationException("One or more roles were not found.");

        var primary = roles.FirstOrDefault(r => r.Name.Equals(primaryRoleName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Primary role not found.");

        var existing = await _context.Set<UserRoleAssignment>()
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var assignment in existing)
        {
            if (!roles.Any(r => r.Id == assignment.RoleId))
                _context.Set<UserRoleAssignment>().Remove(assignment);
        }

        foreach (var role in roles)
        {
            var assignment = existing.FirstOrDefault(a => a.RoleId == role.Id);
            var isPrimary = role.Id == primary.Id;
            if (assignment == null)
            {
                await _context.Set<UserRoleAssignment>().AddAsync(new UserRoleAssignment
                {
                    UserId = userId,
                    RoleId = role.Id,
                    IsPrimary = isPrimary,
                    AssignedBy = assignedBy,
                }, cancellationToken);
            }
            else
            {
                assignment.IsPrimary = isPrimary;
                assignment.AssignedBy = assignedBy;
            }
        }

        var user = await _context.Users.FindAsync([userId], cancellationToken);
        if (user != null)
        {
            user.PrimaryRoleId = primary.Id;
            user.Role = Khadamati.Domain.Constants.RoleNames.MapToLegacyEnum(primary.Name);
        }
    }

    public async Task AddLoginHistoryAsync(LoginHistory entry, CancellationToken cancellationToken = default)
    {
        await _context.Set<LoginHistory>().AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddSecurityLogAsync(SecurityLog entry, CancellationToken cancellationToken = default)
    {
        await _context.Set<SecurityLog>().AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPasswordHistoryAsync(PasswordHistory entry, CancellationToken cancellationToken = default)
    {
        await _context.Set<PasswordHistory>().AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PasswordHistory>> GetRecentPasswordHistoryAsync(Guid userId, int count, CancellationToken cancellationToken = default) =>
        await _context.Set<PasswordHistory>()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.ChangedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LoginHistory>> GetLoginHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default) =>
        await _context.Set<LoginHistory>()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.LoginAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public Task<int> CountLoginHistoryAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.Set<LoginHistory>().CountAsync(l => l.UserId == userId, cancellationToken);
}
