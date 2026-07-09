using Khadamati.Application.DTOs.Messaging;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class DeviceTokenService : IDeviceTokenService
{
    private readonly ApplicationDbContext _context;

    public DeviceTokenService(ApplicationDbContext context) => _context = context;

    public async Task RegisterAsync(Guid userId, RegisterPushTokenDto request, CancellationToken cancellationToken = default)
    {
        var existing = await _context.DevicePushTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == request.Token, cancellationToken);

        if (existing != null)
        {
            existing.Platform = request.Platform;
            existing.DeviceName = request.DeviceName;
            existing.LastUsedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.IsDeleted = false;
            existing.DeletedAt = null;
        }
        else
        {
            await _context.DevicePushTokens.AddAsync(new DevicePushToken
            {
                UserId = userId,
                Token = request.Token,
                Platform = request.Platform,
                DeviceName = request.DeviceName,
                LastUsedAt = DateTime.UtcNow,
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UnregisterAsync(Guid userId, string token, CancellationToken cancellationToken = default)
    {
        var existing = await _context.DevicePushTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token, cancellationToken);

        if (existing == null) return;

        existing.IsDeleted = true;
        existing.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
