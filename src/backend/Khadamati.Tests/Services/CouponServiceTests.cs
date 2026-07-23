using FluentAssertions;
using Khadamati.Domain.Entities;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Tests.Services;

public class CouponServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CouponService _service;

    public CouponServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        var unitOfWork = new UnitOfWork(_context);
        _service = new CouponService(unitOfWork, _context);
    }

    [Fact]
    public async Task ValidateAsync_ValidCoupon_ReturnsDiscount()
    {
        await SeedCouponAsync("SAVE10", maxUses: 5, usedCount: 0, discountPercentage: 10m);

        var result = await _service.ValidateAsync("save10", 100m);

        result.IsValid.Should().BeTrue();
        result.DiscountAmount.Should().Be(10m);
        result.FinalAmount.Should().Be(90m);
    }

    [Fact]
    public async Task ValidateAsync_ExpiredCoupon_ReturnsInvalid()
    {
        await SeedCouponAsync("EXPIRED", validTo: DateTime.UtcNow.AddDays(-1));

        var result = await _service.ValidateAsync("EXPIRED", 100m);

        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("not valid");
    }

    [Fact]
    public async Task TryRedeemAsync_ShouldIncrementUsedCount()
    {
        await SeedCouponAsync("REDEEM1", maxUses: 2, usedCount: 0, discountPercentage: 20m);

        var result = await _service.TryRedeemAsync("REDEEM1", 50m);

        result.IsValid.Should().BeTrue();
        result.FinalAmount.Should().Be(40m);

        var coupon = await _context.Coupons.SingleAsync();
        coupon.UsedCount.Should().Be(1);
    }

    [Fact]
    public async Task TryRedeemAsync_AtMaxUses_ReturnsInvalid()
    {
        await SeedCouponAsync("MAXED", maxUses: 1, usedCount: 1);

        var result = await _service.TryRedeemAsync("MAXED", 50m);

        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("usage limit");
    }

    private async Task SeedCouponAsync(
        string code,
        int maxUses = 0,
        int usedCount = 0,
        decimal discountPercentage = 10m,
        DateTime? validTo = null)
    {
        await _context.Coupons.AddAsync(new Coupon
        {
            Code = code,
            DescriptionEn = "Test coupon",
            DescriptionAr = "قسيمة تجريبية",
            DiscountPercentage = discountPercentage,
            MaxUses = maxUses,
            UsedCount = usedCount,
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidTo = validTo ?? DateTime.UtcNow.AddDays(30),
            IsActive = true,
        });
        await _context.SaveChangesAsync();
    }

    public void Dispose() => _context.Dispose();
}
