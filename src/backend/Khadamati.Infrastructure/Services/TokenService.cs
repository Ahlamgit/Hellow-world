using System.Security.Cryptography;
using System.Text;
using Khadamati.Application.Interfaces;

namespace Khadamati.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public TokenService(Microsoft.Extensions.Configuration.IConfiguration configuration) => _configuration = configuration;

    public (string Token, string JwtId, DateTime ExpiresAt) GenerateAccessToken(Guid userId, string email, string role)
    {
        var jwtId = Guid.NewGuid().ToString();
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15"));

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, userId.ToString()),
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role),
            new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, jwtId)
        };

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
    public string GenerateOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }

    public string HashOtp(string otp) => BCrypt.Net.BCrypt.HashPassword(otp, workFactor: 12);
    public bool VerifyOtp(string otp, string hash) => BCrypt.Net.BCrypt.Verify(otp, hash);
}
