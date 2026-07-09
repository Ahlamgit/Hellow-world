using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Payments;

public class PaymentWebhookService : IPaymentWebhookService
{
    private readonly IBookingService _bookingService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentWebhookService> _logger;

    public PaymentWebhookService(
        IBookingService bookingService,
        IConfiguration configuration,
        ILogger<PaymentWebhookService> logger)
    {
        _bookingService = bookingService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<PaymentWebhookResultDto> ProcessMoyasarWebhookAsync(
        MoyasarWebhookDto payload, string? signature, CancellationToken cancellationToken = default)
    {
        ValidateSignature(signature);

        var status = payload.Status?.ToLowerInvariant() ?? string.Empty;
        if (status is not ("paid" or "captured" or "success"))
        {
            _logger.LogInformation("Moyasar webhook ignored with status {Status}", payload.Status);
            return new PaymentWebhookResultDto { Processed = false, Message = $"Ignored status: {payload.Status}" };
        }

        var reference = ResolveTransactionReference(payload);
        if (string.IsNullOrWhiteSpace(reference))
            return new PaymentWebhookResultDto { Processed = false, Message = "Missing transaction reference." };

        try
        {
            var booking = await _bookingService.ConfirmPaymentFromWebhookAsync(reference, cancellationToken);
            _logger.LogInformation("Webhook confirmed payment for booking {BookingId}", booking.Id);
            return new PaymentWebhookResultDto
            {
                Processed = true,
                Message = "Payment confirmed.",
                BookingId = booking.Id,
            };
        }
        catch (NotFoundException)
        {
            return new PaymentWebhookResultDto { Processed = false, Message = "Payment reference not found." };
        }
        catch (ConflictException ex)
        {
            return new PaymentWebhookResultDto { Processed = false, Message = ex.Message };
        }
    }

    private void ValidateSignature(string? signature)
    {
        var secret = _configuration["Payment:Moyasar:WebhookSecret"];
        if (string.IsNullOrWhiteSpace(secret)) return;

        if (!string.Equals(signature, secret, StringComparison.Ordinal))
            throw new UnauthorizedException("Invalid webhook signature.");
    }

    private static string? ResolveTransactionReference(MoyasarWebhookDto payload)
    {
        if (payload.Metadata != null)
        {
            if (payload.Metadata.TryGetValue("session_id", out var sessionId) && !string.IsNullOrWhiteSpace(sessionId))
                return sessionId;
            if (payload.Metadata.TryGetValue("payment_id", out var paymentId) && Guid.TryParse(paymentId, out var id))
                return $"KHD-{id:N}";
        }

        return payload.Id;
    }
}
