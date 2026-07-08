using Khadamati.Application.DTOs.Craftsman;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Craftsman;

public record GetMyCraftsmanProfileQuery(Guid UserId) : IRequest<CraftsmanProfileDto>;
public record UpdateMyCraftsmanProfileCommand(Guid UserId, UpdateCraftsmanProfileDto Request) : IRequest<CraftsmanProfileDto>;
public record UpsertCraftsmanServiceCommand(Guid UserId, UpsertCraftsmanServiceDto Request) : IRequest<CraftsmanServiceDto>;
public record DeleteCraftsmanServiceCommand(Guid UserId, Guid CraftsmanServiceId) : IRequest<Unit>;
public record UpsertCraftsmanWorkingHourCommand(Guid UserId, UpsertWorkingHourDto Request) : IRequest<CraftsmanWorkingHourDto>;
public record DeleteCraftsmanWorkingHourCommand(Guid UserId, Guid WorkingHourId) : IRequest<Unit>;

public class GetMyCraftsmanProfileQueryHandler : IRequestHandler<GetMyCraftsmanProfileQuery, CraftsmanProfileDto>
{
    private readonly ICraftsmanService _service;
    public GetMyCraftsmanProfileQueryHandler(ICraftsmanService service) => _service = service;
    public Task<CraftsmanProfileDto> Handle(GetMyCraftsmanProfileQuery request, CancellationToken ct) =>
        _service.GetProfileAsync(request.UserId, ct);
}

public class UpdateMyCraftsmanProfileCommandHandler : IRequestHandler<UpdateMyCraftsmanProfileCommand, CraftsmanProfileDto>
{
    private readonly ICraftsmanService _service;
    public UpdateMyCraftsmanProfileCommandHandler(ICraftsmanService service) => _service = service;
    public Task<CraftsmanProfileDto> Handle(UpdateMyCraftsmanProfileCommand request, CancellationToken ct) =>
        _service.UpdateProfileAsync(request.UserId, request.Request, ct);
}

public class UpsertCraftsmanServiceCommandHandler : IRequestHandler<UpsertCraftsmanServiceCommand, CraftsmanServiceDto>
{
    private readonly ICraftsmanService _service;
    public UpsertCraftsmanServiceCommandHandler(ICraftsmanService service) => _service = service;
    public Task<CraftsmanServiceDto> Handle(UpsertCraftsmanServiceCommand request, CancellationToken ct) =>
        _service.UpsertServiceAsync(request.UserId, request.Request, ct);
}

public class DeleteCraftsmanServiceCommandHandler : IRequestHandler<DeleteCraftsmanServiceCommand, Unit>
{
    private readonly ICraftsmanService _service;
    public DeleteCraftsmanServiceCommandHandler(ICraftsmanService service) => _service = service;
    public async Task<Unit> Handle(DeleteCraftsmanServiceCommand request, CancellationToken ct)
    {
        await _service.DeleteServiceAsync(request.UserId, request.CraftsmanServiceId, ct);
        return Unit.Value;
    }
}

public class UpsertCraftsmanWorkingHourCommandHandler : IRequestHandler<UpsertCraftsmanWorkingHourCommand, CraftsmanWorkingHourDto>
{
    private readonly ICraftsmanService _service;
    public UpsertCraftsmanWorkingHourCommandHandler(ICraftsmanService service) => _service = service;
    public Task<CraftsmanWorkingHourDto> Handle(UpsertCraftsmanWorkingHourCommand request, CancellationToken ct) =>
        _service.UpsertWorkingHourAsync(request.UserId, request.Request, ct);
}

public class DeleteCraftsmanWorkingHourCommandHandler : IRequestHandler<DeleteCraftsmanWorkingHourCommand, Unit>
{
    private readonly ICraftsmanService _service;
    public DeleteCraftsmanWorkingHourCommandHandler(ICraftsmanService service) => _service = service;
    public async Task<Unit> Handle(DeleteCraftsmanWorkingHourCommand request, CancellationToken ct)
    {
        await _service.DeleteWorkingHourAsync(request.UserId, request.WorkingHourId, ct);
        return Unit.Value;
    }
}
