using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Infrastructure.Services.Payments;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Khadamati.Tests.Services;

public class AreebaPaymentGatewayTests
{
    private const string BaseUrl = "https://areeba.ixopaysandbox.com";

    [Fact]
    public async Task InitialisePaymentAsync_ReturnsPaymentJsConfiguration()
    {
        var gateway = CreateGateway(new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)));

        var result = await gateway.InitialisePaymentAsync(new PaymentInitializationRequest
        {
            PaymentId = Guid.NewGuid(),
            AttemptId = Guid.NewGuid(),
            MerchantTransactionId = "merchant-txn-1",
            Amount = 100m,
            Currency = "USD",
        });

        result.PublicIntegrationKey.Should().Be("public_key_test");
        result.PaymentJsScriptUrl.Should().Contain("payment.1.3.min.js");
        result.Provider.Should().Be(AreebaPaymentGateway.ProviderNameConst);
        result.CheckoutUrl.Should().BeNull();
    }

    [Fact]
    public async Task AuthorizeAsync_SuccessfulDebit_ReturnsProviderUuid()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            success = true,
            uuid = "uuid_123",
            returnType = "FINISHED",
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.AuthorizeAsync(new PaymentAuthorizationRequest
        {
            AttemptId = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            MerchantTransactionId = "merchant-txn-1",
            TransactionToken = "token_123",
            Amount = 100m,
            Currency = "USD",
            CustomerEmail = "customer@test.com",
        });

        result.IsAccepted.Should().BeTrue();
        result.ProviderUuid.Should().Be("uuid_123");
        result.ReturnType.Should().Be("FINISHED");
    }

    [Fact]
    public async Task AuthorizeAsync_FailedDebit_ReturnsFailure()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            success = false,
            uuid = "uuid_fail",
            returnType = "ERROR",
            errors = new[] { new { errorMessage = "Transaction declined" } },
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.AuthorizeAsync(new PaymentAuthorizationRequest
        {
            AttemptId = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            MerchantTransactionId = "merchant-txn-1",
            TransactionToken = "token_123",
            Amount = 100m,
            Currency = "USD",
            CustomerEmail = "customer@test.com",
        });

        result.IsAccepted.Should().BeFalse();
        result.FailureReason.Should().Contain("declined");
    }

    [Fact]
    public async Task AuthorizeAsync_Timeout_ReturnsFailure()
    {
        var handler = new StubHandler(_ => throw new TaskCanceledException());
        var gateway = CreateGateway(handler);

        var result = await gateway.AuthorizeAsync(new PaymentAuthorizationRequest
        {
            AttemptId = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            MerchantTransactionId = "merchant-txn-1",
            TransactionToken = "token_123",
            Amount = 100m,
            Currency = "USD",
            CustomerEmail = "customer@test.com",
        });

        result.IsAccepted.Should().BeFalse();
        result.FailureReason.Should().Contain("timed out");
    }

    [Fact]
    public async Task VerifyAsync_AmountMismatch_ReturnsFailure()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            uuid = "uuid_123",
            result = "OK",
            returnType = "FINISHED",
            amount = "50.00",
            currency = "USD",
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("uuid_123", 100m, "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("mismatch");
    }

    [Fact]
    public async Task VerifyAsync_CurrencyMismatch_ReturnsFailure()
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            uuid = "uuid_123",
            result = "OK",
            returnType = "FINISHED",
            amount = "100.00",
            currency = "LBP",
        }));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("uuid_123", 100m, "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("mismatch");
    }

    [Fact]
    public async Task VerifyAsync_UnknownTransaction_ReturnsFailure()
    {
        var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var gateway = CreateGateway(handler);

        var result = await gateway.VerifyAsync("missing", 100m, "USD");

        result.IsSuccessful.Should().BeFalse();
        result.FailureReason.Should().Contain("not found");
    }

    [Theory]
    [InlineData("FINISHED", "OK", true)]
    [InlineData("PENDING", "PENDING", false)]
    [InlineData("ERROR", "ERROR", false)]
    public async Task VerifyAsync_ReturnTypeMapping_Works(string returnType, string result, bool expectedSuccess)
    {
        var handler = new StubHandler(_ => JsonResponse(HttpStatusCode.OK, new
        {
            uuid = "uuid_status",
            returnType,
            result,
            amount = "100.00",
            currency = "USD",
        }));
        var gateway = CreateGateway(handler);

        var verification = await gateway.VerifyAsync("uuid_status", 100m, "USD");

        verification.IsSuccessful.Should().Be(expectedSuccess);
    }

    private static AreebaPaymentGateway CreateGateway(HttpMessageHandler handler)
    {
        var factory = new StubHttpClientFactory(handler);
        var options = Options.Create(new AreebaOptions
        {
            PublicIntegrationKey = "public_key_test",
            ApiKey = "api_key_test",
            ApiUser = "api_user",
            ApiPassword = "api_password",
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
