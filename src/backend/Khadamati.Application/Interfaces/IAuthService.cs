using Khadamati.Application.DTOs.Auth;

namespace Khadamati.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> VerifyEmailAsync(VerifyEmailRequestDto request, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> ResendEmailVerificationAsync(ResendEmailVerificationRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<OtpSentResponseDto> SendPhoneOtpAsync(Guid userId, SendPhoneOtpRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<MessageResponseDto> VerifyPhoneOtpAsync(Guid userId, VerifyPhoneOtpRequestDto request, CancellationToken cancellationToken = default);
}

public interface ITokenService
{
    (string Token, string JwtId, DateTime ExpiresAt) GenerateAccessToken(Guid userId, string email, string role);
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

public interface ISmsService
{
    Task SendOtpAsync(string phone, string otp, string language, CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    string? IpAddress { get; }
}
