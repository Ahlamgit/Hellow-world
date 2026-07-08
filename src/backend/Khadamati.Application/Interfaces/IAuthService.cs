using Khadamati.Application.DTOs.Auth;

namespace Khadamati.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
}

public interface ITokenService
{
    (string Token, string JwtId, DateTime ExpiresAt) GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    string? IpAddress { get; }
}
