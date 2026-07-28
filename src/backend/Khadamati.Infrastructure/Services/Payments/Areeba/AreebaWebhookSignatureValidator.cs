using System.Security.Cryptography;
using System.Text;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
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

    public void Validate(AreebaWebhookContext context)
    {
        var secret = _options.SharedSecret;
        var requireSecret = _environment.IsProduction() || _environment.IsStaging();

        if (string.IsNullOrWhiteSpace(secret))
        {
            if (requireSecret)
                throw new UnauthorizedException("Areeba shared secret is not configured.");
            _logger.LogWarning(
                "Areeba shared secret not configured — signature validation skipped in {Environment}.",
                _environment.EnvironmentName);
            return;
        }

        if (string.IsNullOrWhiteSpace(context.Signature))
            throw new UnauthorizedException("Invalid webhook signature.");

        var computed = ComputeSignature(secret, context);
        if (!FixedTimeEqualsBase64(computed, context.Signature))
            throw new UnauthorizedException("Invalid webhook signature.");
    }

    public static string ComputeSignature(string sharedSecret, AreebaWebhookContext context)
    {
        var bodyHash = Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(context.RawBody))).ToLowerInvariant();
        var message = string.Join('\n', new[]
        {
            "POST",
            bodyHash,
            context.ContentType ?? "application/json; charset=utf-8",
            context.DateHeader ?? string.Empty,
            context.RequestUri,
        });

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(sharedSecret));
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
    }

    private static bool FixedTimeEqualsBase64(string left, string right)
    {
        try
        {
            var leftBytes = Convert.FromBase64String(left.Trim());
            var rightBytes = Convert.FromBase64String(right.Trim());
            return leftBytes.Length == rightBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
