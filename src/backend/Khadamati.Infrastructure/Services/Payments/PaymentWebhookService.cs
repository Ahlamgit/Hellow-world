using System.Security.Cryptography;
using System.Text;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Payments;

public class PaymentWebhookService : IPaymentWebhookService
{
    private readonly IBookingService _bookingService;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<PaymentWebhookService> _logger;

    public PaymentWebhookService(
        IBookingService bookingService,
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger<PaymentWebhookService> logger)
    {
        _bookingService = bookingService;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    public async Task<PaymentWebhookResultDto> ProcessMoyasarWebhookAsync(
        MoyasarWebhookDto payload, string? signature, CancellationToken cancellationToken = default)
    {
        ValidateSharedSecretSignature(
            signature,
            _configuration["Payment:Moyasar:WebhookSecret"],
            "Moyasar");

        var status = payload.Status?.ToLowerInvariant() ?? string.Empty;
        if (status is not ("paid" or "captured" or "success"))
        {
            _logger.LogInformation("Moyasar webhook ignored with status {Status}", payload.Status);
            return new PaymentWebhookResultDto { Processed = false, Message = $"Ignored status: {payload.Status}" };
        }

        var reference = ResolveMoyasarReference(payload);
        if (string.IsNullOrWhiteSpace(reference))
            return new PaymentWebhookResultDto { Processed = false, Message = "Missing transaction reference." };

        return await ConfirmAsync(reference, webhookEventId: payload.Id, cancellationToken);
    }

    public async Task<PaymentWebhookResultDto> ProcessAreebaWebhookAsync(
        AreebaWebhookDto payload, string? signature, string rawBody, CancellationToken cancellationToken = default)
    {
        ValidateHmacSignature(
            signature,
            rawBody,
            _configuration["Payment:Areeba:WebhookSecret"],
            "Areeba");

        var status = (payload.Status ?? payload.Result)?.ToUpperInvariant() ?? string.Empty;
        if (status is not ("CAPTURED" or "PAID" or "SUCCESS" or "AUTHORIZED" or "COMPLETED"))
        {
            _logger.LogInformation("Areeba webhook ignored with status {Status}", payload.Status ?? payload.Result);
            return new PaymentWebhookResultDto
            {
                Processed = false,
                Message = $"Ignored status: {payload.Status ?? payload.Result}",
            };
        }

        var reference = payload.OrderId
            ?? payload.TransactionId
            ?? payload.Id;
        if (string.IsNullOrWhiteSpace(reference))
            return new PaymentWebhookResultDto { Processed = false, Message = "Missing transaction reference." };

        var eventId = payload.EventId ?? payload.Id;
        return await ConfirmAsync(reference, eventId, cancellationToken);
    }

    private async Task<PaymentWebhookResultDto> ConfirmAsync(
        string reference, string? webhookEventId, CancellationToken cancellationToken)
    {
        try
        {
            var booking = await _bookingService.ConfirmPaymentFromWebhookAsync(
                reference, webhookEventId, cancellationToken);
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

    private void ValidateSharedSecretSignature(string? signature, string? secret, string provider)
    {
        var requireSecret = _environment.IsProduction() || _environment.IsStaging();
        if (string.IsNullOrWhiteSpace(secret))
        {
            if (requireSecret)
                throw new UnauthorizedException($"{provider} webhook secret is not configured.");
            return;
        }

        if (!string.Equals(signature, secret, StringComparison.Ordinal))
            throw new UnauthorizedException($"Invalid {provider} webhook signature.");
    }

    private void ValidateHmacSignature(string? signature, string rawBody, string? secret, string provider)
    {
        var requireSecret = _environment.IsProduction() || _environment.IsStaging();
        if (string.IsNullOrWhiteSpace(secret))
        {
            if (requireSecret)
                throw new UnauthorizedException($"{provider} webhook secret is not configured.");
            return;
        }

        if (string.IsNullOrWhiteSpace(signature))
            throw new UnauthorizedException($"Invalid {provider} webhook signature.");

        // Prefer HMAC-SHA256(hex). Also accept direct shared-secret equality for boarding flexibility.
        var computed = ComputeHmacSha256Hex(secret, rawBody ?? string.Empty);
        var provided = signature.Trim();
        if (FixedTimeEqualsHex(computed, provided)
            || string.Equals(provided, secret, StringComparison.Ordinal))
        {
            return;
        }

        throw new UnauthorizedException($"Invalid {provider} webhook signature.");
    }

    public static string ComputeHmacSha256Hex(string secret, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static bool FixedTimeEqualsHex(string left, string right)
    {
        var normalizedLeft = left.Trim().ToLowerInvariant();
        var normalizedRight = right.Trim().ToLowerInvariant();
        if (normalizedLeft.Length != normalizedRight.Length) return false;

        var result = 0;
        for (var i = 0; i < normalizedLeft.Length; i++)
            result |= normalizedLeft[i] ^ normalizedRight[i];
        return result == 0;
    }

    private static string? ResolveMoyasarReference(MoyasarWebhookDto payload)
    {
        if (!string.IsNullOrWhiteSpace(payload.InvoiceId))
            return payload.InvoiceId;

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
