using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;

    public CouponService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CouponValidationResultDto> ValidateAsync(string code, decimal amount, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        var coupon = await _unitOfWork.Repository<Coupon>()
            .FirstOrDefaultAsync(c => c.Code == normalized && !c.IsDeleted, cancellationToken);

        if (coupon is null)
            return Invalid(code, "Coupon not found.");

        if (!coupon.IsActive)
            return Invalid(code, "Coupon is inactive.");

        var now = DateTime.UtcNow;
        if (coupon.ValidFrom > now || coupon.ValidTo < now)
            return Invalid(code, "Coupon is not valid at this time.");

        if (coupon.MaxUses > 0 && coupon.UsedCount >= coupon.MaxUses)
            return Invalid(code, "Coupon usage limit reached.");

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
