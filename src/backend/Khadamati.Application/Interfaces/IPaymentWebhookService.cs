using Khadamati.Application.DTOs.Payments;

namespace Khadamati.Application.Interfaces;

public interface IPaymentWebhookService
{
    Task<PaymentWebhookResultDto> ProcessMoyasarWebhookAsync(
        MoyasarWebhookDto payload, string? signature, string rawBody, CancellationToken cancellationToken = default);
}
