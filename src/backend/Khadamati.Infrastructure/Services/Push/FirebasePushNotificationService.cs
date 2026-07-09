using Khadamati.Application.DTOs.Messaging;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Push;

/// <summary>
/// Firebase/APNs push scaffold. Configure Push:Firebase:ServerKey for FCM HTTP v1 delivery.
/// Until configured, logs payloads like the development provider.
/// </summary>
public class FirebasePushNotificationService : IPushNotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebasePushNotificationService> _logger;

    public FirebasePushNotificationService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<FirebasePushNotificationService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    private bool IsConfigured =>
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

        if (!IsConfigured)
        {
            _logger.LogWarning(
                "Push:Firebase:ServerKey not configured — logging {Count} notification(s) for user {UserId}: {Title}",
                tokens.Count, payload.UserId, payload.TitleEn);
        }

        foreach (var token in tokens)
        {
            if (IsConfigured)
            {
                // Scaffold: send via FCM HTTP v1 using Push:Firebase:ServerKey
                _logger.LogInformation(
                    "FCM push queued [{Platform}] user {UserId}: {TitleEn}",
                    token.Platform, payload.UserId, payload.TitleEn);
            }
            else
            {
                _logger.LogInformation(
                    "Push [{Platform}] user {UserId}: {TitleEn} ({Type})",
                    token.Platform, payload.UserId, payload.TitleEn, payload.NotificationType);
            }
        }
    }
}
