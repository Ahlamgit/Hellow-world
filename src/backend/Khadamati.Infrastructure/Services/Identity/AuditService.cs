using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Identity;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditService> _logger;

    public AuditService(ApplicationDbContext context, ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogSecurityEventAsync(
        Guid? userId, string eventType, string description, string? ipAddress, string? userAgent,
        string severity = "Info", string? metadata = null, CancellationToken cancellationToken = default)
    {
        await _context.Set<Domain.Entities.Identity.SecurityLog>().AddAsync(new Domain.Entities.Identity.SecurityLog
        {
            UserId = userId,
            EventType = eventType,
            Description = description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Severity = severity,
            Metadata = metadata,
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Security event {EventType} for user {UserId}: {Description}", eventType, userId, description);
    }

    public async Task LogAuditAsync(
        string tableName, string action, string? entityId, Guid? userId, string? userEmail,
        string? oldValues, string? newValues, string? ipAddress, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(new AuditLog
        {
            TableName = tableName,
            Action = action,
            EntityId = entityId,
            UserId = userId?.ToString(),
            UserEmail = userEmail,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
        }, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
