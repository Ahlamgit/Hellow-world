using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Auth;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public AuthService(
        ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IAuthRepository authRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IOtpService otpService,
        ISmsService smsService,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authRepository = authRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _otpService = otpService;
        _smsService = smsService;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (await _authRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new ConflictException("Email is already registered.");

        if (await _authRepository.PhoneExistsAsync(request.Phone, cancellationToken))
            throw new ConflictException("Phone number is already registered.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role) || role == UserRole.Administrator)
            throw new Khadamati.Application.Common.ValidationException(["Invalid role specified. Administrator accounts cannot be self-registered."]);

        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = role,
            Status = UserStatus.Pending,
            VerificationStatus = VerificationStatus.Unverified,
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

        await CreateAndSendEmailVerificationAsync(user, cancellationToken);

        return await GenerateAuthResponseAsync(user, ipAddress, rememberMe: false, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken)
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

        return await GenerateAuthResponseAsync(user, ipAddress, request.RememberMe, cancellationToken);
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

        return await GenerateAuthResponseAsync(storedToken.User, ipAddress, storedToken.RememberMe, cancellationToken, storedToken.Token);
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

    public async Task<MessageResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return new MessageResponseDto("If the email exists, a password reset link has been sent.");

        var existing = await _authRepository.GetActivePasswordResetTokenAsync(user.Id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Repository<PasswordResetToken>().Update(existing);
        }

        var rawToken = _tokenService.GenerateSecureToken();
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddHours(GetConfigInt("Auth:PasswordResetExpirationHours", 1)),
            RequestedFromIp = ipAddress
        };

        await _unitOfWork.Repository<PasswordResetToken>().AddAsync(resetToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var language = user.Profile?.PreferredLanguage ?? "ar";
        await _emailService.SendPasswordResetAsync(user.Email, user.Profile?.FirstName ?? user.Email, rawToken, language, cancellationToken);

        _logger.LogInformation("Password reset requested for user {UserId}", user.Id);
        return new MessageResponseDto("If the email exists, a password reset link has been sent.");
    }

    public async Task<MessageResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenService.HashToken(request.Token);
        var storedToken = await FindPasswordResetTokenAsync(request.Token, cancellationToken)
            ?? throw new UnauthorizedException("Invalid or expired reset token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Invalid or expired reset token.");

        var user = storedToken.User;
        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.UpdatedAt = DateTime.UtcNow;

        storedToken.UsedAt = DateTime.UtcNow;
        _unitOfWork.Repository<User>().Update(user);
        _unitOfWork.Repository<PasswordResetToken>().Update(storedToken);
        await _authRepository.RevokeAllRefreshTokensAsync(user.Id, ipAddress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MessageResponseDto("Password has been reset successfully.");
    }

    public async Task<MessageResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.GetUserByIdWithProfileAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<User>().Update(user);
        await _authRepository.RevokeAllRefreshTokensAsync(userId, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MessageResponseDto("Password changed successfully. Please log in again.");
    }

    public async Task<MessageResponseDto> VerifyEmailAsync(VerifyEmailRequestDto request, CancellationToken cancellationToken = default)
    {
        var storedToken = await FindEmailVerificationTokenAsync(request.Token, cancellationToken)
            ?? throw new UnauthorizedException("Invalid or expired verification token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Invalid or expired verification token.");

        var user = storedToken.User;
        user.EmailVerifiedAt = DateTime.UtcNow;
        user.VerificationStatus = VerificationStatus.Verified;
        if (user.Status == UserStatus.Pending)
            user.Status = UserStatus.Active;

        storedToken.VerifiedAt = DateTime.UtcNow;
        _unitOfWork.Repository<User>().Update(user);
        _unitOfWork.Repository<EmailVerificationToken>().Update(storedToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MessageResponseDto("Email verified successfully.");
    }

    public async Task<MessageResponseDto> ResendEmailVerificationAsync(ResendEmailVerificationRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null || user.IsEmailVerified)
            return new MessageResponseDto("If the email exists and is unverified, a verification email has been sent.");

        await CreateAndSendEmailVerificationAsync(user, cancellationToken);
        return new MessageResponseDto("If the email exists and is unverified, a verification email has been sent.");
    }

    public async Task<OtpSentResponseDto> SendPhoneOtpAsync(Guid userId, SendPhoneOtpRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.GetUserByIdWithProfileAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (user.Phone != request.Phone)
            throw new Khadamati.Application.Common.ValidationException(["Phone number does not match your account."]);

        var maxRequests = GetConfigInt("Auth:MaxOtpRequestsPerHour", 5);
        var recentCount = await _authRepository.CountRecentOtpRequestsAsync(userId, TimeSpan.FromHours(1), cancellationToken);
        if (recentCount >= maxRequests)
            throw new Khadamati.Application.Common.ValidationException(["Too many OTP requests. Please try again later."]);

        var existing = await _authRepository.GetActivePhoneOtpAsync(userId, request.Phone, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Repository<PhoneOtpToken>().Update(existing);
        }

        var otp = _otpService.GenerateOtp();
        var expiresMinutes = GetConfigInt("Auth:OtpExpirationMinutes", 10);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var otpToken = new PhoneOtpToken
        {
            UserId = userId,
            Phone = request.Phone,
            OtpHash = _otpService.HashOtp(otp),
            ExpiresAt = expiresAt,
            RequestedFromIp = ipAddress
        };

        await _unitOfWork.Repository<PhoneOtpToken>().AddAsync(otpToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var language = user.Profile?.PreferredLanguage ?? "ar";
        await _smsService.SendOtpAsync(request.Phone, otp, language, cancellationToken);

        return new OtpSentResponseDto("OTP sent successfully.", expiresAt, expiresMinutes * 60);
    }

    public async Task<MessageResponseDto> VerifyPhoneOtpAsync(Guid userId, VerifyPhoneOtpRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.GetUserByIdWithProfileAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var otpToken = await _authRepository.GetActivePhoneOtpAsync(userId, request.Phone, cancellationToken)
            ?? throw new UnauthorizedException("Invalid or expired OTP.");

        var maxAttempts = GetConfigInt("Auth:MaxOtpAttempts", 5);
        if (otpToken.AttemptCount >= maxAttempts)
            throw new UnauthorizedException("Maximum OTP attempts exceeded. Please request a new code.");

        if (!_otpService.VerifyOtp(request.Otp, otpToken.OtpHash))
        {
            otpToken.AttemptCount++;
            _unitOfWork.Repository<PhoneOtpToken>().Update(otpToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Invalid OTP.");
        }

        otpToken.VerifiedAt = DateTime.UtcNow;
        user.PhoneVerifiedAt = DateTime.UtcNow;
        _unitOfWork.Repository<PhoneOtpToken>().Update(otpToken);
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MessageResponseDto("Phone number verified successfully.");
    }

    private async Task CreateAndSendEmailVerificationAsync(User user, CancellationToken cancellationToken)
    {
        var existing = await _authRepository.GetActiveEmailVerificationTokenAsync(user.Id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Repository<EmailVerificationToken>().Update(existing);
        }

        var rawToken = _tokenService.GenerateSecureToken();
        var emailToken = new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddHours(GetConfigInt("Auth:EmailVerificationExpirationHours", 24))
        };

        await _unitOfWork.Repository<EmailVerificationToken>().AddAsync(emailToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var language = user.Profile?.PreferredLanguage ?? "ar";
        await _emailService.SendEmailVerificationAsync(user.Email, user.Profile?.FirstName ?? user.Email, rawToken, language, cancellationToken);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(
        User user, string? ipAddress, bool rememberMe, CancellationToken cancellationToken, string? replacedToken = null)
    {
        if (user.Profile == null)
            user = await _authRepository.GetUserByIdWithProfileAsync(user.Id, cancellationToken) ?? user;

        var (accessToken, jwtId, expiresAt) = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshDays = rememberMe
            ? GetConfigInt("Jwt:RememberMeRefreshTokenExpirationDays", 30)
            : GetConfigInt("Jwt:RefreshTokenExpirationDays", 7);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            JwtId = jwtId,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays),
            CreatedByIp = ipAddress,
            ReplacedByToken = replacedToken,
            RememberMe = rememberMe
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        return new AuthResponseDto(
            accessToken,
            refreshToken.Token,
            expiresAt,
            userDto,
            !user.IsEmailVerified,
            !user.IsPhoneVerified);
    }

    private async Task<EmailVerificationToken?> FindEmailVerificationTokenAsync(string rawToken, CancellationToken cancellationToken)
    {
        var tokens = await _context.EmailVerificationTokens
            .Include(t => t.User).ThenInclude(u => u.Profile)
            .Where(t => t.VerifiedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        return tokens.FirstOrDefault(t => _tokenService.VerifyToken(rawToken, t.TokenHash));
    }

    private async Task<PasswordResetToken?> FindPasswordResetTokenAsync(string rawToken, CancellationToken cancellationToken)
    {
        var tokens = await _context.PasswordResetTokens
            .Include(t => t.User).ThenInclude(u => u.Profile)
            .Where(t => t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        return tokens.FirstOrDefault(t => _tokenService.VerifyToken(rawToken, t.TokenHash));
    }

    private int GetConfigInt(string key, int defaultValue) =>
        int.TryParse(_configuration[key], out var value) ? value : defaultValue;

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
