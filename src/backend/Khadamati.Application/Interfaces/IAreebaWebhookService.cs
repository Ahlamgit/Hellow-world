using Khadamati.Application.DTOs.Payments;

namespace Khadamati.Application.Interfaces;

public interface IAreebaWebhookService
{
    Task<PaymentWebhookResultDto> ProcessWebhookAsync(
        AreebaWebhookContext context,
        CancellationToken cancellationToken = default);
}
