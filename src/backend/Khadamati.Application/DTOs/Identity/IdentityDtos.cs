namespace Khadamati.Application.DTOs.Identity;

public record DeviceInfoDto(
    string? DeviceId,
    string? DeviceName,
    string? Platform,
    string? Browser,
    string? UserAgent);

public record RegisterRequestDto(
    string Email,
    string Phone,
    string Password,
    string FirstName,
    string LastName,
    string Role,
    string PreferredLanguage = "ar",
    DeviceInfoDto? Device = null);

public record LoginRequestDto(
    string Email,
    string Password,
    bool RememberMe = false,
    DeviceInfoDto? Device = null);

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

public record AdminVerifyEmailRequestDto(Guid UserId);

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PrimaryRole { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string SubscriptionStatus { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public string Timezone { get; set; } = "Asia/Riyadh";
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool RequiresEmailVerification { get; set; }
    public bool RequiresPhoneVerification { get; set; }

    /// <summary>Legacy single role for backward compatibility.</summary>
    public string Role => PrimaryRole;
}

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid SessionId,
    UserDto User);

public record MessageResponseDto(string Message);

public record OtpSentResponseDto(string Message, DateTime ExpiresAt, int ExpiresInSeconds);

public class SessionDto
{
    public Guid Id { get; set; }
    public string? DeviceName { get; set; }
    public string? Platform { get; set; }
    public string? Browser { get; set; }
    public string? IpAddress { get; set; }
    public bool RememberMe { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsCurrent { get; set; }
}

public class LoginHistoryDto
{
    public long Id { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceName { get; set; }
    public string? Platform { get; set; }
    public string? Browser { get; set; }
    public DateTime LoginAt { get; set; }
}

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Nationality { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? AddressLine { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public string Timezone { get; set; } = "Asia/Riyadh";
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
}

public record UpdateProfileRequestDto(
    string FirstName,
    string LastName,
    string? Gender,
    DateTime? BirthDate,
    string? Nationality,
    string? ProfilePictureUrl,
    string? AddressLine,
    string? Country,
    string? City,
    string? Region,
    decimal? Latitude,
    decimal? Longitude,
    string PreferredLanguage,
    string Timezone);

public class PagedLoginHistoryDto
{
    public IReadOnlyList<LoginHistoryDto> Items { get; set; } = Array.Empty<LoginHistoryDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
