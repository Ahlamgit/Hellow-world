using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Subscriptions.Commands;

public record CreateSubscriptionPlanCommand(CreateSubscriptionPlanDto Request, string? UserId) : IRequest<SubscriptionPlanDto>;
public record UpdateSubscriptionPlanCommand(Guid Id, UpdateSubscriptionPlanDto Request, string? UserId) : IRequest<SubscriptionPlanDto>;
public record DeleteSubscriptionPlanCommand(Guid Id, string? UserId) : IRequest<Unit>;
public record CloneSubscriptionPlanCommand(Guid Id, CloneSubscriptionPlanDto Request, string? UserId) : IRequest<SubscriptionPlanDto>;
public record ActivateSubscriptionPlanCommand(Guid Id, string? UserId) : IRequest<PlanActionResponseDto>;
public record DeactivateSubscriptionPlanCommand(Guid Id, string? UserId) : IRequest<PlanActionResponseDto>;
public record SuspendSubscriptionPlanCommand(Guid Id, string? UserId) : IRequest<PlanActionResponseDto>;
public record ArchiveSubscriptionPlanCommand(Guid Id, string? UserId) : IRequest<PlanActionResponseDto>;

public class CreateSubscriptionPlanCommandHandler : IRequestHandler<CreateSubscriptionPlanCommand, SubscriptionPlanDto>
{
    private readonly ISubscriptionPlanService _service;
    public CreateSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public Task<SubscriptionPlanDto> Handle(CreateSubscriptionPlanCommand request, CancellationToken cancellationToken) =>
        _service.CreateAsync(request.Request, request.UserId, cancellationToken);
}

public class UpdateSubscriptionPlanCommandHandler : IRequestHandler<UpdateSubscriptionPlanCommand, SubscriptionPlanDto>
{
    private readonly ISubscriptionPlanService _service;
    public UpdateSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public Task<SubscriptionPlanDto> Handle(UpdateSubscriptionPlanCommand request, CancellationToken cancellationToken) =>
        _service.UpdateAsync(request.Id, request.Request, request.UserId, cancellationToken);
}

public class DeleteSubscriptionPlanCommandHandler : IRequestHandler<DeleteSubscriptionPlanCommand, Unit>
{
    private readonly ISubscriptionPlanService _service;
    public DeleteSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public async Task<Unit> Handle(DeleteSubscriptionPlanCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Id, request.UserId, cancellationToken);
        return Unit.Value;
    }
}

public class CloneSubscriptionPlanCommandHandler : IRequestHandler<CloneSubscriptionPlanCommand, SubscriptionPlanDto>
{
    private readonly ISubscriptionPlanService _service;
    public CloneSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public Task<SubscriptionPlanDto> Handle(CloneSubscriptionPlanCommand request, CancellationToken cancellationToken) =>
        _service.CloneAsync(request.Id, request.Request, request.UserId, cancellationToken);
}

public class ActivateSubscriptionPlanCommandHandler : IRequestHandler<ActivateSubscriptionPlanCommand, PlanActionResponseDto>
{
    private readonly ISubscriptionPlanService _service;
    public ActivateSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public Task<PlanActionResponseDto> Handle(ActivateSubscriptionPlanCommand request, CancellationToken cancellationToken) =>
        _service.ActivateAsync(request.Id, request.UserId, cancellationToken);
}

public class DeactivateSubscriptionPlanCommandHandler : IRequestHandler<DeactivateSubscriptionPlanCommand, PlanActionResponseDto>
{
    private readonly ISubscriptionPlanService _service;
    public DeactivateSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public Task<PlanActionResponseDto> Handle(DeactivateSubscriptionPlanCommand request, CancellationToken cancellationToken) =>
        _service.DeactivateAsync(request.Id, request.UserId, cancellationToken);
}

public class SuspendSubscriptionPlanCommandHandler : IRequestHandler<SuspendSubscriptionPlanCommand, PlanActionResponseDto>
{
    private readonly ISubscriptionPlanService _service;
    public SuspendSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public Task<PlanActionResponseDto> Handle(SuspendSubscriptionPlanCommand request, CancellationToken cancellationToken) =>
        _service.SuspendAsync(request.Id, request.UserId, cancellationToken);
}

public class ArchiveSubscriptionPlanCommandHandler : IRequestHandler<ArchiveSubscriptionPlanCommand, PlanActionResponseDto>
{
    private readonly ISubscriptionPlanService _service;
    public ArchiveSubscriptionPlanCommandHandler(ISubscriptionPlanService service) => _service = service;
    public Task<PlanActionResponseDto> Handle(ArchiveSubscriptionPlanCommand request, CancellationToken cancellationToken) =>
        _service.ArchiveAsync(request.Id, request.UserId, cancellationToken);
}
