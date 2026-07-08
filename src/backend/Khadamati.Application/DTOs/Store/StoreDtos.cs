namespace Khadamati.Application.DTOs.Store;

public class StoreProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string? CommercialRegistration { get; set; }
    public string? Description { get; set; }
    public decimal Rating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsOpen { get; set; }
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
    public IReadOnlyList<StoreProductDto> Products { get; set; } = Array.Empty<StoreProductDto>();
}

public class StoreProductDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Sku { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateStoreProfileDto
{
    public string StoreName { get; set; } = string.Empty;
    public string? CommercialRegistration { get; set; }
    public string? Description { get; set; }
    public bool IsOpen { get; set; } = true;
    public string? OpeningTime { get; set; }
    public string? ClosingTime { get; set; }
}

public class UpsertStoreProductDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Sku { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
