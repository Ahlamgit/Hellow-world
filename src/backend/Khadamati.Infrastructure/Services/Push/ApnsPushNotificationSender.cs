using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Khadamati.Application.DTOs.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Khadamati.Infrastructure.Services.Push;

/// <summary>
/// Sends notifications to iOS devices via Apple Push Notification service (HTTP/2).
/// Configure Push:Apns:TeamId, KeyId, BundleId, and PrivateKey (.p8 PEM contents).
/// </summary>
public class ApnsPushNotificationSender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApnsPushNotificationSender> _logger;

    public ApnsPushNotificationSender(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ApnsPushNotificationSender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_configuration["Push:Apns:TeamId"]) &&
        !string.IsNullOrWhiteSpace(_configuration["Push:Apns:KeyId"]) &&
        !string.IsNullOrWhiteSpace(_configuration["Push:Apns:BundleId"]) &&
        !string.IsNullOrWhiteSpace(_configuration["Push:Apns:PrivateKey"]);

    public async Task SendAsync(
        string deviceToken,
        PushNotificationPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            _logger.LogWarning(
                "Push:Apns is not configured — logging iOS notification for user {UserId}: {TitleEn}",
                payload.UserId,
                payload.TitleEn);
            return;
        }

        var useSandbox = bool.TryParse(_configuration["Push:Apns:UseSandbox"], out var sandbox) && sandbox;
        var host = useSandbox
            ? "https://api.sandbox.push.apple.com"
            : "https://api.push.apple.com";

        var jwt = CreateProviderToken();
        var client = _httpClientFactory.CreateClient(nameof(ApnsPushNotificationSender));
        client.DefaultRequestVersion = HttpVersion.Version20;
        client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;

        var apnsPayload = new
        {
            aps = new
            {
                alert = new
                {
                    title = payload.TitleEn,
                    body = payload.MessageEn,
                },
                sound = "default",
            },
            type = payload.NotificationType,
            referenceId = payload.ReferenceId?.ToString(),
            titleAr = payload.TitleAr,
            messageAr = payload.MessageAr,
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{host}/3/device/{deviceToken}");
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", jwt);
        request.Headers.TryAddWithoutValidation("apns-topic", _configuration["Push:Apns:BundleId"]);
        request.Headers.TryAddWithoutValidation("apns-push-type", "alert");
        request.Headers.TryAddWithoutValidation("apns-priority", "10");
        request.Content = new StringContent(
            JsonSerializer.Serialize(apnsPayload),
            Encoding.UTF8,
            "application/json");

        var response = await client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "APNs push delivered to user {UserId}: {TitleEn}",
                payload.UserId,
                payload.TitleEn);
            return;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError(
            "APNs push failed ({StatusCode}) for user {UserId}: {Body}",
            (int)response.StatusCode,
            payload.UserId,
            body);
    }

    private string CreateProviderToken()
    {
        var keyId = _configuration["Push:Apns:KeyId"]!;
        var teamId = _configuration["Push:Apns:TeamId"]!;
        var privateKeyPem = _configuration["Push:Apns:PrivateKey"]!
            .Replace("\\n", "\n", StringComparison.Ordinal);

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(privateKeyPem);

        var securityKey = new ECDsaSecurityKey(ecdsa) { KeyId = keyId };
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

        var token = new JwtSecurityToken(
            issuer: teamId,
            claims: null,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(50),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
