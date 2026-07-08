namespace Khadamati.Domain.Interfaces;

public interface IAuthRepository
{
    Task<Entities.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Entities.User?> GetUserByIdWithProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken = default);
    Task<Entities.EmailVerificationToken?> GetActiveEmailVerificationTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Entities.EmailVerificationToken?> GetEmailVerificationTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<Entities.PasswordResetToken?> GetActivePasswordResetTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Entities.PasswordResetToken?> GetPasswordResetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<Entities.PhoneOtpToken?> GetActivePhoneOtpAsync(Guid userId, string phone, CancellationToken cancellationToken = default);
    Task RevokeAllRefreshTokensAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default);
    Task<int> CountRecentOtpRequestsAsync(Guid userId, TimeSpan window, CancellationToken cancellationToken = default);
}
