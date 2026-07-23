using Khadamati.Domain.Common;
using Khadamati.Domain.Constants;

namespace Khadamati.Domain.Entities;

public class Address : BaseEntity
{
    public Guid UserId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = PlatformDefaults.CountryCode;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsDefault { get; set; }

    public User User { get; set; } = null!;
}
