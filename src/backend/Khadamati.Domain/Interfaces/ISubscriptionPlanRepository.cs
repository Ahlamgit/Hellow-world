using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Interfaces;

public interface ISubscriptionPlanRepository
{
    Task<SubscriptionPlan?> GetByIdAsync(Guid id, bool includeBillingOptions = false, CancellationToken cancellationToken = default);
    Task<SubscriptionPlan?> GetByCodeAsync(string planCode, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string planCode, Guid? excludePlanId = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SubscriptionPlan> Items, int TotalCount)> SearchAsync(
        string? search,
        PlanStatus? status,
        UserRole? targetRole,
        bool? featured,
        bool includeArchived,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SubscriptionPlan>> GetPublicPlansAsync(UserRole? targetRole, CancellationToken cancellationToken = default);
    Task AddAsync(SubscriptionPlan plan, CancellationToken cancellationToken = default);
    void Update(SubscriptionPlan plan);
    void SoftDelete(SubscriptionPlan plan, string? deletedBy = null);
}
