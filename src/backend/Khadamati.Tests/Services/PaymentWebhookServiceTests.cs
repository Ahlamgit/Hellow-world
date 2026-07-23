using FluentAssertions;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Services.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Khadamati.Tests.Services;

public class PaymentWebhookServiceTests
{
    private readonly Mock<IBookingService> _bookingService = new();
    private readonly Mock<IHostEnvironment> _environment = new();

    public PaymentWebhookServiceTests()
    {
        _environment.SetupGet(e => e.EnvironmentName).Returns(Environments.Development);
    }

    private PaymentWebhookService CreateService(IConfiguration? config = null) =>
        new(
            _bookingService.Object,
            config ?? new ConfigurationBuilder().Build(),
            _environment.Object,
            NullLogger<PaymentWebhookService>.Instance);

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_InvalidSignature_ThrowsUnauthorized()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Moyasar:WebhookSecret"] = "expected-secret",
        }).Build();

        var service = CreateService(config);

        var act = () => service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "paid", InvoiceId = "inv_1" },
            "wrong-secret",
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_PaidStatus_ConfirmsBooking()
    {
        var bookingId = Guid.NewGuid();
        _bookingService
            .Setup(s => s.ConfirmPaymentFromWebhookAsync("inv_123", "evt_1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = bookingId });

        var service = CreateService();

        var result = await service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Id = "evt_1", Status = "paid", InvoiceId = "inv_123" },
            null,
            CancellationToken.None);

        result.Processed.Should().BeTrue();
        result.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_PendingStatus_IsIgnored()
    {
        var service = CreateService();

        var result = await service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "pending", InvoiceId = "inv_123" },
            null,
            CancellationToken.None);

        result.Processed.Should().BeFalse();
        _bookingService.Verify(
            s => s.ConfirmPaymentFromWebhookAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessAreebaWebhookAsync_InvalidSignature_ThrowsUnauthorized()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Areeba:WebhookSecret"] = "areeba-secret",
        }).Build();
        var service = CreateService(config);
        var body = """{"orderId":"ord_1","status":"CAPTURED"}""";

        var act = () => service.ProcessAreebaWebhookAsync(
            new AreebaWebhookDto { OrderId = "ord_1", Status = "CAPTURED" },
            "bad-signature",
            body,
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ProcessAreebaWebhookAsync_ValidHmac_ConfirmsBooking()
    {
        var bookingId = Guid.NewGuid();
        var secret = "areeba-secret";
        var body = """{"orderId":"ord_99","status":"CAPTURED","eventId":"evt_99"}""";
        var signature = PaymentWebhookService.ComputeHmacSha256Hex(secret, body);

        _bookingService
            .Setup(s => s.ConfirmPaymentFromWebhookAsync("ord_99", "evt_99", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = bookingId });

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Areeba:WebhookSecret"] = secret,
        }).Build();
        var service = CreateService(config);

        var result = await service.ProcessAreebaWebhookAsync(
            new AreebaWebhookDto { OrderId = "ord_99", Status = "CAPTURED", EventId = "evt_99" },
            signature,
            body,
            CancellationToken.None);

        result.Processed.Should().BeTrue();
        result.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public async Task ProcessAreebaWebhookAsync_DuplicateEvent_IsIdempotentViaBookingService()
    {
        var bookingId = Guid.NewGuid();
        _bookingService
            .Setup(s => s.ConfirmPaymentFromWebhookAsync("ord_1", "evt_dup", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = bookingId });

        var service = CreateService();
        var payload = new AreebaWebhookDto { OrderId = "ord_1", Status = "SUCCESS", EventId = "evt_dup" };

        var first = await service.ProcessAreebaWebhookAsync(payload, null, "{}", CancellationToken.None);
        var second = await service.ProcessAreebaWebhookAsync(payload, null, "{}", CancellationToken.None);

        first.Processed.Should().BeTrue();
        second.Processed.Should().BeTrue();
        _bookingService.Verify(
            s => s.ConfirmPaymentFromWebhookAsync("ord_1", "evt_dup", It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}
