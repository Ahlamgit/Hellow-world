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
        MoyasarWebhookDto payload, string? signature, string rawBody, CancellationToken cancellationToken = default)
    {
        ValidateSignature(signature, rawBody);

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

    internal void ValidateSignature(string? signature, string rawBody)
    {
        var secret = _configuration["Payment:Moyasar:WebhookSecret"];
        var requireSecret = _environment.IsProduction() || _environment.IsStaging();

        if (string.IsNullOrWhiteSpace(secret))
        {
            if (requireSecret)
                throw new UnauthorizedException("Webhook secret is not configured.");
            return;
        }

        if (string.IsNullOrWhiteSpace(signature))
            throw new UnauthorizedException("Invalid webhook signature.");

        var computed = ComputeHmacSha256Hex(secret, rawBody);
        var provided = signature.Trim();

        if (!FixedTimeEqualsHex(computed, provided))
            throw new UnauthorizedException("Invalid webhook signature.");
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
        if (normalizedLeft.Length != normalizedRight.Length)
            return false;

        var leftBytes = Encoding.UTF8.GetBytes(normalizedLeft);
        var rightBytes = Encoding.UTF8.GetBytes(normalizedRight);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    private static string? ResolveTransactionReference(MoyasarWebhookDto payload)
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
