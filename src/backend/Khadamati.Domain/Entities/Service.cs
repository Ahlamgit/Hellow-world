using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class Service : BaseEntity
{
    public Guid CategoryId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public decimal BasePrice { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int EstimatedDurationMinutes { get; set; }

    public ServiceCategory Category { get; set; } = null!;
    public ICollection<CraftsmanService> CraftsmanServices { get; set; } = new List<CraftsmanService>();
    public ICollection<ServiceRequest> Requests { get; set; } = new List<ServiceRequest>();
}
