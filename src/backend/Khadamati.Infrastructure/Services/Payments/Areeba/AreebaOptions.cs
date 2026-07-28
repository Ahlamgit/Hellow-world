namespace Khadamati.Infrastructure.Services.Payments.Areeba;

public class AreebaOptions
{
    public const string SectionName = "Payment:Areeba";
    public const string DefaultSignatureHeaderName = "X-Signature";
    public const string DefaultPaymentJsScriptUrl = "https://areeba.ixopaysandbox.com/js/integrated/payment.1.3.min.js";
    public const string DefaultApiBaseUrl = "https://areeba.ixopaysandbox.com";

    public string PublicIntegrationKey { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiUser { get; set; } = string.Empty;
    public string ApiPassword { get; set; } = string.Empty;
    public string SharedSecret { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = DefaultApiBaseUrl;
    public string PaymentJsScriptUrl { get; set; } = DefaultPaymentJsScriptUrl;
    public string CallbackUrl { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string FailureUrl { get; set; } = string.Empty;
    public string SignatureHeaderName { get; set; } = DefaultSignatureHeaderName;
    public string TransactionMode { get; set; } = "Debit";
}
