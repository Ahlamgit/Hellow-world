using Khadamati.Application.DTOs.Store;

namespace Khadamati.Application.Interfaces;

public interface IStoreService
{
    Task<StoreProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<StoreProfileDto> UpdateProfileAsync(Guid userId, UpdateStoreProfileDto dto, CancellationToken cancellationToken = default);
    Task<StoreProductDto> UpsertProductAsync(Guid userId, UpsertStoreProductDto dto, Guid? productId = null, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
}
