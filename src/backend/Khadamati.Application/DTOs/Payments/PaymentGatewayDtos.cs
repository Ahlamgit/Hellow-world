using Khadamati.Domain.Constants;

namespace Khadamati.Application.DTOs.Payments;

public class PaymentInitializationRequest
{
    public Guid PaymentId { get; set; }
    public Guid AttemptId { get; set; }
    public string MerchantTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = PlatformDefaults.Currency;
    public string Description { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerFirstName { get; set; }
    public string? CustomerLastName { get; set; }
}

public class PaymentInitializationDto
{
    public Guid AttemptId { get; set; }
    public string MerchantTransactionId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = PlatformDefaults.Currency;
    public string? PublicIntegrationKey { get; set; }
    public string? PaymentJsScriptUrl { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? SessionId { get; set; }
}

public class PaymentAuthorizationRequest
{
    public Guid AttemptId { get; set; }
    public Guid PaymentId { get; set; }
    public string MerchantTransactionId { get; set; } = string.Empty;
    public string TransactionToken { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = PlatformDefaults.Currency;
    public string Description { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerFirstName { get; set; }
    public string? CustomerLastName { get; set; }
}

public class PaymentAuthorizationResult
{
    public bool IsAccepted { get; set; }
    public string? ProviderUuid { get; set; }
    public string? GatewayStatus { get; set; }
    public string? ReturnType { get; set; }
    public string? RedirectUrl { get; set; }
    public string? FailureReason { get; set; }
}
