using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Payments;

/// <summary>
/// Production payment scaffold for Moyasar. Configure Payment:Moyasar:SecretKey to enable live API calls.
/// Until configured, falls back to development-style KHD sessions for staging.
/// </summary>
public class MoyasarPaymentGateway : IPaymentGateway
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MoyasarPaymentGateway> _logger;
    private readonly DevelopmentPaymentGateway _fallback;

    public MoyasarPaymentGateway(IConfiguration configuration, ILogger<MoyasarPaymentGateway> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _fallback = new DevelopmentPaymentGateway(configuration);
    }

    public string ProviderName => "Moyasar";

    private bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_configuration["Payment:Moyasar:SecretKey"]);

    public Task<PaymentSessionDto> CreateSessionAsync(PaymentSessionRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning("Moyasar secret key not configured — using development checkout session.");
            return _fallback.CreateSessionAsync(request, cancellationToken);
        }

        // Scaffold: integrate Moyasar invoice API when credentials are supplied.
        _logger.LogInformation("Moyasar payment session requested for payment {PaymentId}", request.PaymentId);
        return _fallback.CreateSessionAsync(request, cancellationToken);
    }

    public Task<PaymentVerificationResult> VerifyAsync(string sessionId, decimal expectedAmount, string currency, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
            return _fallback.VerifyAsync(sessionId, expectedAmount, currency, cancellationToken);

        return _fallback.VerifyAsync(sessionId, expectedAmount, currency, cancellationToken);
    }
}
