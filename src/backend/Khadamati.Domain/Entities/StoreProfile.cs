using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class StoreProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? CommercialRegistration { get; set; }
    public string? Description { get; set; }
    public decimal Rating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsOpen { get; set; } = true;
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }

    public User User { get; set; } = null!;
    public ICollection<StoreProduct> Products { get; set; } = new List<StoreProduct>();
}
