namespace Khadamati.Application.DTOs.Auth;

public record RegisterRequestDto(
    string Email,
    string Phone,
    string Password,
    string FirstName,
    string LastName,
    string Role,
    string PreferredLanguage = "ar");

public record LoginRequestDto(
    string Email,
    string Password,
    bool RememberMe = false);

public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);

public record ForgotPasswordRequestDto(string Email);

public record ResetPasswordRequestDto(string Token, string NewPassword, string ConfirmPassword);

public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword);

public record VerifyEmailRequestDto(string Token);

public record ResendEmailVerificationRequestDto(string Email);

public record SendPhoneOtpRequestDto(string Phone);

public record VerifyPhoneOtpRequestDto(string Phone, string Otp);

public record RevokeTokenRequestDto(string RefreshToken);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User,
    bool RequiresEmailVerification = false,
    bool RequiresPhoneVerification = false);

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string SubscriptionStatus { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
}

public record MessageResponseDto(string Message);

public record OtpSentResponseDto(string Message, DateTime ExpiresAt, int ExpiresInSeconds);
