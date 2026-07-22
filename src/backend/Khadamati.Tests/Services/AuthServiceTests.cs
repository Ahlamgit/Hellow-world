using AutoMapper;
using FluentAssertions;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Interfaces;
using Khadamati.Application.Mappings;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Khadamati.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;
    private readonly PasswordHasher _passwordHasher = new();
    private readonly Role _customerRole;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _customerRole = new Role { Name = RoleNames.Customer, NameAr = "عميل" };
        _context.Roles.Add(_customerRole);
        _context.SaveChanges();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "Khadamati-Super-Secret-Key-Minimum-32-Characters-Long-2024!",
                ["Jwt:Issuer"] = "https://api.khadamati.com",
                ["Jwt:Audience"] = "https://khadamati.com",
                ["Jwt:AccessTokenExpirationMinutes"] = "15",
                ["Jwt:RefreshTokenExpirationDays"] = "7",
                ["Auth:PasswordPolicy:MinLength"] = "8",
                ["Auth:PasswordPolicy:RequireSpecialChar"] = "true",
                ["Auth:MaxFailedLoginAttempts"] = "5",
                ["Auth:LockoutDurationMinutes"] = "15",
            })
            .Build();

        var unitOfWork = new UnitOfWork(_context);
        var authRepository = new AuthRepository(_context);
        var identityRepository = new IdentityRepository(_context);
        var sessionRepository = new SessionRepository(_context);
        var tokenService = new TokenService(config);
        var passwordPolicy = new PasswordPolicyService(config, identityRepository, _passwordHasher);
        var permissionService = new PermissionService(identityRepository, new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));
        var emailMock = new Mock<IEmailService>();
        emailMock.Setup(e => e.SendEmailVerificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("http://localhost:5173/verify-email?token=test");
        var smsMock = new Mock<ISmsService>();
        var auditMock = new Mock<IAuditService>();
        var mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        var logger = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _context, unitOfWork, authRepository, identityRepository, sessionRepository,
            tokenService, _passwordHasher, passwordPolicy, permissionService,
            emailMock.Object, new OtpService(), smsMock.Object, auditMock.Object,
            mapper, config, logger.Object);
    }

    [Fact]
    public async Task Register_ShouldCreateUserAndReturnTokens()
    {
        var request = new RegisterRequestDto("newuser@test.com", "+966509999999", "Password1!", "Password1!", "Test", "User", RoleNames.Customer);
        var result = await _authService.RegisterAsync(request, "127.0.0.1");

        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("newuser@test.com");
        result.User.RequiresEmailVerification.Should().BeTrue();
        result.SessionId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnTokens()
    {
        var user = new User
        {
            Email = "login@test.com",
            Phone = "+966501111111",
            PasswordHash = _passwordHasher.Hash("Password1!"),
            Role = UserRole.Customer,
            PrimaryRoleId = _customerRole.Id,
            Status = UserStatus.Active,
            Profile = new UserProfile { FirstName = "Login", LastName = "Test" }
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _context.UserRoles.Add(new UserRoleAssignment { UserId = user.Id, RoleId = _customerRole.Id, IsPrimary = true });
        await _context.SaveChangesAsync();

        var result = await _authService.LoginAsync(new LoginRequestDto("login@test.com", "Password1!", true), "127.0.0.1");

        result.AccessToken.Should().NotBeNullOrEmpty();
        result.User.PrimaryRole.Should().Be(RoleNames.Customer);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldThrow()
    {
        var user = new User
        {
            Email = "badlogin@test.com",
            Phone = "+966502222222",
            PasswordHash = _passwordHasher.Hash("Password1!"),
            Role = UserRole.Customer,
            PrimaryRoleId = _customerRole.Id,
            Status = UserStatus.Active
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var act = () => _authService.LoginAsync(new LoginRequestDto("badlogin@test.com", "WrongPass1!"), "127.0.0.1");
        await act.Should().ThrowAsync<Khadamati.Application.Common.UnauthorizedException>();
    }

    [Fact]
    public async Task ChangePassword_ShouldUpdateHash()
    {
        var user = new User
        {
            Email = "changepw@test.com",
            Phone = "+966503333333",
            PasswordHash = _passwordHasher.Hash("OldPass1!"),
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _authService.ChangePasswordAsync(user.Id, new ChangePasswordRequestDto("OldPass1!", "NewPass1!", "NewPass1!"), "127.0.0.1");

        var updated = await _context.Users.FindAsync(user.Id);
        _passwordHasher.Verify("NewPass1!", updated!.PasswordHash).Should().BeTrue();
    }

    public void Dispose() => _context.Dispose();
}
