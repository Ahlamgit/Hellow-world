using FluentAssertions;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Tests.Services;

public class UserSubscriptionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserSubscriptionService _service;
    private readonly Guid _craftsmanUserId;
    private readonly Guid _planId;
    private readonly Guid _billingOptionId;

    public UserSubscriptionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        var subscriptionRepository = new UserSubscriptionRepository(_context);
        var planRepository = new SubscriptionPlanRepository(_context);
        var userRepository = new UserRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        _service = new UserSubscriptionService(subscriptionRepository, planRepository, userRepository, unitOfWork);

        _craftsmanUserId = Guid.NewGuid();
        _planId = Guid.NewGuid();
        _billingOptionId = Guid.NewGuid();

        var craftsman = new User
        {
            Id = _craftsmanUserId,
            Email = "craftsman@test.com",
            Phone = "+966500000010",
            PasswordHash = "hash",
            Role = UserRole.Craftsman,
            Status = UserStatus.Active,
            EmailVerifiedAt = DateTime.UtcNow,
            Profile = new UserProfile { FirstName = "Ali", LastName = "Craft" },
        };

        var plan = new SubscriptionPlan
        {
            Id = _planId,
            PlanCode = "CRAFTSMAN_BASIC",
            NameEn = "Craftsman Basic",
            NameAr = "حرفي أساسي",
            Currency = PlatformDefaults.Currency,
            TargetRole = UserRole.Craftsman,
            Status = PlanStatus.Active,
            TrialDays = 0,
            BillingOptions =
            [
                new PlanBillingOption
                {
                    Id = _billingOptionId,
                    PlanId = _planId,
                    Cycle = BillingCycle.Monthly,
                    Price = 99m,
                    DurationDays = 30,
                    IsActive = true,
                },
            ],
        };

        _context.Users.Add(craftsman);
        _context.SubscriptionPlans.Add(plan);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Subscribe_ShouldCreateActiveSubscriptionAndSyncUser()
    {
        var result = await _service.SubscribeAsync(_craftsmanUserId, new SubscribeRequestDto
        {
            PlanId = _planId,
            BillingOptionId = _billingOptionId,
            AutoRenew = true,
        }, CancellationToken.None);

        result.Status.Should().Be(nameof(SubscriptionStatus.Active));
        result.PlanCode.Should().Be("CRAFTSMAN_BASIC");
        result.AmountPaid.Should().Be(99m);

        var user = await _context.Users.FindAsync(_craftsmanUserId);
        user!.SubscriptionStatus.Should().Be(SubscriptionStatus.Active);
        user.SubscriptionExpiresAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Subscribe_ShouldRejectWhenActiveSubscriptionExists()
    {
        await _service.SubscribeAsync(_craftsmanUserId, new SubscribeRequestDto
        {
            PlanId = _planId,
            BillingOptionId = _billingOptionId,
        }, CancellationToken.None);

        var act = () => _service.SubscribeAsync(_craftsmanUserId, new SubscribeRequestDto
        {
            PlanId = _planId,
            BillingOptionId = _billingOptionId,
        }, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Cancel_ShouldMarkSubscriptionCancelled()
    {
        var created = await _service.SubscribeAsync(_craftsmanUserId, new SubscribeRequestDto
        {
            PlanId = _planId,
            BillingOptionId = _billingOptionId,
        }, CancellationToken.None);

        var result = await _service.CancelAsync(_craftsmanUserId, created.Id, new CancelSubscriptionDto
        {
            Reason = "No longer needed",
        }, CancellationToken.None);

        result.Status.Should().Be(nameof(SubscriptionStatus.Cancelled));

        var user = await _context.Users.FindAsync(_craftsmanUserId);
        user!.SubscriptionStatus.Should().Be(SubscriptionStatus.Cancelled);
    }

    public void Dispose() => _context.Dispose();
}
