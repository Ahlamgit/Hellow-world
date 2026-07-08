using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Users.Queries;

public record SearchAdminUsersQuery(AdminUserListQueryDto Query) : IRequest<PagedResult<AdminUserListItemDto>>;
public record GetAdminUserByIdQuery(Guid Id) : IRequest<AdminUserDetailDto>;

public class SearchAdminUsersQueryHandler : IRequestHandler<SearchAdminUsersQuery, PagedResult<AdminUserListItemDto>>
{
    private readonly IUserManagementService _service;
    public SearchAdminUsersQueryHandler(IUserManagementService service) => _service = service;
    public Task<PagedResult<AdminUserListItemDto>> Handle(SearchAdminUsersQuery request, CancellationToken cancellationToken) =>
        _service.SearchAsync(request.Query, cancellationToken);
}

public class GetAdminUserByIdQueryHandler : IRequestHandler<GetAdminUserByIdQuery, AdminUserDetailDto>
{
    private readonly IUserManagementService _service;
    public GetAdminUserByIdQueryHandler(IUserManagementService service) => _service = service;
    public Task<AdminUserDetailDto> Handle(GetAdminUserByIdQuery request, CancellationToken cancellationToken) =>
        _service.GetByIdAsync(request.Id, cancellationToken);
}
