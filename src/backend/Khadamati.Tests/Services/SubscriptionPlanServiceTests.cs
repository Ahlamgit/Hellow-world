using AutoMapper;
using FluentAssertions;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Mappings;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Tests.Services;

public class SubscriptionPlanServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SubscriptionPlanService _service;

    public SubscriptionPlanServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var repository = new SubscriptionPlanRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _service = new SubscriptionPlanService(repository, unitOfWork, mapper);
    }

    private static CreateSubscriptionPlanDto SamplePlan(string code = "CRAFTSMAN_BASIC") => new()
    {
        PlanCode = code,
        NameEn = "Craftsman Basic",
        NameAr = "حرفي أساسي",
        Currency = "SAR",
        TargetRole = "Craftsman",
        Status = "Inactive",
        BillingOptions = new List<PlanBillingOptionDto>
        {
            new() { Cycle = "Monthly", Price = 99m, DurationDays = 30 },
            new() { Cycle = "Annual", Price = 999m, DurationDays = 365 }
        },
        MaxServices = 10,
        VerificationBadge = true,
        PaymentMethods = new List<string> { "Card" }
    };

    [Fact]
    public async Task Create_ShouldPersistPlanWithBillingOptions()
    {
        var result = await _service.CreateAsync(SamplePlan(), "admin", CancellationToken.None);

        result.PlanCode.Should().Be("CRAFTSMAN_BASIC");
        result.BillingOptions.Should().HaveCount(2);
        result.Status.Should().Be("Inactive");
    }

    [Fact]
    public async Task Activate_ShouldSetStatusToActive()
    {
        var created = await _service.CreateAsync(SamplePlan("STORE_PRO"), "admin", CancellationToken.None);
        var result = await _service.ActivateAsync(created.Id, "admin", CancellationToken.None);

        result.Status.Should().Be(nameof(PlanStatus.Active));
    }

    [Fact]
    public async Task Clone_ShouldCreateCopyWithInactiveStatus()
    {
        var created = await _service.CreateAsync(SamplePlan("CRAFTSMAN_CLONE"), "admin", CancellationToken.None);
        var clone = await _service.CloneAsync(created.Id, new CloneSubscriptionPlanDto { NewPlanCode = "CRAFTSMAN_CLONE_COPY" }, "admin", CancellationToken.None);

        clone.PlanCode.Should().Be("CRAFTSMAN_CLONE_COPY");
        clone.Status.Should().Be(nameof(PlanStatus.Inactive));
        clone.ClonedFromPlanId.Should().Be(created.Id);
        clone.BillingOptions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPublicPlans_ShouldReturnOnlyActivePlans()
    {
        var plan = await _service.CreateAsync(SamplePlan("PUBLIC_TEST"), "admin", CancellationToken.None);
        await _service.ActivateAsync(plan.Id, "admin", CancellationToken.None);

        var publicPlans = await _service.GetPublicPlansAsync("Craftsman", CancellationToken.None);
        publicPlans.Should().ContainSingle(p => p.PlanCode == "PUBLIC_TEST");
    }

    public void Dispose() => _context.Dispose();
}
