using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class CraftsmanProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string? Specialization { get; set; }
    public int YearsOfExperience { get; set; }
    public decimal Rating { get; set; }
    public int TotalReviews { get; set; }
    public int CompletedJobs { get; set; }
    public bool IsAvailable { get; set; } = true;
    public decimal? ServiceRadiusKm { get; set; }
    public string? LicenseNumber { get; set; }

    public User User { get; set; } = null!;
    public ICollection<CraftsmanService> Services { get; set; } = new List<CraftsmanService>();
}
