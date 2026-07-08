using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class SubscriptionPlanRepository : ISubscriptionPlanRepository
{
    private readonly ApplicationDbContext _context;

    public SubscriptionPlanRepository(ApplicationDbContext context) => _context = context;

    public async Task<SubscriptionPlan?> GetByIdAsync(Guid id, bool includeBillingOptions = false, CancellationToken cancellationToken = default)
    {
        var query = _context.SubscriptionPlans.AsQueryable();
        if (includeBillingOptions)
            query = query.Include(p => p.BillingOptions.Where(b => !b.IsDeleted));
        return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<SubscriptionPlan?> GetByCodeAsync(string planCode, CancellationToken cancellationToken = default) =>
        _context.SubscriptionPlans
            .Include(p => p.BillingOptions.Where(b => !b.IsDeleted))
            .FirstOrDefaultAsync(p => p.PlanCode == planCode.ToUpperInvariant(), cancellationToken);

    public Task<bool> CodeExistsAsync(string planCode, Guid? excludePlanId = null, CancellationToken cancellationToken = default)
    {
        var code = planCode.ToUpperInvariant();
        return excludePlanId.HasValue
            ? _context.SubscriptionPlans.AnyAsync(p => p.PlanCode == code && p.Id != excludePlanId.Value, cancellationToken)
            : _context.SubscriptionPlans.AnyAsync(p => p.PlanCode == code, cancellationToken);
    }

    public async Task<(IReadOnlyList<SubscriptionPlan> Items, int TotalCount)> SearchAsync(
        string? search,
        PlanStatus? status,
        UserRole? targetRole,
        bool? featured,
        bool includeArchived,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SubscriptionPlans
            .Include(p => p.BillingOptions.Where(b => !b.IsDeleted))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.PlanCode.ToLower().Contains(term) ||
                p.NameEn.ToLower().Contains(term) ||
                p.NameAr.Contains(term));
        }

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);
        else if (!includeArchived)
            query = query.Where(p => p.Status != PlanStatus.Archived);

        if (targetRole.HasValue)
            query = query.Where(p => p.TargetRole == targetRole.Value);

        if (featured.HasValue)
            query = query.Where(p => p.IsFeatured == featured.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.DisplayPriority)
            .ThenBy(p => p.NameEn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<SubscriptionPlan>> GetPublicPlansAsync(UserRole? targetRole, CancellationToken cancellationToken = default)
    {
        var query = _context.SubscriptionPlans
            .Include(p => p.BillingOptions.Where(b => !b.IsDeleted && b.IsActive))
            .Where(p => p.Status == PlanStatus.Active)
            .AsQueryable();

        if (targetRole.HasValue)
            query = query.Where(p => p.TargetRole == targetRole.Value);

        return await query
            .OrderByDescending(p => p.DisplayPriority)
            .ThenByDescending(p => p.IsFeatured)
            .ThenBy(p => p.NameEn)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SubscriptionPlan plan, CancellationToken cancellationToken = default) =>
        await _context.SubscriptionPlans.AddAsync(plan, cancellationToken);

    public void Update(SubscriptionPlan plan) => _context.SubscriptionPlans.Update(plan);

    public void SoftDelete(SubscriptionPlan plan, string? deletedBy = null)
    {
        plan.IsDeleted = true;
        plan.DeletedAt = DateTime.UtcNow;
        plan.DeletedBy = deletedBy;
        _context.SubscriptionPlans.Update(plan);
    }
}
