using Khadamati.Domain.Entities;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context) => _context = context;

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        _context.RefreshTokens
            .Include(r => r.User).ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

    public Task<IReadOnlyList<RefreshToken>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(r => r.LastActivityAt ?? r.CreatedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<RefreshToken>)t.Result, cancellationToken);

    public Task<RefreshToken?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        _context.RefreshTokens.FirstOrDefaultAsync(r => r.Id == sessionId, cancellationToken);

    public async Task RevokeSessionAsync(Guid sessionId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var session = await GetByIdAsync(sessionId, cancellationToken);
        if (session == null || !session.IsActive) return;
        session.RevokedAt = DateTime.UtcNow;
        session.LogoutAt = DateTime.UtcNow;
        session.RevokedByIp = ipAddress;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllExceptAsync(Guid userId, Guid exceptSessionId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.RefreshTokens
            .Where(r => r.UserId == userId && r.Id != exceptSessionId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var s in sessions)
        {
            s.RevokedAt = DateTime.UtcNow;
            s.LogoutAt = DateTime.UtcNow;
            s.RevokedByIp = ipAddress;
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var s in sessions)
        {
            s.RevokedAt = DateTime.UtcNow;
            s.LogoutAt = DateTime.UtcNow;
            s.RevokedByIp = ipAddress;
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateLastActivityAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await GetByIdAsync(sessionId, cancellationToken);
        if (session == null) return;
        session.LastActivityAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
