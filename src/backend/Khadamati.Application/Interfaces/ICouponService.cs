using Khadamati.Application.DTOs.Payments;

namespace Khadamati.Application.Interfaces;

public interface ICouponService
{
    Task<CouponValidationResultDto> ValidateAsync(string code, decimal amount, CancellationToken cancellationToken = default);
    Task<CouponValidationResultDto> TryRedeemAsync(string code, decimal amount, CancellationToken cancellationToken = default);
}
