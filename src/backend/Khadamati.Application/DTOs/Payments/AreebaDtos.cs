namespace Khadamati.Application.DTOs.Payments;

public class AreebaWebhookDto
{
    public string? Result { get; set; }
    public string? Uuid { get; set; }
    public string? MerchantTransactionId { get; set; }
    public string? PurchaseId { get; set; }
    public string? TransactionType { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Amount { get; set; }
    public string? Currency { get; set; }
    public string? Message { get; set; }
    public string? AdapterCode { get; set; }
    public string? AdapterMessage { get; set; }
}

public class AreebaWebhookContext
{
    public AreebaWebhookDto Payload { get; set; } = new();
    public string RawBody { get; set; } = string.Empty;
    public string? Signature { get; set; }
    public string? DateHeader { get; set; }
    public string? ContentType { get; set; }
    public string RequestUri { get; set; } = string.Empty;
}
