using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public CouponService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<CouponValidationResultDto> ValidateAsync(string code, decimal amount, CancellationToken cancellationToken = default)
    {
        var coupon = await GetCouponAsync(code, cancellationToken);
        if (coupon is null)
            return Invalid(code, "Coupon not found.");

        return ValidateCoupon(coupon, amount);
    }

    public async Task<CouponValidationResultDto> TryRedeemAsync(string code, decimal amount, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(code, amount, cancellationToken);
        if (!validation.IsValid)
            return validation;

        var normalized = code.Trim().ToUpperInvariant();
        var now = DateTime.UtcNow;

        if (_context.Database.IsRelational())
        {
            var updated = await _context.Coupons
                .Where(c => c.Code == normalized && !c.IsDeleted && c.IsActive
                    && c.ValidFrom <= now && c.ValidTo >= now
                    && (c.MaxUses == 0 || c.UsedCount < c.MaxUses))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.UsedCount, c => c.UsedCount + 1)
                    .SetProperty(c => c.UpdatedAt, now), cancellationToken);

            if (updated == 0)
                return Invalid(code, "Coupon usage limit reached.");

            return validation;
        }

        var coupon = await GetCouponAsync(code, cancellationToken);
        if (coupon is null || coupon.MaxUses > 0 && coupon.UsedCount >= coupon.MaxUses)
            return Invalid(code, "Coupon usage limit reached.");

        coupon.UsedCount++;
        coupon.UpdatedAt = now;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return validation;
    }

    private async Task<Coupon?> GetCouponAsync(string code, CancellationToken cancellationToken)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return await _unitOfWork.Repository<Coupon>()
            .FirstOrDefaultAsync(c => c.Code == normalized && !c.IsDeleted, cancellationToken);
    }

    private static CouponValidationResultDto ValidateCoupon(Coupon coupon, decimal amount)
    {
        if (!coupon.IsActive)
            return Invalid(coupon.Code, "Coupon is inactive.");

        var now = DateTime.UtcNow;
        if (coupon.ValidFrom > now || coupon.ValidTo < now)
            return Invalid(coupon.Code, "Coupon is not valid at this time.");

        if (coupon.MaxUses > 0 && coupon.UsedCount >= coupon.MaxUses)
            return Invalid(coupon.Code, "Coupon usage limit reached.");

        var discount = Math.Round(amount * coupon.DiscountPercentage / 100m, 2);
        if (coupon.MaxDiscountAmount is > 0)
            discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);
        discount = Math.Min(discount, amount);
        var final = amount - discount;

        return new CouponValidationResultDto
        {
            IsValid = true,
            Code = coupon.Code,
            DiscountAmount = discount,
            FinalAmount = final,
            Message = "Coupon applied successfully.",
        };
    }

    private static CouponValidationResultDto Invalid(string code, string message) => new()
    {
        IsValid = false,
        Code = code,
        Message = message,
    };
}
