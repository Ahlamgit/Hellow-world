using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Payments.Commands;

public record ProcessMoyasarWebhookCommand(MoyasarWebhookDto Payload, string? Signature, string RawBody) : IRequest<PaymentWebhookResultDto>;

public class ProcessMoyasarWebhookCommandHandler : IRequestHandler<ProcessMoyasarWebhookCommand, PaymentWebhookResultDto>
{
    private readonly IPaymentWebhookService _webhooks;

    public ProcessMoyasarWebhookCommandHandler(IPaymentWebhookService webhooks) => _webhooks = webhooks;

    public Task<PaymentWebhookResultDto> Handle(ProcessMoyasarWebhookCommand request, CancellationToken cancellationToken) =>
        _webhooks.ProcessMoyasarWebhookAsync(request.Payload, request.Signature, request.RawBody, cancellationToken);
}
