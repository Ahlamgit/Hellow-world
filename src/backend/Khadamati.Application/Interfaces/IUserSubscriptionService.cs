using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;

namespace Khadamati.Application.Interfaces;

public interface IUserSubscriptionService
{
    Task<UserSubscriptionDto?> GetCurrentAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserSubscriptionDto>> GetHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<UserSubscriptionDto> SubscribeAsync(Guid userId, SubscribeRequestDto request, CancellationToken cancellationToken = default);
    Task<SubscriptionActionResponseDto> CancelAsync(Guid userId, Guid subscriptionId, CancelSubscriptionDto request, CancellationToken cancellationToken = default);
    Task<UserSubscriptionDto> UpdateAutoRenewAsync(Guid userId, UpdateAutoRenewDto request, CancellationToken cancellationToken = default);
    Task<PagedResult<UserSubscriptionDto>> SearchAdminAsync(UserSubscriptionListQueryDto query, CancellationToken cancellationToken = default);
    Task<UserSubscriptionDto> GetByIdAdminAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserSubscriptionDto> GrantAsync(Guid userId, AdminGrantSubscriptionDto request, string? grantedBy, CancellationToken cancellationToken = default);
    Task<SubscriptionActionResponseDto> CancelAdminAsync(Guid subscriptionId, CancelSubscriptionDto request, string? cancelledBy, CancellationToken cancellationToken = default);
}
