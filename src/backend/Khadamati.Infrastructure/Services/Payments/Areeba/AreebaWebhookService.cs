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
        AreebaWebhookDto payload,
        string? signature,
        string rawBody,
        CancellationToken cancellationToken = default)
    {
        _signatureValidator.Validate(signature, rawBody);

        var eventId = payload.EventId?.Trim();
        if (string.IsNullOrWhiteSpace(eventId))
            return new PaymentWebhookResultDto { Processed = false, Message = "Missing webhook event id." };

        var sessionReference = ResolveSessionReference(payload);
        if (string.IsNullOrWhiteSpace(sessionReference))
            return new PaymentWebhookResultDto { Processed = false, Message = "Missing payment session reference." };

        var attempt = await _paymentAttemptService.GetBySessionIdAsync(sessionReference, cancellationToken);
        if (attempt is null)
        {
            _logger.LogWarning("Areeba webhook for unknown session {SessionReference}", sessionReference);
            return new PaymentWebhookResultDto { Processed = false, Message = "Payment attempt not found." };
        }

        if (!await _paymentAttemptService.TryRegisterWebhookEventAsync(
                AreebaPaymentGateway.ProviderNameConst, eventId, attempt.Id, payload.Status, cancellationToken))
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

        var status = payload.Status?.ToLowerInvariant() ?? string.Empty;
        if (status is not ("paid" or "captured" or "success" or "completed"))
        {
            var failureReason = payload.FailureReason ?? $"Ignored status: {payload.Status}";
            await _paymentAttemptService.MarkAttemptFailedAsync(attempt, failureReason, cancellationToken);
            await UpdateBookingPaymentFailureAsync(attempt.BookingPaymentId, failureReason, cancellationToken);
            return new PaymentWebhookResultDto { Processed = false, Message = failureReason };
        }

        var verification = await _paymentGateway.VerifyAsync(
            sessionReference, attempt.Amount, attempt.Currency, cancellationToken);

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
            attempt, verification.TransactionReference ?? payload.TransactionId, cancellationToken);

        try
        {
            var booking = await _bookingService.ConfirmPaymentFromWebhookAsync(sessionReference, cancellationToken);
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

    private static string? ResolveSessionReference(AreebaWebhookDto payload)
    {
        if (!string.IsNullOrWhiteSpace(payload.SessionId))
            return payload.SessionId.Trim();
        if (!string.IsNullOrWhiteSpace(payload.TransactionId))
            return payload.TransactionId.Trim();
        return payload.MerchantReference?.Trim();
    }
}
