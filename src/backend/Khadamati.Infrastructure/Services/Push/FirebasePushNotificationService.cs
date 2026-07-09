using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Khadamati.Application.DTOs.Messaging;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Push;

/// <summary>
/// Routes push delivery to FCM (Android) and APNs (iOS) based on stored device platform.
/// </summary>
public class FirebasePushNotificationService : IPushNotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApnsPushNotificationSender _apnsSender;
    private readonly ILogger<FirebasePushNotificationService> _logger;

    public FirebasePushNotificationService(
        ApplicationDbContext context,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ApnsPushNotificationSender apnsSender,
        ILogger<FirebasePushNotificationService> logger)
    {
        _context = context;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _apnsSender = apnsSender;
        _logger = logger;
    }

    private bool IsFcmConfigured =>
        !string.IsNullOrWhiteSpace(_configuration["Push:Firebase:ServerKey"]);

    public async Task SendAsync(PushNotificationPayload payload, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.DevicePushTokens
            .Where(t => t.UserId == payload.UserId && !t.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            _logger.LogDebug("No push tokens for user {UserId}", payload.UserId);
            return;
        }

        foreach (var token in tokens)
        {
            if (token.Token.StartsWith("dev-", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Skipping simulated push token for user {UserId}", payload.UserId);
                continue;
            }

            if (IsIosPlatform(token.Platform))
            {
                await _apnsSender.SendAsync(token.Token, payload, cancellationToken);
                continue;
            }

            if (IsFcmConfigured)
            {
                await SendFcmAsync(token.Token, payload, cancellationToken);
            }
            else
            {
                _logger.LogWarning(
                    "Push:Firebase:ServerKey not configured — logging Android notification for user {UserId}: {TitleEn}",
                    payload.UserId,
                    payload.TitleEn);
            }
        }
    }

    private async Task SendFcmAsync(
        string deviceToken,
        PushNotificationPayload payload,
        CancellationToken cancellationToken)
    {
        var serverKey = _configuration["Push:Firebase:ServerKey"]!;
        var client = _httpClientFactory.CreateClient(nameof(FirebasePushNotificationService));

        var body = new
        {
            to = deviceToken,
            notification = new
            {
                title = payload.TitleEn,
                body = payload.MessageEn,
            },
            data = new Dictionary<string, string>
            {
                ["type"] = payload.NotificationType,
                ["referenceId"] = payload.ReferenceId?.ToString() ?? string.Empty,
                ["titleAr"] = payload.TitleAr,
                ["messageAr"] = payload.MessageAr,
            },
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
        request.Headers.TryAddWithoutValidation("Authorization", $"key={serverKey}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");

        var response = await client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "FCM push delivered to user {UserId}: {TitleEn}",
                payload.UserId,
                payload.TitleEn);
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError(
            "FCM push failed ({StatusCode}) for user {UserId}: {Body}",
            (int)response.StatusCode,
            payload.UserId,
            responseBody);
    }

    private static bool IsIosPlatform(string platform) =>
        platform.Equals("iOS", StringComparison.OrdinalIgnoreCase) ||
        platform.Equals("ios", StringComparison.OrdinalIgnoreCase);
}
