using FluentAssertions;
using Khadamati.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Tests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ShouldProduceDifferentHashEachTime()
    {
        var hash1 = _hasher.Hash("Password1!");
        var hash2 = _hasher.Hash("Password1!");
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        var password = "SecurePass1!";
        var hash = _hasher.Hash(password);
        _hasher.Verify(password, hash).Should().BeTrue();
    }

    [Fact]
    public void Verify_WithWrongPassword_ShouldReturnFalse()
    {
        var hash = _hasher.Hash("CorrectPass1!");
        _hasher.Verify("WrongPass1!", hash).Should().BeFalse();
    }
}

public class OtpServiceTests
{
    private readonly OtpService _otpService = new();

    [Fact]
    public void GenerateOtp_ShouldReturnSixDigits()
    {
        var otp = _otpService.GenerateOtp();
        otp.Should().HaveLength(6);
        otp.Should().MatchRegex(@"^\d{6}$");
    }

    [Fact]
    public void VerifyOtp_WithCorrectCode_ShouldReturnTrue()
    {
        var otp = "123456";
        var hash = _otpService.HashOtp(otp);
        _otpService.VerifyOtp(otp, hash).Should().BeTrue();
    }
}

public class TokenServiceTests
{
    [Fact]
    public void HashAndVerifyToken_ShouldWork()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "Khadamati-Super-Secret-Key-Minimum-32-Characters-Long-2024!",
                ["Jwt:Issuer"] = "https://api.khadamati.com",
                ["Jwt:Audience"] = "https://khadamati.com",
                ["Jwt:AccessTokenExpirationMinutes"] = "15"
            })
            .Build();

        var tokenService = new TokenService(config);
        var raw = tokenService.GenerateSecureToken();
        var hash = tokenService.HashToken(raw);

        tokenService.VerifyToken(raw, hash).Should().BeTrue();
        tokenService.VerifyToken("wrong-token", hash).Should().BeFalse();
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeClaims()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "Khadamati-Super-Secret-Key-Minimum-32-Characters-Long-2024!",
                ["Jwt:Issuer"] = "https://api.khadamati.com",
                ["Jwt:Audience"] = "https://khadamati.com",
                ["Jwt:AccessTokenExpirationMinutes"] = "15"
            })
            .Build();

        var tokenService = new TokenService(config);
        var userId = Guid.NewGuid();
        var (token, jwtId, expiresAt) = tokenService.GenerateAccessToken(userId, "test@khadamati.com", "Customer");

        token.Should().NotBeNullOrEmpty();
        jwtId.Should().NotBeNullOrEmpty();
        expiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}
