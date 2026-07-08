namespace Khadamati.Application.DTOs.Subscriptions;

public class UserSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public Guid PlanId { get; set; }
    public string PlanCode { get; set; } = string.Empty;
    public string PlanNameEn { get; set; } = string.Empty;
    public string PlanNameAr { get; set; } = string.Empty;
    public Guid? BillingOptionId { get; set; }
    public string? BillingCycle { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public decimal? AmountPaid { get; set; }
    public string? Currency { get; set; }
    public string? CouponCode { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SubscribeRequestDto
{
    public Guid PlanId { get; set; }
    public Guid BillingOptionId { get; set; }
    public bool AutoRenew { get; set; } = true;
    public string? CouponCode { get; set; }
}

public class CancelSubscriptionDto
{
    public string? Reason { get; set; }
}

public class UpdateAutoRenewDto
{
    public bool AutoRenew { get; set; }
}

public class AdminGrantSubscriptionDto
{
    public Guid PlanId { get; set; }
    public Guid BillingOptionId { get; set; }
    public bool AutoRenew { get; set; }
    public string? Notes { get; set; }
}

public class UserSubscriptionListQueryDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public Guid? UserId { get; set; }
    public Guid? PlanId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SubscriptionActionResponseDto
{
    public Guid SubscriptionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
