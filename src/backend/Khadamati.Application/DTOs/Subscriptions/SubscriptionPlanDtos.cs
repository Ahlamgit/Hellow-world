using Khadamati.Domain.Constants;

namespace Khadamati.Application.DTOs.Subscriptions;

public class PlanBillingOptionDto
{
    public Guid? Id { get; set; }
    public string Cycle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateSubscriptionPlanDto
{
    public string PlanCode { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string Currency { get; set; } = PlatformDefaults.Currency;
    public string TargetRole { get; set; } = string.Empty;
    public string Status { get; set; } = "Inactive";

    public int DisplayPriority { get; set; }
    public int SearchPriority { get; set; }
    public bool IsFeatured { get; set; }
    public bool HomePageVisible { get; set; }
    public bool BannerVisible { get; set; }
    public bool CategoryVisible { get; set; }

    public int? MaxCategories { get; set; }
    public int? MaxServices { get; set; }
    public int? MaxPhotos { get; set; }
    public int? MaxVideos { get; set; }
    public int? MaxAdvertisements { get; set; }
    public int AdvertisementCredits { get; set; }
    public int FeaturedDays { get; set; }

    public bool VerificationBadge { get; set; }
    public bool PremiumBadge { get; set; }
    public bool StatisticsDashboard { get; set; }
    public bool Analytics { get; set; }
    public bool PriorityCustomerSupport { get; set; }

    public bool RenewalReminder { get; set; } = true;
    public bool AutoRenewal { get; set; }
    public bool ExpiryNotification { get; set; } = true;
    public int GracePeriodDays { get; set; }
    public int TrialDays { get; set; }

    public decimal? DiscountPercentage { get; set; }
    public bool CouponSupport { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? VatRate { get; set; }
    public bool PaymentRequired { get; set; } = true;
    public List<string> PaymentMethods { get; set; } = new();

    public string? PlanColor { get; set; }
    public string? PlanIcon { get; set; }

    public List<PlanBillingOptionDto> BillingOptions { get; set; } = new();
}

public class UpdateSubscriptionPlanDto : CreateSubscriptionPlanDto;

public class SubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string PlanCode { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string Currency { get; set; } = PlatformDefaults.Currency;
    public string TargetRole { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public int DisplayPriority { get; set; }
    public int SearchPriority { get; set; }
    public bool IsFeatured { get; set; }
    public bool HomePageVisible { get; set; }
    public bool BannerVisible { get; set; }
    public bool CategoryVisible { get; set; }

    public int? MaxCategories { get; set; }
    public int? MaxServices { get; set; }
    public int? MaxPhotos { get; set; }
    public int? MaxVideos { get; set; }
    public int? MaxAdvertisements { get; set; }
    public int AdvertisementCredits { get; set; }
    public int FeaturedDays { get; set; }

    public bool VerificationBadge { get; set; }
    public bool PremiumBadge { get; set; }
    public bool StatisticsDashboard { get; set; }
    public bool Analytics { get; set; }
    public bool PriorityCustomerSupport { get; set; }

    public bool RenewalReminder { get; set; }
    public bool AutoRenewal { get; set; }
    public bool ExpiryNotification { get; set; }
    public int GracePeriodDays { get; set; }
    public int TrialDays { get; set; }

    public decimal? DiscountPercentage { get; set; }
    public bool CouponSupport { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? VatRate { get; set; }
    public bool PaymentRequired { get; set; }
    public List<string> PaymentMethods { get; set; } = new();

    public string? PlanColor { get; set; }
    public string? PlanIcon { get; set; }

    public Guid? ClonedFromPlanId { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<PlanBillingOptionDto> BillingOptions { get; set; } = new();
}

public class SubscriptionPlanListQueryDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? TargetRole { get; set; }
    public bool? Featured { get; set; }
    public bool IncludeArchived { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CloneSubscriptionPlanDto
{
    public string? NewPlanCode { get; set; }
    public string? NameEnSuffix { get; set; } = " (Copy)";
    public string? NameArSuffix { get; set; } = " (نسخة)";
}

public class PlanActionResponseDto
{
    public Guid PlanId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
