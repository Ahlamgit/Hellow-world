using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Infrastructure.Services.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration) => _configuration = configuration;

    public (string Token, string JwtId, DateTime ExpiresAt) GenerateAccessToken(
        Guid userId, string email, IReadOnlyList<string> roles, IReadOnlyList<string> permissions, bool emailVerified)
    {
        var jwtId = Guid.NewGuid().ToString();
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
            key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue("Jwt:AccessTokenExpirationMinutes", 15));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, jwtId),
            new("email_verified", emailVerified.ToString().ToLowerInvariant()),
        };

        foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var permission in permissions.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim("permission", permission));

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token), jwtId, expiresAt);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToHexString(randomBytes);
    }

    public string HashToken(string token) => BCrypt.Net.BCrypt.HashPassword(token, workFactor: 12);
    public bool VerifyToken(string token, string hash) => BCrypt.Net.BCrypt.Verify(token, hash);
}

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}

public class OtpService : IOtpService
{
    public string GenerateOtp() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    public string HashOtp(string otp) => BCrypt.Net.BCrypt.HashPassword(otp, workFactor: 12);
    public bool VerifyOtp(string otp, string hash) => BCrypt.Net.BCrypt.Verify(otp, hash);
}
