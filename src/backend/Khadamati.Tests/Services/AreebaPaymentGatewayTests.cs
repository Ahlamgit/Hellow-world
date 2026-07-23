using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Infrastructure.Services.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Khadamati.Tests.Services;

public class AreebaPaymentGatewayTests
{
    [Fact]
    public async Task CreateSessionAsync_WhenNotConfigured_Throws()
    {
        var gateway = CreateGateway(new Dictionary<string, string?>(), new StubHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)));

        var act = () => gateway.CreateSessionAsync(new PaymentSessionRequest
        {
            PaymentId = Guid.NewGuid(),
            Amount = 10m,
            Currency = "USD",
        });

        await act.Should().ThrowAsync<Khadamati.Application.Common.ApplicationException>()
            .WithMessage("*not configured*");
    }

    [Fact]
    public async Task CreateSessionAsync_ValidResponse_ReturnsOrderSession()
    {
        var paymentId = Guid.NewGuid();
        var handler = new StubHandler(req =>
        {
            req.Headers.Authorization.Should().NotBeNull();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"result":"SUCCESS","session":{"id":"SESSION123"}}""", Encoding.UTF8, "application/json"),
            };
        });

        var gateway = CreateGateway(ConfiguredSettings(), handler);
        var session = await gateway.CreateSessionAsync(new PaymentSessionRequest
        {
            PaymentId = paymentId,
            Amount = 25.50m,
            Currency = "USD",
            Description = "Test",
        });

        session.Provider.Should().Be("Areeba");
        session.SessionId.Should().Be(paymentId.ToString("N"));
        session.GatewaySessionId.Should().Be("SESSION123");
        session.CheckoutUrl.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateSessionAsync_GatewayFailure_Throws()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("bad"),
        });
        var gateway = CreateGateway(ConfiguredSettings(), handler);

        var act = () => gateway.CreateSessionAsync(new PaymentSessionRequest
        {
            PaymentId = Guid.NewGuid(),
            Amount = 10m,
            Currency = "USD",
        });

        await act.Should().ThrowAsync<Khadamati.Application.Common.ApplicationException>();
    }

    [Fact]
    public async Task VerifyAsync_WrongAmount_Fails()
    {
        var orderId = Guid.NewGuid().ToString("N");
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new { id = orderId, status = "CAPTURED", amount = 9.99, currency = "USD" }),
                Encoding.UTF8,
                "application/json"),
        });
        var gateway = CreateGateway(ConfiguredSettings(), handler);

        var result = await gateway.VerifyAsync(orderId, expectedAmount: 25m, currency: "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("amount or currency");
    }

    [Fact]
    public async Task VerifyAsync_WrongCurrency_Fails()
    {
        var orderId = Guid.NewGuid().ToString("N");
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new { id = orderId, status = "CAPTURED", amount = 25.00, currency = "EUR" }),
                Encoding.UTF8,
                "application/json"),
        });
        var gateway = CreateGateway(ConfiguredSettings(), handler);

        var result = await gateway.VerifyAsync(orderId, expectedAmount: 25m, currency: "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("amount or currency");
    }

    [Fact]
    public async Task VerifyAsync_CapturedMatching_Succeeds()
    {
        var orderId = Guid.NewGuid().ToString("N");
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new { id = orderId, status = "CAPTURED", amount = 25.00, currency = "USD" }),
                Encoding.UTF8,
                "application/json"),
        });
        var gateway = CreateGateway(ConfiguredSettings(), handler);

        var result = await gateway.VerifyAsync(orderId, expectedAmount: 25m, currency: "USD");

        result.IsSuccessful.Should().BeTrue();
        result.TransactionReference.Should().Be(orderId);
    }

    private static Dictionary<string, string?> ConfiguredSettings() => new()
    {
        ["Payment:Areeba:MerchantId"] = "MERCHANT1",
        ["Payment:Areeba:ApiPassword"] = "password",
        ["Payment:Areeba:ApiBaseUrl"] = "https://epayment.areeba.com",
        ["Payment:Areeba:ApiVersion"] = "100",
        ["Payment:CheckoutBaseUrl"] = "https://app.test/pay",
    };

    private static AreebaPaymentGateway CreateGateway(Dictionary<string, string?> settings, HttpMessageHandler handler)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var factory = new StubHttpClientFactory(handler);
        return new AreebaPaymentGateway(config, factory, NullLogger<AreebaPaymentGateway>.Instance);
    }

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler) { BaseAddress = new Uri("https://epayment.areeba.com") };
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(responder(request));
    }
}
