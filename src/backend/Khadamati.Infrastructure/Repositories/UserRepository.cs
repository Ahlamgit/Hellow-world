using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();
        if (includeDetails)
            query = query
                .Include(u => u.Profile)
                .Include(u => u.PrimaryRole)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
        return await query.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var normalized = email.ToLowerInvariant();
        return excludeUserId.HasValue
            ? _context.Users.AnyAsync(u => u.Email == normalized && u.Id != excludeUserId.Value, cancellationToken)
            : _context.Users.AnyAsync(u => u.Email == normalized, cancellationToken);
    }

    public Task<bool> PhoneExistsAsync(string phone, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        return excludeUserId.HasValue
            ? _context.Users.AnyAsync(u => u.Phone == phone && u.Id != excludeUserId.Value, cancellationToken)
            : _context.Users.AnyAsync(u => u.Phone == phone, cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
        string? search,
        UserStatus? status,
        string? roleName,
        DateTime? fromDate,
        DateTime? toDate,
        string sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .Include(u => u.Profile)
            .Include(u => u.PrimaryRole)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.Phone.Contains(term) ||
                (u.Profile != null && (u.Profile.FirstName.ToLower().Contains(term) || u.Profile.LastName.ToLower().Contains(term))));
        }

        if (status.HasValue)
            query = query.Where(u => u.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(roleName))
            query = query.Where(u =>
                (u.PrimaryRole != null && u.PrimaryRole.Name == roleName) ||
                u.UserRoles.Any(ur => ur.Role.Name == roleName));

        if (fromDate.HasValue)
            query = query.Where(u => u.CreatedAt >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(u => u.CreatedAt <= toDate.Value);

        query = ApplySort(query, sortBy, sortDescending);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user) => _context.Users.Update(user);

    public void SoftDelete(User user, string? deletedBy = null)
    {
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = deletedBy;
        _context.Users.Update(user);
    }

    private static IQueryable<User> ApplySort(IQueryable<User> query, string sortBy, bool desc) =>
        (sortBy?.ToLowerInvariant()) switch
        {
            "email" => desc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "firstname" or "firstName" => desc
                ? query.OrderByDescending(u => u.Profile!.FirstName)
                : query.OrderBy(u => u.Profile!.FirstName),
            "status" => desc ? query.OrderByDescending(u => u.Status) : query.OrderBy(u => u.Status),
            "role" => desc
                ? query.OrderByDescending(u => u.PrimaryRole!.Name)
                : query.OrderBy(u => u.PrimaryRole!.Name),
            _ => desc ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
        };
}
