using FluentAssertions;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Services.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Khadamati.Tests.Services;

public class PaymentWebhookServiceTests
{
    private readonly Mock<IBookingService> _bookingService = new();

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_InvalidSignature_ThrowsUnauthorized()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Moyasar:WebhookSecret"] = "expected-secret",
        }).Build();

        var service = new PaymentWebhookService(
            _bookingService.Object,
            config,
            NullLogger<PaymentWebhookService>.Instance);

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
            .Setup(s => s.ConfirmPaymentFromWebhookAsync("inv_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = bookingId });

        var service = new PaymentWebhookService(
            _bookingService.Object,
            new ConfigurationBuilder().Build(),
            NullLogger<PaymentWebhookService>.Instance);

        var result = await service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "paid", InvoiceId = "inv_123" },
            null,
            CancellationToken.None);

        result.Processed.Should().BeTrue();
        result.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_PendingStatus_IsIgnored()
    {
        var service = new PaymentWebhookService(
            _bookingService.Object,
            new ConfigurationBuilder().Build(),
            NullLogger<PaymentWebhookService>.Instance);

        var result = await service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "pending", InvoiceId = "inv_123" },
            null,
            CancellationToken.None);

        result.Processed.Should().BeFalse();
        _bookingService.Verify(
            s => s.ConfirmPaymentFromWebhookAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
