using Khadamati.Application.DTOs.Identity;

namespace Khadamati.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> VerifyEmailAsync(VerifyEmailRequestDto request, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ResendEmailVerificationAsync(ResendEmailVerificationRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> AdminVerifyEmailAsync(Guid adminUserId, AdminVerifyEmailRequestDto request, CancellationToken cancellationToken = default);
    Task<OtpSentResponseDto> SendPhoneOtpAsync(Guid userId, SendPhoneOtpRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> VerifyPhoneOtpAsync(Guid userId, VerifyPhoneOtpRequestDto request, CancellationToken cancellationToken = default);
}

public interface ISessionService
{
    Task<IReadOnlyList<SessionDto>> GetActiveSessionsAsync(Guid userId, Guid? currentSessionId, CancellationToken cancellationToken = default);
    Task RevokeSessionAsync(Guid userId, Guid sessionId, string? ipAddress, bool isAdmin, CancellationToken cancellationToken = default);
    Task RevokeAllOtherSessionsAsync(Guid userId, Guid currentSessionId, string? ipAddress, CancellationToken cancellationToken = default);
    Task RevokeAllSessionsAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default);
}

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default);
}

public interface ILoginHistoryService
{
    Task<PagedLoginHistoryDto> GetUserLoginHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
}

public interface IPermissionService
{
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default);
}

public interface IPasswordPolicyService
{
    void ValidatePassword(string password);
    Task ValidatePasswordNotReusedAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default);
}

public interface ITokenService
{
    (string Token, string JwtId, DateTime ExpiresAt) GenerateAccessToken(
        Guid userId, string email, IReadOnlyList<string> roles, IReadOnlyList<string> permissions, bool emailVerified);
    string GenerateRefreshToken();
    string GenerateSecureToken();
    string HashToken(string token);
    bool VerifyToken(string token, string hash);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IEmailProvider
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string firstName, string verificationToken, string language, CancellationToken cancellationToken = default);
    Task SendPasswordResetAsync(string email, string firstName, string resetToken, string language, CancellationToken cancellationToken = default);
}

public interface IOtpService
{
    string GenerateOtp();
    string HashOtp(string otp);
    bool VerifyOtp(string otp, string hash);
}

public interface ISmsProvider
{
    Task SendAsync(string phone, string message, CancellationToken cancellationToken = default);
}

public interface ISmsService
{
    Task SendOtpAsync(string phone, string otp, string language, CancellationToken cancellationToken = default);
}

public interface IAuditService
{
    Task LogSecurityEventAsync(Guid? userId, string eventType, string description, string? ipAddress, string? userAgent, string severity = "Info", string? metadata = null, CancellationToken cancellationToken = default);
    Task LogAuditAsync(string tableName, string action, string? entityId, Guid? userId, string? userEmail, string? oldValues, string? newValues, string? ipAddress, CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? PrimaryRole { get; }
    string? Role { get; }
    Guid? SessionId { get; }
    IReadOnlyList<string> Permissions { get; }
    bool EmailVerified { get; }
    string? IpAddress { get; }
}
