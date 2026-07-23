namespace Khadamati.Application.DTOs.Payments;

public class MoyasarWebhookDto
{
    public string? Id { get; set; }
    public string? InvoiceId { get; set; }
    public string? Status { get; set; }
    public int? Amount { get; set; }
    public string? Currency { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class AreebaWebhookDto
{
    public string? Id { get; set; }
    public string? EventId { get; set; }
    public string? OrderId { get; set; }
    public string? TransactionId { get; set; }
    public string? Status { get; set; }
    public string? Result { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
}

public class PaymentWebhookResultDto
{
    public bool Processed { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? BookingId { get; set; }
}
