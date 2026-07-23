using Khadamati.Domain.Constants;

namespace Khadamati.Application.DTOs.Payments;

public class PaymentSessionRequest
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = PlatformDefaults.Currency;
    public string Description { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
}

public class PaymentSessionDto
{
    public string SessionId { get; set; } = string.Empty;
    public string? CheckoutUrl { get; set; }
    public string Provider { get; set; } = string.Empty;
}

public class PaymentVerificationResult
{
    public bool IsSuccessful { get; set; }
    public string? TransactionReference { get; set; }
    public string? FailureReason { get; set; }
}

public class ValidateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class CouponValidationResultDto
{
    public bool IsValid { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? Message { get; set; }
}
