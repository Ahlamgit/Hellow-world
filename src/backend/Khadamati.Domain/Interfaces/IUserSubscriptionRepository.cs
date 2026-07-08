using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Interfaces;

public interface IUserSubscriptionRepository
{
    Task<UserSubscription?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<UserSubscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<UserSubscription> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<UserSubscription> Items, int TotalCount)> SearchAsync(
        string? search,
        SubscriptionStatus? status,
        Guid? userId,
        Guid? planId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserSubscription subscription, CancellationToken cancellationToken = default);
    void Update(UserSubscription subscription);
}
