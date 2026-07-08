using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Subscriptions.Queries;

public record GetCurrentUserSubscriptionQuery(Guid UserId) : IRequest<UserSubscriptionDto?>;
public record GetUserSubscriptionHistoryQuery(Guid UserId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<UserSubscriptionDto>>;
public record SearchAdminUserSubscriptionsQuery(UserSubscriptionListQueryDto Query) : IRequest<PagedResult<UserSubscriptionDto>>;
public record GetAdminUserSubscriptionByIdQuery(Guid Id) : IRequest<UserSubscriptionDto>;

public class GetCurrentUserSubscriptionQueryHandler : IRequestHandler<GetCurrentUserSubscriptionQuery, UserSubscriptionDto?>
{
    private readonly IUserSubscriptionService _service;
    public GetCurrentUserSubscriptionQueryHandler(IUserSubscriptionService service) => _service = service;
    public Task<UserSubscriptionDto?> Handle(GetCurrentUserSubscriptionQuery request, CancellationToken cancellationToken) =>
        _service.GetCurrentAsync(request.UserId, cancellationToken);
}

public class GetUserSubscriptionHistoryQueryHandler : IRequestHandler<GetUserSubscriptionHistoryQuery, PagedResult<UserSubscriptionDto>>
{
    private readonly IUserSubscriptionService _service;
    public GetUserSubscriptionHistoryQueryHandler(IUserSubscriptionService service) => _service = service;
    public Task<PagedResult<UserSubscriptionDto>> Handle(GetUserSubscriptionHistoryQuery request, CancellationToken cancellationToken) =>
        _service.GetHistoryAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
}

public class SearchAdminUserSubscriptionsQueryHandler : IRequestHandler<SearchAdminUserSubscriptionsQuery, PagedResult<UserSubscriptionDto>>
{
    private readonly IUserSubscriptionService _service;
    public SearchAdminUserSubscriptionsQueryHandler(IUserSubscriptionService service) => _service = service;
    public Task<PagedResult<UserSubscriptionDto>> Handle(SearchAdminUserSubscriptionsQuery request, CancellationToken cancellationToken) =>
        _service.SearchAdminAsync(request.Query, cancellationToken);
}

public class GetAdminUserSubscriptionByIdQueryHandler : IRequestHandler<GetAdminUserSubscriptionByIdQuery, UserSubscriptionDto>
{
    private readonly IUserSubscriptionService _service;
    public GetAdminUserSubscriptionByIdQueryHandler(IUserSubscriptionService service) => _service = service;
    public Task<UserSubscriptionDto> Handle(GetAdminUserSubscriptionByIdQuery request, CancellationToken cancellationToken) =>
        _service.GetByIdAdminAsync(request.Id, cancellationToken);
}
