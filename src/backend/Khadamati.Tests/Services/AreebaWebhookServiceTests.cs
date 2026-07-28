using FluentAssertions;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services.Payments;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Khadamati.Tests.Services;

public class AreebaWebhookServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentAttemptService _paymentAttemptService;
    private readonly Mock<IPaymentGateway> _paymentGateway = new();
    private readonly Mock<IBookingService> _bookingService = new();
    private readonly Mock<IAuditService> _auditService = new();
    private readonly AreebaWebhookService _service;
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly Guid _paymentId = Guid.NewGuid();
    private readonly Guid _attemptId = Guid.NewGuid();
    private const string Uuid = "areeba_uuid_123";
    private const string MerchantTransactionId = "merchant_txn_123";
    private const string RawBody = """{"result":"OK","uuid":"areeba_uuid_123","merchantTransactionId":"merchant_txn_123","amount":"120.00","currency":"USD","transactionType":"DEBIT"}""";

    public AreebaWebhookServiceTests()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        _paymentAttemptService = new PaymentAttemptService(new PaymentAttemptRepository(_context));
        SeedAttempt();

        var env = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Development");
        var signatureValidator = new AreebaWebhookSignatureValidator(
            Options.Create(new AreebaOptions { SharedSecret = "shared_secret" }),
            env.Object,
            NullLogger<AreebaWebhookSignatureValidator>.Instance);

        _service = new AreebaWebhookService(
            signatureValidator,
            _paymentAttemptService,
            _paymentGateway.Object,
            _bookingService.Object,
            new UnitOfWork(_context),
            _auditService.Object,
            NullLogger<AreebaWebhookService>.Instance);
    }

    [Fact]
    public async Task ProcessWebhookAsync_ValidSignature_ConfirmsPayment()
    {
        var context = BuildContext(RawBody);
        _paymentGateway.Setup(g => g.VerifyAsync(Uuid, 120m, "USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentVerificationResult { IsSuccessful = true, TransactionReference = Uuid });
        _bookingService.Setup(s => s.ConfirmPaymentFromWebhookAsync(Uuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = _bookingId });

        var result = await _service.ProcessWebhookAsync(context);

        result.Processed.Should().BeTrue();
        var attempt = await _context.BookingPaymentAttempts.SingleAsync();
        attempt.Status.Should().Be(PaymentAttemptStatus.Completed);
    }

    [Fact]
    public async Task ProcessWebhookAsync_InvalidSignature_ThrowsUnauthorized()
    {
        var context = BuildContext(RawBody);
        context.Signature = "invalid";

        var act = () => _service.ProcessWebhookAsync(context);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ProcessWebhookAsync_DuplicateWebhook_IsIgnored()
    {
        var context = BuildContext(RawBody);
        _paymentGateway.Setup(g => g.VerifyAsync(Uuid, 120m, "USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentVerificationResult { IsSuccessful = true, TransactionReference = Uuid });
        _bookingService.Setup(s => s.ConfirmPaymentFromWebhookAsync(Uuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = _bookingId });

        await _service.ProcessWebhookAsync(context);
        var second = await _service.ProcessWebhookAsync(context);

        second.Processed.Should().BeTrue();
        second.Message.Should().Contain("Duplicate");
        _bookingService.Verify(s => s.ConfirmPaymentFromWebhookAsync(Uuid, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessWebhookAsync_UnknownPaymentAttempt_ReturnsNotFound()
    {
        var body = """{"result":"OK","uuid":"unknown","merchantTransactionId":"missing","amount":"120.00","currency":"USD"}""";
        var result = await _service.ProcessWebhookAsync(BuildContext(body));

        result.Processed.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task ProcessWebhookAsync_DelayedWebhook_OnCompletedAttempt_IsIgnored()
    {
        var attempt = await _context.BookingPaymentAttempts.SingleAsync();
        attempt.Status = PaymentAttemptStatus.Completed;
        attempt.CompletedAt = DateTime.UtcNow.AddMinutes(-5);
        await _context.SaveChangesAsync();

        var body = """{"result":"OK","uuid":"areeba_uuid_123","merchantTransactionId":"merchant_txn_123","amount":"120.00","currency":"USD"}""";
        var result = await _service.ProcessWebhookAsync(BuildContext(body));

        result.Processed.Should().BeTrue();
        result.Message.Should().Contain("already completed");
        _bookingService.Verify(s => s.ConfirmPaymentFromWebhookAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessWebhookAsync_CompletedAttemptCannotBeOverwritten()
    {
        var context = BuildContext(RawBody);
        _paymentGateway.Setup(g => g.VerifyAsync(Uuid, 120m, "USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentVerificationResult { IsSuccessful = true, TransactionReference = Uuid });
        _bookingService.Setup(s => s.ConfirmPaymentFromWebhookAsync(Uuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = _bookingId });

        await _service.ProcessWebhookAsync(context);

        var attempt = await _context.BookingPaymentAttempts.SingleAsync();
        await _paymentAttemptService.MarkAttemptFailedAsync(attempt, "should not apply");
        await _paymentAttemptService.MarkAttemptCompletedAsync(attempt, Uuid, "evt");

        attempt.Status.Should().Be(PaymentAttemptStatus.Completed);
        attempt.ProviderUuid.Should().Be(Uuid);
    }

    private AreebaWebhookContext BuildContext(string rawBody)
    {
        var payload = System.Text.Json.JsonSerializer.Deserialize<AreebaWebhookDto>(
            rawBody,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        var signature = AreebaWebhookSignatureValidator.ComputeSignature("shared_secret", new AreebaWebhookContext
        {
            RawBody = rawBody,
            ContentType = "application/json; charset=utf-8",
            DateHeader = "Mon, 01 Jan 2024 00:00:00 GMT",
            RequestUri = "/api/v1/webhooks/areeba",
        });

        return new AreebaWebhookContext
        {
            Payload = payload,
            RawBody = rawBody,
            Signature = signature,
            ContentType = "application/json; charset=utf-8",
            DateHeader = "Mon, 01 Jan 2024 00:00:00 GMT",
            RequestUri = "/api/v1/webhooks/areeba",
        };
    }

    private void SeedAttempt()
    {
        _context.BookingPayments.Add(new BookingPayment
        {
            Id = _paymentId,
            ServiceRequestId = _bookingId,
            PayerUserId = Guid.NewGuid(),
            PayeeUserId = Guid.NewGuid(),
            Amount = 120m,
            Currency = "USD",
            Status = PaymentStatus.AwaitingGatewayConfirmation,
            PaymentMethod = "Card",
        });

        _context.BookingPaymentAttempts.Add(new BookingPaymentAttempt
        {
            Id = _attemptId,
            BookingPaymentId = _paymentId,
            Provider = AreebaPaymentGateway.ProviderNameConst,
            MerchantTransactionId = MerchantTransactionId,
            SessionId = MerchantTransactionId,
            ProviderUuid = Uuid,
            Amount = 120m,
            Currency = "USD",
            Status = PaymentAttemptStatus.AwaitingGatewayConfirmation,
        });

        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();
}
