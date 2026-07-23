using FluentAssertions;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
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
    private readonly PaymentAttemptRepository _attemptRepository;
    private readonly IPaymentAttemptService _paymentAttemptService;
    private readonly Mock<IPaymentGateway> _paymentGateway = new();
    private readonly Mock<IBookingService> _bookingService = new();
    private readonly Mock<IAuditService> _auditService = new();
    private readonly IUnitOfWork _unitOfWork;
    private readonly AreebaWebhookSignatureValidator _signatureValidator;
    private readonly AreebaWebhookService _service;
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly Guid _paymentId = Guid.NewGuid();
    private readonly Guid _attemptId = Guid.NewGuid();
    private const string SessionId = "areeba_sess_123";
    private const string EventId = "evt_123";
    private const string RawBody = """{"eventId":"evt_123","sessionId":"areeba_sess_123","status":"paid"}""";

    public AreebaWebhookServiceTests()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        _attemptRepository = new PaymentAttemptRepository(_context);
        _paymentAttemptService = new PaymentAttemptService(_attemptRepository);

        SeedAttempt();

        var env = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Development");
        _signatureValidator = new AreebaWebhookSignatureValidator(
            Options.Create(new AreebaOptions { WebhookSecret = "webhook_secret" }),
            env.Object,
            NullLogger<AreebaWebhookSignatureValidator>.Instance);

        _unitOfWork = new UnitOfWork(_context);

        _service = new AreebaWebhookService(
            _signatureValidator,
            _paymentAttemptService,
            _paymentGateway.Object,
            _bookingService.Object,
            _unitOfWork,
            _auditService.Object,
            NullLogger<AreebaWebhookService>.Instance);
    }

    [Fact]
    public async Task ProcessWebhookAsync_ValidSignature_ConfirmsPayment()
    {
        var signature = PaymentWebhookService.ComputeHmacSha256Hex("webhook_secret", RawBody);
        _paymentGateway.Setup(g => g.VerifyAsync(SessionId, 120m, "USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentVerificationResult { IsSuccessful = true, TransactionReference = "txn_1" });
        _bookingService.Setup(s => s.ConfirmPaymentFromWebhookAsync(SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = _bookingId });

        var result = await _service.ProcessWebhookAsync(
            new AreebaWebhookDto { EventId = EventId, SessionId = SessionId, Status = "paid" },
            signature,
            RawBody);

        result.Processed.Should().BeTrue();
        var attempt = await _context.BookingPaymentAttempts.SingleAsync();
        attempt.Status.Should().Be(PaymentAttemptStatus.Completed);
        attempt.ProviderTransactionId.Should().Be("txn_1");
    }

    [Fact]
    public async Task ProcessWebhookAsync_InvalidSignature_ThrowsUnauthorized()
    {
        var act = () => _service.ProcessWebhookAsync(
            new AreebaWebhookDto { EventId = EventId, SessionId = SessionId, Status = "paid" },
            "invalid",
            RawBody);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ProcessWebhookAsync_DuplicateWebhook_IsIgnored()
    {
        var signature = PaymentWebhookService.ComputeHmacSha256Hex("webhook_secret", RawBody);
        _paymentGateway.Setup(g => g.VerifyAsync(SessionId, 120m, "USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentVerificationResult { IsSuccessful = true, TransactionReference = "txn_1" });
        _bookingService.Setup(s => s.ConfirmPaymentFromWebhookAsync(SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = _bookingId });

        await _service.ProcessWebhookAsync(
            new AreebaWebhookDto { EventId = EventId, SessionId = SessionId, Status = "paid" },
            signature,
            RawBody);

        var second = await _service.ProcessWebhookAsync(
            new AreebaWebhookDto { EventId = EventId, SessionId = SessionId, Status = "paid" },
            signature,
            RawBody);

        second.Processed.Should().BeTrue();
        second.Message.Should().Contain("Duplicate");
        _bookingService.Verify(
            s => s.ConfirmPaymentFromWebhookAsync(SessionId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessWebhookAsync_UnknownPaymentAttempt_ReturnsNotFound()
    {
        var body = """{"eventId":"evt_unknown","sessionId":"missing","status":"paid"}""";
        var signature = PaymentWebhookService.ComputeHmacSha256Hex("webhook_secret", body);

        var result = await _service.ProcessWebhookAsync(
            new AreebaWebhookDto { EventId = "evt_unknown", SessionId = "missing", Status = "paid" },
            signature,
            body);

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

        await _context.PaymentWebhookEvents.AddAsync(new PaymentWebhookEvent
        {
            Provider = AreebaPaymentGateway.ProviderNameConst,
            WebhookEventId = "evt_delayed",
            BookingPaymentAttemptId = attempt.Id,
            ProcessedAt = DateTime.UtcNow,
            EventStatus = "paid",
        });
        await _context.SaveChangesAsync();

        var body = """{"eventId":"evt_delayed_new","sessionId":"areeba_sess_123","status":"paid"}""";
        var signature = PaymentWebhookService.ComputeHmacSha256Hex("webhook_secret", body);

        var result = await _service.ProcessWebhookAsync(
            new AreebaWebhookDto { EventId = "evt_delayed_new", SessionId = SessionId, Status = "paid" },
            signature,
            body);

        result.Processed.Should().BeTrue();
        result.Message.Should().Contain("already completed");
        _bookingService.Verify(
            s => s.ConfirmPaymentFromWebhookAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessWebhookAsync_CompletedAttemptCannotBeOverwritten()
    {
        var signature = PaymentWebhookService.ComputeHmacSha256Hex("webhook_secret", RawBody);
        _paymentGateway.Setup(g => g.VerifyAsync(SessionId, 120m, "USD", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentVerificationResult { IsSuccessful = true, TransactionReference = "txn_1" });
        _bookingService.Setup(s => s.ConfirmPaymentFromWebhookAsync(SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = _bookingId });

        await _service.ProcessWebhookAsync(
            new AreebaWebhookDto { EventId = EventId, SessionId = SessionId, Status = "paid" },
            signature,
            RawBody);

        var attempt = await _context.BookingPaymentAttempts.SingleAsync();
        attempt.Status = PaymentAttemptStatus.Completed;
        attempt.ProviderTransactionId = "txn_1";
        attempt.CompletedAt = DateTime.UtcNow;

        await _paymentAttemptService.MarkAttemptFailedAsync(attempt, "should not apply");
        await _paymentAttemptService.MarkAttemptCompletedAsync(attempt, "txn_overwrite");

        attempt.Status.Should().Be(PaymentAttemptStatus.Completed);
        attempt.ProviderTransactionId.Should().Be("txn_1");
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
            Status = PaymentStatus.Processing,
            PaymentMethod = "Card",
        });

        _context.BookingPaymentAttempts.Add(new BookingPaymentAttempt
        {
            Id = _attemptId,
            BookingPaymentId = _paymentId,
            Provider = AreebaPaymentGateway.ProviderNameConst,
            SessionId = SessionId,
            Amount = 120m,
            Currency = "USD",
            Status = PaymentAttemptStatus.Processing,
        });

        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();
}
