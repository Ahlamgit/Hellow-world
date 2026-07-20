using FluentValidation;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Domain.Enums;

namespace Khadamati.Application.Validators;

public class PlanBillingOptionValidator : AbstractValidator<PlanBillingOptionDto>
{
    private static readonly string[] ValidCycles = Enum.GetNames<BillingCycle>();

    public PlanBillingOptionValidator()
    {
        RuleFor(x => x.Cycle)
            .NotEmpty()
            .Must(c => ValidCycles.Contains(c, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Cycle must be one of: {string.Join(", ", ValidCycles)}");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DurationDays)
            .GreaterThan(0)
            .When(x => !string.Equals(x.Cycle, nameof(BillingCycle.Lifetime), StringComparison.OrdinalIgnoreCase));

        RuleFor(x => x.DurationDays)
            .Equal(0)
            .When(x => string.Equals(x.Cycle, nameof(BillingCycle.Lifetime), StringComparison.OrdinalIgnoreCase));
    }
}

public class CreateSubscriptionPlanValidator : AbstractValidator<CreateSubscriptionPlanDto>
{
    private static readonly string[] ValidRoles = Enum.GetNames<UserRole>();
    private static readonly string[] ValidStatuses = Enum.GetNames<PlanStatus>();

    public CreateSubscriptionPlanValidator()
    {
        RuleFor(x => x.PlanCode)
            .NotEmpty().MaximumLength(50)
            .Matches(@"^[A-Z0-9_]+$").WithMessage("Plan code must contain only uppercase letters, numbers, and underscores.");

        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(150);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DescriptionEn).MaximumLength(2000);
        RuleFor(x => x.DescriptionAr).MaximumLength(2000);
        RuleFor(x => x.Currency).NotEmpty().Length(3);

        RuleFor(x => x.TargetRole)
            .NotEmpty()
            .Must(r => ValidRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Target role must be one of: {string.Join(", ", ValidRoles)}");

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.DisplayPriority).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SearchPriority).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxCategories).GreaterThanOrEqualTo(0).When(x => x.MaxCategories.HasValue);
        RuleFor(x => x.MaxServices).GreaterThanOrEqualTo(0).When(x => x.MaxServices.HasValue);
        RuleFor(x => x.MaxPhotos).GreaterThanOrEqualTo(0).When(x => x.MaxPhotos.HasValue);
        RuleFor(x => x.MaxVideos).GreaterThanOrEqualTo(0).When(x => x.MaxVideos.HasValue);
        RuleFor(x => x.MaxAdvertisements).GreaterThanOrEqualTo(0).When(x => x.MaxAdvertisements.HasValue);
        RuleFor(x => x.AdvertisementCredits).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FeaturedDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.GracePeriodDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TrialDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountPercentage).InclusiveBetween(0, 100).When(x => x.DiscountPercentage.HasValue);
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 100).When(x => x.TaxRate.HasValue);
        RuleFor(x => x.VatRate).InclusiveBetween(0, 100).When(x => x.VatRate.HasValue);
        RuleFor(x => x.PlanColor).MaximumLength(20);
        RuleFor(x => x.PlanIcon).MaximumLength(500);

        RuleFor(x => x.BillingOptions)
            .NotEmpty().WithMessage("At least one billing option is required.");

        RuleForEach(x => x.BillingOptions).SetValidator(new PlanBillingOptionValidator());

        RuleFor(x => x.BillingOptions)
            .Must(options => options.Select(o => o.Cycle.ToLowerInvariant()).Distinct().Count() == options.Count)
            .WithMessage("Duplicate billing cycles are not allowed.");
    }
}

public class UpdateSubscriptionPlanValidator : CreateSubscriptionPlanValidator;

public class CloneSubscriptionPlanValidator : AbstractValidator<CloneSubscriptionPlanDto>
{
    public CloneSubscriptionPlanValidator()
    {
        RuleFor(x => x.NewPlanCode)
            .MaximumLength(50)
            .Matches(@"^[A-Z0-9_]*$").WithMessage("Plan code must contain only uppercase letters, numbers, and underscores.")
            .When(x => !string.IsNullOrWhiteSpace(x.NewPlanCode));
    }
}

public class SubscriptionPlanListQueryValidator : AbstractValidator<SubscriptionPlanListQueryDto>
{
    public SubscriptionPlanListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
