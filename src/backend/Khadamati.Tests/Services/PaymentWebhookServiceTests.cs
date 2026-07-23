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

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_InvalidSignature_ThrowsUnauthorized()
    {
        const string secret = "expected-secret";
        const string rawBody = """{"status":"paid","invoiceId":"inv_1"}""";
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Moyasar:WebhookSecret"] = secret,
        }).Build();

        var service = new PaymentWebhookService(
            _bookingService.Object,
            config,
            CreateEnvironment(Environments.Production),
            NullLogger<PaymentWebhookService>.Instance);

        var act = () => service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "paid", InvoiceId = "inv_1" },
            "wrong-signature",
            rawBody,
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_ValidHmacSignature_ConfirmsBooking()
    {
        const string secret = "expected-secret";
        const string rawBody = """{"status":"paid","invoiceId":"inv_123"}""";
        var signature = PaymentWebhookService.ComputeHmacSha256Hex(secret, rawBody);
        var bookingId = Guid.NewGuid();

        _bookingService
            .Setup(s => s.ConfirmPaymentFromWebhookAsync("inv_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = bookingId });

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Moyasar:WebhookSecret"] = secret,
        }).Build();

        var service = new PaymentWebhookService(
            _bookingService.Object,
            config,
            CreateEnvironment(Environments.Production),
            NullLogger<PaymentWebhookService>.Instance);

        var result = await service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "paid", InvoiceId = "inv_123" },
            signature,
            rawBody,
            CancellationToken.None);

        result.Processed.Should().BeTrue();
        result.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public async Task ProcessMoyasarWebhookAsync_PaidStatusWithoutSecretInDevelopment_ConfirmsBooking()
    {
        var bookingId = Guid.NewGuid();
        _bookingService
            .Setup(s => s.ConfirmPaymentFromWebhookAsync("inv_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingDto { Id = bookingId });

        var service = new PaymentWebhookService(
            _bookingService.Object,
            new ConfigurationBuilder().Build(),
            CreateEnvironment(Environments.Development),
            NullLogger<PaymentWebhookService>.Instance);

        var result = await service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "paid", InvoiceId = "inv_123" },
            null,
            """{"status":"paid","invoiceId":"inv_123"}""",
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
            CreateEnvironment(Environments.Development),
            NullLogger<PaymentWebhookService>.Instance);

        var result = await service.ProcessMoyasarWebhookAsync(
            new MoyasarWebhookDto { Status = "pending", InvoiceId = "inv_123" },
            null,
            """{"status":"pending","invoiceId":"inv_123"}""",
            CancellationToken.None);

        result.Processed.Should().BeFalse();
        _bookingService.Verify(
            s => s.ConfirmPaymentFromWebhookAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static IHostEnvironment CreateEnvironment(string environmentName) =>
        new HostEnvironment { EnvironmentName = environmentName };

    private sealed class HostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "Khadamati.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
