using Khadamati.Domain.Common;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string PlanCode { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string Currency { get; set; } = PlatformDefaults.Currency;
    public UserRole TargetRole { get; set; }
    public PlanStatus Status { get; set; } = PlanStatus.Inactive;

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
    public string? PaymentMethods { get; set; }

    public string? PlanColor { get; set; }
    public string? PlanIcon { get; set; }

    public Guid? ClonedFromPlanId { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public ICollection<PlanBillingOption> BillingOptions { get; set; } = new List<PlanBillingOption>();
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
