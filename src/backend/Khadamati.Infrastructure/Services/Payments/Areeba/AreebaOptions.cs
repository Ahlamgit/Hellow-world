namespace Khadamati.Infrastructure.Services.Payments.Areeba;

public class AreebaOptions
{
    public const string SectionName = "Payment:Areeba";
    public const string DefaultSignatureHeaderName = "X-Areeba-Signature";

    public string MerchantId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = "https://sandbox.areeba.example/v1";
    public string CallbackUrl { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string FailureUrl { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string SignatureHeaderName { get; set; } = DefaultSignatureHeaderName;
}
