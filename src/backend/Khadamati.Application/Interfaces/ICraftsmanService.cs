using Khadamati.Application.DTOs.Craftsman;

namespace Khadamati.Application.Interfaces;

public interface ICraftsmanService
{
    Task<CraftsmanProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CraftsmanProfileDto> UpdateProfileAsync(Guid userId, UpdateCraftsmanProfileDto dto, CancellationToken cancellationToken = default);
    Task<CraftsmanServiceDto> UpsertServiceAsync(Guid userId, UpsertCraftsmanServiceDto dto, CancellationToken cancellationToken = default);
    Task DeleteServiceAsync(Guid userId, Guid craftsmanServiceId, CancellationToken cancellationToken = default);
    Task<CraftsmanWorkingHourDto> UpsertWorkingHourAsync(Guid userId, UpsertWorkingHourDto dto, CancellationToken cancellationToken = default);
    Task DeleteWorkingHourAsync(Guid userId, Guid workingHourId, CancellationToken cancellationToken = default);
}
