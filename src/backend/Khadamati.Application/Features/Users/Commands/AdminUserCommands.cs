using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Features.Identity.Commands;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Users.Commands;

public record CreateAdminUserCommand(CreateAdminUserDto Request, Guid AdminUserId, string? IpAddress) : IRequest<AdminUserDetailDto>;
public record UpdateAdminUserCommand(Guid Id, UpdateAdminUserDto Request, Guid AdminUserId) : IRequest<AdminUserDetailDto>;
public record DeleteAdminUserCommand(Guid Id, Guid AdminUserId) : IRequest<Unit>;
public record SuspendUserCommand(Guid Id, SuspendUserDto Request, Guid AdminUserId) : IRequest<UserActionResponseDto>;
public record ActivateUserCommand(Guid Id, Guid AdminUserId) : IRequest<UserActionResponseDto>;
public record AssignUserRolesCommand(Guid Id, AssignUserRolesDto Request, Guid AdminUserId) : IRequest<AdminUserDetailDto>;
public record AdminVerifyUserEmailCommand(Guid Id, Guid AdminUserId) : IRequest<MessageResponseDto>;
public record GetUserPermissionMatrixQuery(Guid UserId) : IRequest<UserPermissionMatrixDto>;
public record UpdateUserPermissionsCommand(Guid UserId, UpdateUserPermissionsDto Request, Guid AdminUserId) : IRequest<UserPermissionMatrixDto>;

public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, AdminUserDetailDto>
{
    private readonly IUserManagementService _service;
    public CreateAdminUserCommandHandler(IUserManagementService service) => _service = service;
    public Task<AdminUserDetailDto> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken) =>
        _service.CreateAsync(request.Request, request.AdminUserId, request.IpAddress, cancellationToken);
}

public class UpdateAdminUserCommandHandler : IRequestHandler<UpdateAdminUserCommand, AdminUserDetailDto>
{
    private readonly IUserManagementService _service;
    public UpdateAdminUserCommandHandler(IUserManagementService service) => _service = service;
    public Task<AdminUserDetailDto> Handle(UpdateAdminUserCommand request, CancellationToken cancellationToken) =>
        _service.UpdateAsync(request.Id, request.Request, request.AdminUserId, cancellationToken);
}

public class DeleteAdminUserCommandHandler : IRequestHandler<DeleteAdminUserCommand, Unit>
{
    private readonly IUserManagementService _service;
    public DeleteAdminUserCommandHandler(IUserManagementService service) => _service = service;
    public async Task<Unit> Handle(DeleteAdminUserCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Id, request.AdminUserId, cancellationToken);
        return Unit.Value;
    }
}

public class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, UserActionResponseDto>
{
    private readonly IUserManagementService _service;
    public SuspendUserCommandHandler(IUserManagementService service) => _service = service;
    public Task<UserActionResponseDto> Handle(SuspendUserCommand request, CancellationToken cancellationToken) =>
        _service.SuspendAsync(request.Id, request.Request, request.AdminUserId, cancellationToken);
}

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserActionResponseDto>
{
    private readonly IUserManagementService _service;
    public ActivateUserCommandHandler(IUserManagementService service) => _service = service;
    public Task<UserActionResponseDto> Handle(ActivateUserCommand request, CancellationToken cancellationToken) =>
        _service.ActivateAsync(request.Id, request.AdminUserId, cancellationToken);
}

public class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, AdminUserDetailDto>
{
    private readonly IUserManagementService _service;
    public AssignUserRolesCommandHandler(IUserManagementService service) => _service = service;
    public Task<AdminUserDetailDto> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken) =>
        _service.AssignRolesAsync(request.Id, request.Request, request.AdminUserId, cancellationToken);
}

public class AdminVerifyUserEmailCommandHandler : IRequestHandler<AdminVerifyUserEmailCommand, MessageResponseDto>
{
    private readonly IAuthService _auth;
    public AdminVerifyUserEmailCommandHandler(IAuthService auth) => _auth = auth;
    public Task<MessageResponseDto> Handle(AdminVerifyUserEmailCommand request, CancellationToken cancellationToken) =>
        _auth.AdminVerifyEmailAsync(request.AdminUserId, new AdminVerifyEmailRequestDto(request.Id), cancellationToken);
}

public class GetUserPermissionMatrixQueryHandler : IRequestHandler<GetUserPermissionMatrixQuery, UserPermissionMatrixDto>
{
    private readonly IUserManagementService _service;
    public GetUserPermissionMatrixQueryHandler(IUserManagementService service) => _service = service;
    public Task<UserPermissionMatrixDto> Handle(GetUserPermissionMatrixQuery request, CancellationToken cancellationToken) =>
        _service.GetPermissionMatrixAsync(request.UserId, cancellationToken);
}

public class UpdateUserPermissionsCommandHandler : IRequestHandler<UpdateUserPermissionsCommand, UserPermissionMatrixDto>
{
    private readonly IUserManagementService _service;
    public UpdateUserPermissionsCommandHandler(IUserManagementService service) => _service = service;
    public Task<UserPermissionMatrixDto> Handle(UpdateUserPermissionsCommand request, CancellationToken cancellationToken) =>
        _service.UpdatePermissionsAsync(request.UserId, request.Request, request.AdminUserId, cancellationToken);
}
