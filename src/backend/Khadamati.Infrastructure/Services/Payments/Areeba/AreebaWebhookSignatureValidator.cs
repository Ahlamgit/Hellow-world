using System.Security.Cryptography;
using System.Text;
using Khadamati.Application.Common;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Khadamati.Infrastructure.Services.Payments;

public class AreebaWebhookSignatureValidator : IAreebaWebhookSignatureValidator
{
    private readonly AreebaOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<AreebaWebhookSignatureValidator> _logger;

    public AreebaWebhookSignatureValidator(
        IOptions<AreebaOptions> options,
        IHostEnvironment environment,
        ILogger<AreebaWebhookSignatureValidator> logger)
    {
        _options = options.Value;
        _environment = environment;
        _logger = logger;
    }

    public void Validate(string? signature, string rawBody)
    {
        var secret = _options.WebhookSecret;
        var requireSecret = _environment.IsProduction() || _environment.IsStaging();

        if (string.IsNullOrWhiteSpace(secret))
        {
            if (requireSecret)
                throw new UnauthorizedException("Areeba webhook secret is not configured.");
            _logger.LogWarning("Areeba webhook secret not configured — signature validation skipped in {Environment}.", _environment.EnvironmentName);
            return;
        }

        if (string.IsNullOrWhiteSpace(signature))
            throw new UnauthorizedException("Invalid webhook signature.");

        var computed = PaymentWebhookService.ComputeHmacSha256Hex(secret, rawBody);
        if (!FixedTimeEqualsHex(computed, signature))
            throw new UnauthorizedException("Invalid webhook signature.");
    }

    private static bool FixedTimeEqualsHex(string left, string right)
    {
        var normalizedLeft = left.Trim().ToLowerInvariant();
        var normalizedRight = right.Trim().ToLowerInvariant();
        if (normalizedLeft.Length != normalizedRight.Length)
            return false;

        var leftBytes = Encoding.UTF8.GetBytes(normalizedLeft);
        var rightBytes = Encoding.UTF8.GetBytes(normalizedRight);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
