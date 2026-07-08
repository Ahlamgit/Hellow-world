using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Subscriptions.Queries;

public record GetSubscriptionPlanByIdQuery(Guid Id) : IRequest<SubscriptionPlanDto>;
public record SearchSubscriptionPlansQuery(SubscriptionPlanListQueryDto Query) : IRequest<PagedResult<SubscriptionPlanDto>>;
public record GetPublicSubscriptionPlansQuery(string? TargetRole) : IRequest<IReadOnlyList<SubscriptionPlanDto>>;

public class GetSubscriptionPlanByIdQueryHandler : IRequestHandler<GetSubscriptionPlanByIdQuery, SubscriptionPlanDto>
{
    private readonly ISubscriptionPlanService _service;
    public GetSubscriptionPlanByIdQueryHandler(ISubscriptionPlanService service) => _service = service;
    public Task<SubscriptionPlanDto> Handle(GetSubscriptionPlanByIdQuery request, CancellationToken cancellationToken) =>
        _service.GetByIdAsync(request.Id, cancellationToken);
}

public class SearchSubscriptionPlansQueryHandler : IRequestHandler<SearchSubscriptionPlansQuery, PagedResult<SubscriptionPlanDto>>
{
    private readonly ISubscriptionPlanService _service;
    public SearchSubscriptionPlansQueryHandler(ISubscriptionPlanService service) => _service = service;
    public Task<PagedResult<SubscriptionPlanDto>> Handle(SearchSubscriptionPlansQuery request, CancellationToken cancellationToken) =>
        _service.SearchAsync(request.Query, cancellationToken);
}

public class GetPublicSubscriptionPlansQueryHandler : IRequestHandler<GetPublicSubscriptionPlansQuery, IReadOnlyList<SubscriptionPlanDto>>
{
    private readonly ISubscriptionPlanService _service;
    public GetPublicSubscriptionPlansQueryHandler(ISubscriptionPlanService service) => _service = service;
    public Task<IReadOnlyList<SubscriptionPlanDto>> Handle(GetPublicSubscriptionPlansQuery request, CancellationToken cancellationToken) =>
        _service.GetPublicPlansAsync(request.TargetRole, cancellationToken);
}
