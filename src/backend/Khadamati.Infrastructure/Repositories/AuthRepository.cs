using Khadamati.Domain.Entities;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _context;

    public AuthRepository(ApplicationDbContext context) => _context = context;

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public Task<User?> GetUserByIdWithProfileAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken = default) =>
        _context.Users.AnyAsync(u => u.Phone == phone, cancellationToken);

    public Task<EmailVerificationToken?> GetActiveEmailVerificationTokenAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.EmailVerificationTokens
            .Where(t => t.UserId == userId && t.VerifiedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<EmailVerificationToken?> GetEmailVerificationTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _context.EmailVerificationTokens
            .Include(t => t.User).ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.VerifiedAt == null, cancellationToken);

    public Task<PasswordResetToken?> GetActivePasswordResetTokenAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.PasswordResetTokens
            .Where(t => t.UserId == userId && t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<PasswordResetToken?> GetPasswordResetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _context.PasswordResetTokens
            .Include(t => t.User).ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.UsedAt == null, cancellationToken);

    public Task<PhoneOtpToken?> GetActivePhoneOtpAsync(Guid userId, string phone, CancellationToken cancellationToken = default) =>
        _context.PhoneOtpTokens
            .Where(t => t.UserId == userId && t.Phone == phone && t.VerifiedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task RevokeAllRefreshTokensAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
        }
    }

    public Task<int> CountRecentOtpRequestsAsync(Guid userId, TimeSpan window, CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow.Subtract(window);
        return _context.PhoneOtpTokens.CountAsync(t => t.UserId == userId && t.CreatedAt >= since, cancellationToken);
    }
}
