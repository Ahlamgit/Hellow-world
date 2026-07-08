using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Auth;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Khadamati.Infrastructure.Data;

namespace Khadamati.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public AuthService(
        ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        IConfiguration configuration)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            throw new ConflictException("Email is already registered.");

        if (await _context.Users.AnyAsync(u => u.Phone == request.Phone, cancellationToken))
            throw new ConflictException("Phone number is already registered.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            throw new ValidationException(["Invalid role specified."]);

        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = role,
            Status = UserStatus.Pending,
            Profile = new UserProfile
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PreferredLanguage = request.PreferredLanguage
            }
        };

        if (role == UserRole.Craftsman)
            user.CraftsmanProfile = new CraftsmanProfile();
        else if (role == UserRole.Store)
            user.StoreProfile = new StoreProfile { StoreName = $"{request.FirstName} {request.LastName}" };

        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GenerateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            throw new UnauthorizedException("Account is temporarily locked. Please try again later.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
                user.FailedLoginAttempts = 0;
            }
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (user.Status is UserStatus.Suspended or UserStatus.Banned)
            throw new UnauthorizedException("Account is not active.");

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GenerateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new UnauthorizedException("Invalid access token.");

        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti)!;

        var storedToken = await _context.RefreshTokens
            .Include(r => r.User).ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken && r.UserId == userId, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!storedToken.IsActive || storedToken.JwtId != jwtId)
            throw new UnauthorizedException("Invalid refresh token.");

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        _unitOfWork.Repository<RefreshToken>().Update(storedToken);

        return await GenerateAuthResponseAsync(storedToken.User, ipAddress, cancellationToken, storedToken.Token);
    }

    public async Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken, cancellationToken)
            ?? throw new NotFoundException("Refresh token not found.");

        if (!token.IsActive)
            throw new UnauthorizedException("Token is not active.");

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        _unitOfWork.Repository<RefreshToken>().Update(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user, string? ipAddress, CancellationToken cancellationToken, string? replacedToken = null)
    {
        var (accessToken, jwtId, expiresAt) = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            JwtId = jwtId,
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")),
            CreatedByIp = ipAddress,
            ReplacedByToken = replacedToken
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(accessToken, refreshToken.Token, expiresAt, _mapper.Map<UserDto>(user));
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
