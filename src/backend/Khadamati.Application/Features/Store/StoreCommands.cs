using Khadamati.Application.DTOs.Store;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Store;

public record GetMyStoreProfileQuery(Guid UserId) : IRequest<StoreProfileDto>;
public record UpdateMyStoreProfileCommand(Guid UserId, UpdateStoreProfileDto Request) : IRequest<StoreProfileDto>;
public record UpsertStoreProductCommand(Guid UserId, UpsertStoreProductDto Request, Guid? ProductId) : IRequest<StoreProductDto>;
public record DeleteStoreProductCommand(Guid UserId, Guid ProductId) : IRequest<Unit>;

public class GetMyStoreProfileQueryHandler : IRequestHandler<GetMyStoreProfileQuery, StoreProfileDto>
{
    private readonly IStoreService _service;
    public GetMyStoreProfileQueryHandler(IStoreService service) => _service = service;
    public Task<StoreProfileDto> Handle(GetMyStoreProfileQuery request, CancellationToken ct) =>
        _service.GetProfileAsync(request.UserId, ct);
}

public class UpdateMyStoreProfileCommandHandler : IRequestHandler<UpdateMyStoreProfileCommand, StoreProfileDto>
{
    private readonly IStoreService _service;
    public UpdateMyStoreProfileCommandHandler(IStoreService service) => _service = service;
    public Task<StoreProfileDto> Handle(UpdateMyStoreProfileCommand request, CancellationToken ct) =>
        _service.UpdateProfileAsync(request.UserId, request.Request, ct);
}

public class UpsertStoreProductCommandHandler : IRequestHandler<UpsertStoreProductCommand, StoreProductDto>
{
    private readonly IStoreService _service;
    public UpsertStoreProductCommandHandler(IStoreService service) => _service = service;
    public Task<StoreProductDto> Handle(UpsertStoreProductCommand request, CancellationToken ct) =>
        _service.UpsertProductAsync(request.UserId, request.Request, request.ProductId, ct);
}

public class DeleteStoreProductCommandHandler : IRequestHandler<DeleteStoreProductCommand, Unit>
{
    private readonly IStoreService _service;
    public DeleteStoreProductCommandHandler(IStoreService service) => _service = service;
    public async Task<Unit> Handle(DeleteStoreProductCommand request, CancellationToken ct)
    {
        await _service.DeleteProductAsync(request.UserId, request.ProductId, ct);
        return Unit.Value;
    }
}
