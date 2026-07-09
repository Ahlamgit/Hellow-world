using Khadamati.Application.DTOs.Messaging;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Push;

public class DevelopmentPushNotificationService : IPushNotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevelopmentPushNotificationService> _logger;

    public DevelopmentPushNotificationService(ApplicationDbContext context, ILogger<DevelopmentPushNotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendAsync(PushNotificationPayload payload, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.DevicePushTokens
            .Where(t => t.UserId == payload.UserId && !t.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            _logger.LogInformation(
                "Push [{Platform}] to user {UserId} token {TokenPrefix}…: {TitleEn} ({Type}) ref={ReferenceId}",
                token.Platform,
                payload.UserId,
                token.Token.Length > 8 ? token.Token[..8] : token.Token,
                payload.TitleEn,
                payload.NotificationType,
                payload.ReferenceId);
        }

        if (tokens.Count == 0)
        {
            _logger.LogDebug("No push tokens registered for user {UserId}; in-app notification only.", payload.UserId);
        }
    }
}
