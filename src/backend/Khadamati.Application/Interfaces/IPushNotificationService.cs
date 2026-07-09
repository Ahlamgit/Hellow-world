using Khadamati.Application.DTOs.Messaging;

namespace Khadamati.Application.Interfaces;

public interface IPushNotificationService
{
    Task SendAsync(PushNotificationPayload payload, CancellationToken cancellationToken = default);
}
