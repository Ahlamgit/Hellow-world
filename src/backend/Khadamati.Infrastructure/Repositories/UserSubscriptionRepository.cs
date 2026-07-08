using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class UserSubscriptionRepository : IUserSubscriptionRepository
{
    private readonly ApplicationDbContext _context;

    public UserSubscriptionRepository(ApplicationDbContext context) => _context = context;

    private IQueryable<UserSubscription> BaseQuery(bool includeDetails) =>
        includeDetails
            ? _context.UserSubscriptions
                .Include(s => s.User).ThenInclude(u => u.Profile)
                .Include(s => s.Plan)
                .Include(s => s.BillingOption)
            : _context.UserSubscriptions.AsQueryable();

    public Task<UserSubscription?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default) =>
        BaseQuery(includeDetails).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<UserSubscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        BaseQuery(includeDetails: true)
            .Where(s => s.UserId == userId && (s.Status == SubscriptionStatus.Trial || s.Status == SubscriptionStatus.Active))
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<(IReadOnlyList<UserSubscription> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = BaseQuery(includeDetails: true).Where(s => s.UserId == userId);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(s => s.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<UserSubscription> Items, int TotalCount)> SearchAsync(
        string? search,
        SubscriptionStatus? status,
        Guid? userId,
        Guid? planId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BaseQuery(includeDetails: true);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.User.Email.ToLower().Contains(term) ||
                s.Plan.PlanCode.ToLower().Contains(term) ||
                s.Plan.NameEn.ToLower().Contains(term) ||
                s.Plan.NameAr.Contains(term));
        }

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (userId.HasValue)
            query = query.Where(s => s.UserId == userId.Value);

        if (planId.HasValue)
            query = query.Where(s => s.PlanId == planId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(s => s.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.UserSubscriptions.AnyAsync(
            s => s.UserId == userId && (s.Status == SubscriptionStatus.Trial || s.Status == SubscriptionStatus.Active),
            cancellationToken);

    public async Task AddAsync(UserSubscription subscription, CancellationToken cancellationToken = default) =>
        await _context.UserSubscriptions.AddAsync(subscription, cancellationToken);

    public void Update(UserSubscription subscription) => _context.UserSubscriptions.Update(subscription);
}
