using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Subscriptions.Commands;

public record SubscribeCommand(Guid UserId, SubscribeRequestDto Request) : IRequest<UserSubscriptionDto>;
public record CancelUserSubscriptionCommand(Guid UserId, Guid SubscriptionId, CancelSubscriptionDto Request) : IRequest<SubscriptionActionResponseDto>;
public record UpdateAutoRenewCommand(Guid UserId, UpdateAutoRenewDto Request) : IRequest<UserSubscriptionDto>;
public record GrantUserSubscriptionCommand(Guid UserId, AdminGrantSubscriptionDto Request, string? GrantedBy) : IRequest<UserSubscriptionDto>;
public record CancelAdminUserSubscriptionCommand(Guid SubscriptionId, CancelSubscriptionDto Request, string? CancelledBy) : IRequest<SubscriptionActionResponseDto>;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, UserSubscriptionDto>
{
    private readonly IUserSubscriptionService _service;
    public SubscribeCommandHandler(IUserSubscriptionService service) => _service = service;
    public Task<UserSubscriptionDto> Handle(SubscribeCommand request, CancellationToken cancellationToken) =>
        _service.SubscribeAsync(request.UserId, request.Request, cancellationToken);
}

public class CancelUserSubscriptionCommandHandler : IRequestHandler<CancelUserSubscriptionCommand, SubscriptionActionResponseDto>
{
    private readonly IUserSubscriptionService _service;
    public CancelUserSubscriptionCommandHandler(IUserSubscriptionService service) => _service = service;
    public Task<SubscriptionActionResponseDto> Handle(CancelUserSubscriptionCommand request, CancellationToken cancellationToken) =>
        _service.CancelAsync(request.UserId, request.SubscriptionId, request.Request, cancellationToken);
}

public class UpdateAutoRenewCommandHandler : IRequestHandler<UpdateAutoRenewCommand, UserSubscriptionDto>
{
    private readonly IUserSubscriptionService _service;
    public UpdateAutoRenewCommandHandler(IUserSubscriptionService service) => _service = service;
    public Task<UserSubscriptionDto> Handle(UpdateAutoRenewCommand request, CancellationToken cancellationToken) =>
        _service.UpdateAutoRenewAsync(request.UserId, request.Request, cancellationToken);
}

public class GrantUserSubscriptionCommandHandler : IRequestHandler<GrantUserSubscriptionCommand, UserSubscriptionDto>
{
    private readonly IUserSubscriptionService _service;
    public GrantUserSubscriptionCommandHandler(IUserSubscriptionService service) => _service = service;
    public Task<UserSubscriptionDto> Handle(GrantUserSubscriptionCommand request, CancellationToken cancellationToken) =>
        _service.GrantAsync(request.UserId, request.Request, request.GrantedBy, cancellationToken);
}

public class CancelAdminUserSubscriptionCommandHandler : IRequestHandler<CancelAdminUserSubscriptionCommand, SubscriptionActionResponseDto>
{
    private readonly IUserSubscriptionService _service;
    public CancelAdminUserSubscriptionCommandHandler(IUserSubscriptionService service) => _service = service;
    public Task<SubscriptionActionResponseDto> Handle(CancelAdminUserSubscriptionCommand request, CancellationToken cancellationToken) =>
        _service.CancelAdminAsync(request.SubscriptionId, request.Request, request.CancelledBy, cancellationToken);
}
