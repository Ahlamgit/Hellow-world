using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class PlanBillingOption : BaseEntity
{
    public Guid PlanId { get; set; }
    public BillingCycle Cycle { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;

    public SubscriptionPlan Plan { get; set; } = null!;
}
