using FluentAssertions;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Validators;

namespace Khadamati.Tests.Validators;

public class SubscriptionPlanValidatorTests
{
    private readonly CreateSubscriptionPlanValidator _createValidator = new();
    private readonly CloneSubscriptionPlanValidator _cloneValidator = new();

    private static CreateSubscriptionPlanDto ValidPlan() => new()
    {
        PlanCode = "CRAFTSMAN_PRO",
        NameEn = "Craftsman Pro",
        NameAr = "حرفي احترافي",
        DescriptionEn = "Premium plan",
        Currency = "SAR",
        TargetRole = "Craftsman",
        Status = "Active",
        BillingOptions = new List<PlanBillingOptionDto>
        {
            new() { Cycle = "Monthly", Price = 99m, DurationDays = 30 },
            new() { Cycle = "Annual", Price = 999m, DurationDays = 365 }
        },
        PaymentMethods = new List<string> { "Card", "Mada" }
    };

    [Fact]
    public async Task Create_WithValidData_ShouldPass()
    {
        var result = await _createValidator.ValidateAsync(ValidPlan());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Create_WithInvalidPlanCode_ShouldFail()
    {
        var dto = ValidPlan();
        dto.PlanCode = "invalid-code";
        var result = await _createValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WithoutBillingOptions_ShouldFail()
    {
        var dto = ValidPlan();
        dto.BillingOptions = new List<PlanBillingOptionDto>();
        var result = await _createValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WithDuplicateBillingCycles_ShouldFail()
    {
        var dto = ValidPlan();
        dto.BillingOptions = new List<PlanBillingOptionDto>
        {
            new() { Cycle = "Monthly", Price = 99m, DurationDays = 30 },
            new() { Cycle = "Monthly", Price = 89m, DurationDays = 30 }
        };
        var result = await _createValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Clone_WithValidCode_ShouldPass()
    {
        var result = await _cloneValidator.ValidateAsync(new CloneSubscriptionPlanDto { NewPlanCode = "CRAFTSMAN_PRO_COPY" });
        result.IsValid.Should().BeTrue();
    }
}
