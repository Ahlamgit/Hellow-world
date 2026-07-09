using Khadamati.Application.DTOs.Messaging;

namespace Khadamati.Application.Interfaces;

public interface IDeviceTokenService
{
    Task RegisterAsync(Guid userId, RegisterPushTokenDto request, CancellationToken cancellationToken = default);
    Task UnregisterAsync(Guid userId, string token, CancellationToken cancellationToken = default);
}
