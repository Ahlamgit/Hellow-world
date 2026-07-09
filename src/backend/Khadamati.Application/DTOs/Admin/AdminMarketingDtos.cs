namespace Khadamati.Application.DTOs.Admin;

public class CouponListQueryDto
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public class AdvertisementListQueryDto
{
    public string? Search { get; set; }
    public string? Placement { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int MaxUses { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCouponDto : CreateCouponDto;

public class AdminAdvertisementDto
{
    public Guid Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public string Placement { get; set; } = string.Empty;
    public Guid? TargetUserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Impressions { get; set; }
    public int Clicks { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAdvertisementDto
{
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public string Placement { get; set; } = "HomePage";
    public Guid? TargetUserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateAdvertisementDto : CreateAdvertisementDto;
