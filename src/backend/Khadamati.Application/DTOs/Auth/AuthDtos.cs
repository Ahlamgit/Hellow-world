namespace Khadamati.Application.DTOs.Auth;

public record RegisterRequestDto(
    string Email,
    string Phone,
    string Password,
    string FirstName,
    string LastName,
    string Role,
    string PreferredLanguage = "ar");

public record LoginRequestDto(string Email, string Password);

public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string Phone,
    string Role,
    string Status,
    string VerificationStatus,
    string SubscriptionStatus,
    string FirstName,
    string LastName,
    string? ProfilePictureUrl,
    string PreferredLanguage);
