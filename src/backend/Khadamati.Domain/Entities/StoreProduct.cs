using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class StoreProduct : BaseEntity
{
    public Guid StoreProfileId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Sku { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public StoreProfile StoreProfile { get; set; } = null!;
}
