namespace Khadamati.Application.Interfaces;

public interface ILocationCatalogService
{
    Task<bool> IsActiveRegionAsync(string regionName, CancellationToken cancellationToken = default);
    Task<bool> IsActiveCityAsync(string cityName, string? regionName = null, CancellationToken cancellationToken = default);
}
