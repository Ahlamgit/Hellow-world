using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class LocationCatalogService : ILocationCatalogService
{
    private readonly ApplicationDbContext _context;

    public LocationCatalogService(ApplicationDbContext context) => _context = context;

    public Task<bool> IsActiveRegionAsync(string regionName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(regionName)) return Task.FromResult(false);
        var normalized = regionName.Trim().ToLowerInvariant();
        return _context.Regions.AsNoTracking().AnyAsync(
            r => r.IsActive && !r.IsDeleted &&
                 (r.NameEn.ToLower() == normalized || r.NameAr.ToLower() == normalized || r.Code.ToLower() == normalized),
            cancellationToken);
    }

    public Task<bool> IsActiveCityAsync(string cityName, string? regionName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cityName)) return Task.FromResult(false);
        var normalizedCity = cityName.Trim().ToLowerInvariant();
        var query = _context.Cities.Include(c => c.Region).AsNoTracking()
            .Where(c => c.IsActive && !c.IsDeleted &&
                        (c.NameEn.ToLower() == normalizedCity || c.NameAr.ToLower() == normalizedCity || c.Code.ToLower() == normalizedCity));

        if (!string.IsNullOrWhiteSpace(regionName))
        {
            var normalizedRegion = regionName.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Region.NameEn.ToLower() == normalizedRegion ||
                c.Region.NameAr.ToLower() == normalizedRegion ||
                c.Region.Code.ToLower() == normalizedRegion);
        }

        return query.AnyAsync(cancellationToken);
    }
}
