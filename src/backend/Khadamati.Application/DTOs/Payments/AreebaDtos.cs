namespace Khadamati.Application.DTOs.Payments;

public class AreebaWebhookDto
{
    public string? EventId { get; set; }
    public string? SessionId { get; set; }
    public string? TransactionId { get; set; }
    public string? Status { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string? MerchantReference { get; set; }
    public string? FailureReason { get; set; }
}
