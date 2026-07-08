using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;

namespace Khadamati.Application.Interfaces;

public interface ISubscriptionPlanService
{
    Task<SubscriptionPlanDto> CreateAsync(CreateSubscriptionPlanDto dto, string? createdBy, CancellationToken cancellationToken = default);
    Task<SubscriptionPlanDto> UpdateAsync(Guid id, UpdateSubscriptionPlanDto dto, string? updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, string? deletedBy, CancellationToken cancellationToken = default);
    Task<SubscriptionPlanDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<SubscriptionPlanDto>> SearchAsync(SubscriptionPlanListQueryDto query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SubscriptionPlanDto>> GetPublicPlansAsync(string? targetRole, CancellationToken cancellationToken = default);
    Task<SubscriptionPlanDto> CloneAsync(Guid id, CloneSubscriptionPlanDto dto, string? createdBy, CancellationToken cancellationToken = default);
    Task<PlanActionResponseDto> ActivateAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default);
    Task<PlanActionResponseDto> DeactivateAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default);
    Task<PlanActionResponseDto> SuspendAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default);
    Task<PlanActionResponseDto> ArchiveAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default);
}
