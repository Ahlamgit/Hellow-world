using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Khadamati.Tests.Services;

public class AreebaPaymentGatewayTests
{
    private const string BaseUrl = "https://sandbox.areeba.example/v1";

    [Fact]
    public async Task CreateSessionAsync_SuccessfulResponse_ReturnsSession()
    {
        var handler = new StubHandler(_ =>
            JsonResponse(HttpStatusCode.OK, new { sessionId = "sess_123", checkoutUrl = "https://pay.example/checkout/sess_123" }));
        var gateway = CreateGateway(handler);

        var result = await gateway.CreateSessionAsync(new Application.DTOs.Payments.PaymentSessionRequest
        {
            PaymentId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "USD",
            Description = "Test booking",
            CustomerEmail = "customer@test.com",
        });

        result.SessionId.Should().Be("sess_123");
        result.CheckoutUrl.Should().Contain("sess_123");
        result.Provider.Should().Be(AreebaPaymentGateway.ProviderNameConst);
    }

    [Fact]
    public async Task CreateSessionAsync_FailedResponse_Throws()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.BadRequest, new { error = "invalid" }));
        var gateway = CreateGateway(handler);

        var act = () => gateway.CreateSessionAsync(new Application.DTOs.Payments.PaymentSessionRequest
        {
            PaymentId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "USD",
        });

        await act.Should().ThrowAsync<Application.Common.ApplicationException>()
            .WithMessage("*Failed to create Areeba payment session*");
    }

    [Fact]
    public async Task CreateSessionAsync_InvalidResponse_Throws()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        });
        var gateway = CreateGateway(handler);

        var act = () => gateway.CreateSessionAsync(new Application.DTOs.Payments.PaymentSessionRequest
        {
            PaymentId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "USD",
        });

        await act.Should().ThrowAsync<Application.Common.ApplicationException>()
            .WithMessage("*missing session id or checkout url*");
    }

    [Fact]
    public async Task CreateSessionAsync_Timeout_Throws()
    {
        var handler = new StubHandler(_ => throw new TaskCanceledException());
        var gateway = CreateGateway(handler);

        var act = () => gateway.CreateSessionAsync(new Application.DTOs.Payments.PaymentSessionRequest
        {
            PaymentId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "USD",
        });

        await act.Should().ThrowAsync<Application.Common.ApplicationException>()
            .WithMessage("*timed out*");
    }

    [Fact]
    public async Task VerifyAsync_SuccessfulPayment_ReturnsVerified()
    {
        var handler = new StubHandler(request =>
        {
            request.RequestUri!.AbsolutePath.Should().Contain("txn_ok");
            return JsonResponse(HttpStatusCode.OK, new
            {
                sessionId = "txn_ok",
                transactionId = "areeba_txn_1",
                status = "paid",
                amount = 100m,
                currency = "USD",
            });
        });
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("txn_ok", 100m, "USD");

        result.IsSuccessful.Should().BeTrue();
        result.TransactionReference.Should().Be("areeba_txn_1");
    }

    [Fact]
    public async Task VerifyAsync_FailedPayment_ReturnsFailure()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            sessionId = "txn_fail",
            status = "failed",
            amount = 100m,
            currency = "USD",
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("txn_fail", 100m, "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("failed");
    }

    [Fact]
    public async Task VerifyAsync_AmountMismatch_ReturnsFailure()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            sessionId = "txn_amt",
            status = "paid",
            amount = 50m,
            currency = "USD",
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("txn_amt", 100m, "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("mismatch");
    }

    [Fact]
    public async Task VerifyAsync_CurrencyMismatch_ReturnsFailure()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            sessionId = "txn_cur",
            status = "paid",
            amount = 100m,
            currency = "LBP",
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("txn_cur", 100m, "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("mismatch");
    }

    [Fact]
    public async Task VerifyAsync_UnknownTransaction_ReturnsFailure()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("missing_txn", 100m, "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("not found");
    }

    [Theory]
    [InlineData("paid", true)]
    [InlineData("captured", true)]
    [InlineData("success", true)]
    [InlineData("completed", true)]
    [InlineData("failed", false)]
    public async Task VerifyAsync_StatusMapping_Works(string status, bool expectedSuccess)
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            sessionId = "txn_status",
            status,
            amount = 100m,
            currency = "USD",
            transactionId = "txn_1",
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("txn_status", 100m, "USD");

        result.IsSuccessful.Should().Be(expectedSuccess);
    }

    private static AreebaPaymentGateway CreateGateway(HttpMessageHandler handler)
    {
        var factory = new StubHttpClientFactory(handler);
        var options = Options.Create(new AreebaOptions
        {
            MerchantId = "merchant_test",
            SecretKey = "secret_test",
            ApiBaseUrl = BaseUrl,
        });
        return new AreebaPaymentGateway(factory, options, NullLogger<AreebaPaymentGateway>.Instance);
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, object body) =>
        new(statusCode)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"),
        };

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler) { BaseAddress = new Uri(BaseUrl) };
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(responder(request));
    }
}
