using System.Globalization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Payments;

public class AreebaWebhookService : IAreebaWebhookService
{
    private readonly IAreebaWebhookSignatureValidator _signatureValidator;
    private readonly IPaymentAttemptService _paymentAttemptService;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IBookingService _bookingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ILogger<AreebaWebhookService> _logger;

    public AreebaWebhookService(
        IAreebaWebhookSignatureValidator signatureValidator,
        IPaymentAttemptService paymentAttemptService,
        IPaymentGateway paymentGateway,
        IBookingService bookingService,
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        ILogger<AreebaWebhookService> logger)
    {
        _signatureValidator = signatureValidator;
        _paymentAttemptService = paymentAttemptService;
        _paymentGateway = paymentGateway;
        _bookingService = bookingService;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<PaymentWebhookResultDto> ProcessWebhookAsync(
        AreebaWebhookContext context,
        CancellationToken cancellationToken = default)
    {
        _signatureValidator.Validate(context);

        var payload = context.Payload;
        var uuid = payload.Uuid?.Trim();
        if (string.IsNullOrWhiteSpace(uuid))
            return new PaymentWebhookResultDto { Processed = false, Message = "Missing transaction uuid." };

        var merchantTransactionId = payload.MerchantTransactionId?.Trim();
        var attempt = !string.IsNullOrWhiteSpace(merchantTransactionId)
            ? await _paymentAttemptService.GetByMerchantTransactionIdAsync(merchantTransactionId, cancellationToken)
            : null;
        attempt ??= await _paymentAttemptService.GetByProviderUuidAsync(uuid, cancellationToken);

        if (attempt is null)
        {
            _logger.LogWarning("Areeba webhook for unknown transaction {Uuid}", uuid);
            return new PaymentWebhookResultDto { Processed = false, Message = "Payment attempt not found." };
        }

        var eventId = BuildWebhookEventId(uuid, payload.Result, payload.TransactionType);
        if (!await _paymentAttemptService.TryRegisterWebhookEventAsync(
                AreebaPaymentGateway.ProviderNameConst, eventId, attempt.Id, payload.Result, cancellationToken))
        {
            _logger.LogInformation("Duplicate Areeba webhook event {EventId} ignored", eventId);
            return new PaymentWebhookResultDto
            {
                Processed = true,
                Message = "Duplicate webhook ignored.",
                BookingId = attempt.BookingPayment?.ServiceRequestId,
            };
        }

        if (attempt.Status == PaymentAttemptStatus.Completed)
        {
            _logger.LogInformation("Areeba webhook received for already completed attempt {AttemptId}", attempt.Id);
            return new PaymentWebhookResultDto
            {
                Processed = true,
                Message = "Attempt already completed.",
                BookingId = attempt.BookingPayment?.ServiceRequestId,
            };
        }

        var result = payload.Result?.ToUpperInvariant() ?? string.Empty;
        if (result is "PENDING")
        {
            attempt.GatewayStatus = payload.Result;
            attempt.ProviderUuid = uuid;
            attempt.Status = PaymentAttemptStatus.AwaitingGatewayConfirmation;
            await _paymentAttemptService.ApplyAuthorizationResultAsync(
                attempt,
                new PaymentAuthorizationResult
                {
                    IsAccepted = true,
                    ProviderUuid = uuid,
                    GatewayStatus = payload.Result,
                    ReturnType = "PENDING",
                },
                attempt.TransactionToken,
                cancellationToken);

            return new PaymentWebhookResultDto
            {
                Processed = true,
                Message = "Pending webhook recorded.",
                BookingId = attempt.BookingPayment?.ServiceRequestId,
            };
        }

        if (result is not ("OK" or "SUCCESS"))
        {
            var failureReason = payload.Message ?? payload.AdapterMessage ?? $"Ignored result: {payload.Result}";
            await _paymentAttemptService.MarkAttemptFailedAsync(attempt, failureReason, cancellationToken);
            await UpdateBookingPaymentFailureAsync(attempt.BookingPaymentId, failureReason, cancellationToken);
            return new PaymentWebhookResultDto { Processed = false, Message = failureReason };
        }

        if (!TryValidateAmountAndCurrency(payload, attempt, out var mismatchReason))
        {
            await _paymentAttemptService.MarkAttemptFailedAsync(attempt, mismatchReason, cancellationToken);
            await UpdateBookingPaymentFailureAsync(attempt.BookingPaymentId, mismatchReason, cancellationToken);
            return new PaymentWebhookResultDto { Processed = false, Message = mismatchReason };
        }

        var verification = await _paymentGateway.VerifyAsync(uuid, attempt.Amount, attempt.Currency, cancellationToken);
        if (!verification.IsSuccessful)
        {
            var reason = verification.FailureReason ?? "Payment verification failed.";
            await _paymentAttemptService.MarkAttemptFailedAsync(attempt, reason, cancellationToken);
            await UpdateBookingPaymentFailureAsync(attempt.BookingPaymentId, reason, cancellationToken);
            await _auditService.LogSecurityEventAsync(
                attempt.BookingPayment?.PayerUserId ?? Guid.Empty,
                "AreebaWebhookVerificationFailed",
                reason,
                null,
                null,
                cancellationToken: cancellationToken);
            return new PaymentWebhookResultDto { Processed = false, Message = reason };
        }

        await _paymentAttemptService.MarkAttemptCompletedAsync(
            attempt, verification.TransactionReference ?? uuid, eventId, cancellationToken);

        try
        {
            var booking = await _bookingService.ConfirmPaymentFromWebhookAsync(uuid, cancellationToken);
            await _auditService.LogSecurityEventAsync(
                booking.CustomerId,
                "AreebaWebhookPaymentConfirmed",
                $"Payment confirmed via webhook event {eventId}",
                null,
                null,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Areeba webhook confirmed payment for booking {BookingId}", booking.Id);
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

    private static string BuildWebhookEventId(string uuid, string? result, string? transactionType) =>
        $"{uuid}:{result ?? "unknown"}:{transactionType ?? "unknown"}";

    private static bool TryValidateAmountAndCurrency(AreebaWebhookDto payload, BookingPaymentAttempt attempt, out string reason)
    {
        if (!string.IsNullOrWhiteSpace(payload.Amount) &&
            decimal.TryParse(payload.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) &&
            amount != attempt.Amount)
        {
            reason = "Payment amount mismatch.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(payload.Currency) &&
            !string.Equals(payload.Currency, attempt.Currency, StringComparison.OrdinalIgnoreCase))
        {
            reason = "Payment currency mismatch.";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private async Task UpdateBookingPaymentFailureAsync(Guid bookingPaymentId, string reason, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<BookingPayment>().GetByIdAsync(bookingPaymentId, cancellationToken);
        if (payment is null || payment.Status == PaymentStatus.Completed)
            return;

        payment.Status = PaymentStatus.Failed;
        payment.FailureReason = reason;
        _unitOfWork.Repository<BookingPayment>().Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
