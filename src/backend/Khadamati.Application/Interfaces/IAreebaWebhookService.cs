using Khadamati.Application.DTOs.Payments;

namespace Khadamati.Application.Interfaces;

public interface IAreebaWebhookService
{
    Task<PaymentWebhookResultDto> ProcessWebhookAsync(
        AreebaWebhookDto payload,
        string? signature,
        string rawBody,
        CancellationToken cancellationToken = default);
}
