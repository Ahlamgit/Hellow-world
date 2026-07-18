using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Identity;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthRepository _authRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicyService _passwordPolicy;
    private readonly IPermissionService _permissionService;
    private readonly IEmailService _emailService;
    private readonly IOtpService _otpService;
    private readonly ISmsService _smsService;
    private readonly IAuditService _auditService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IAuthRepository authRepository,
        IIdentityRepository identityRepository,
        ISessionRepository sessionRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IPasswordPolicyService passwordPolicy,
        IPermissionService permissionService,
        IEmailService emailService,
        IOtpService otpService,
        ISmsService smsService,
        IAuditService auditService,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authRepository = authRepository;
        _identityRepository = identityRepository;
        _sessionRepository = sessionRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
        _permissionService = permissionService;
        _emailService = emailService;
        _otpService = otpService;
        _smsService = smsService;
        _auditService = auditService;
        _mapper = mapper;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        _passwordPolicy.ValidatePassword(request.Password);

        if (await _authRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new ConflictException("Email is already registered.");
        if (await _authRepository.PhoneExistsAsync(request.Phone, cancellationToken))
            throw new ConflictException("Phone number is already registered.");

        var roleName = RoleNames.MapLegacyRole(request.Role);
        if (!RoleNames.SelfRegistrationRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
            throw new ValidationException(["Invalid role for self-registration."]);

        var role = await _identityRepository.GetRoleByNameAsync(roleName, cancellationToken)
            ?? throw new ValidationException(["Role not found."]);

        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = MapToLegacyEnum(roleName),
            PrimaryRoleId = role.Id,
            Status = UserStatus.Pending,
            VerificationStatus = VerificationStatus.Unverified,
            Profile = new UserProfile
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PreferredLanguage = request.PreferredLanguage,
            }
        };

        if (roleName == RoleNames.Craftsman) user.CraftsmanProfile = new CraftsmanProfile();
        else if (roleName == RoleNames.StoreOwner)
            user.StoreProfile = new StoreProfile { StoreName = $"{request.FirstName} {request.LastName}" };

        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _identityRepository.AssignRoleAsync(user.Id, role.Id, isPrimary: true, assignedBy: user.Id.ToString(), cancellationToken);
        await _identityRepository.AddPasswordHistoryAsync(new PasswordHistory
        {
            UserId = user.Id,
            PasswordHash = user.PasswordHash,
            ChangeReason = "Registration",
            ChangedByIp = ipAddress,
        }, cancellationToken);

        var verificationLink = await CreateAndSendEmailVerificationAsync(user, cancellationToken);
        await _auditService.LogSecurityEventAsync(user.Id, "UserRegistered", $"User registered as {roleName}", ipAddress, request.Device?.UserAgent, cancellationToken: cancellationToken);

        var response = await GenerateAuthResponseAsync(user, ipAddress, rememberMe: false, request.Device, cancellationToken);
        return response with { EmailVerificationLink = ShouldExposeAuthLinks() ? verificationLink : null };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _identityRepository.GetUserByEmailWithRolesAsync(request.Email, cancellationToken);
        var device = request.Device;

        if (user == null)
        {
            await RecordLoginAsync(null, request.Email, false, "Invalid credentials", ipAddress, device, null, cancellationToken);
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            await RecordLoginAsync(user.Id, request.Email, false, "Account locked", ipAddress, device, null, cancellationToken);
            throw new UnauthorizedException("Account is temporarily locked. Please try again later.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            var maxAttempts = _configuration.GetValue("Auth:MaxFailedLoginAttempts", 5);
            if (user.FailedLoginAttempts >= maxAttempts)
            {
                var lockoutMinutes = _configuration.GetValue("Auth:LockoutDurationMinutes", 15);
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                user.FailedLoginAttempts = 0;
                await _auditService.LogSecurityEventAsync(user.Id, "AccountLocked", $"Locked after {maxAttempts} failed attempts", ipAddress, device?.UserAgent, "Warning", cancellationToken: cancellationToken);
            }
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await RecordLoginAsync(user.Id, request.Email, false, "Invalid password", ipAddress, device, null, cancellationToken);
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (user.Status is UserStatus.Suspended or UserStatus.Banned)
        {
            await RecordLoginAsync(user.Id, request.Email, false, $"Account {user.Status}", ipAddress, device, null, cancellationToken);
            throw new UnauthorizedException("Account is not active.");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await GenerateAuthResponseAsync(user, ipAddress, request.RememberMe, device, cancellationToken);
        await RecordLoginAsync(user.Id, request.Email, true, null, ipAddress, device, response.SessionId, cancellationToken);
        await _auditService.LogSecurityEventAsync(user.Id, "LoginSuccess", "User logged in", ipAddress, device?.UserAgent, cancellationToken: cancellationToken);

        return response;
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new UnauthorizedException("Invalid access token.");

        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti)!;

        var storedToken = await _sessionRepository.GetByTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!storedToken.IsActive || storedToken.JwtId != jwtId || storedToken.UserId != userId)
            throw new UnauthorizedException("Invalid refresh token.");

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        _unitOfWork.Repository<RefreshToken>().Update(storedToken);

        var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        return await GenerateAuthResponseAsync(user, ipAddress, storedToken.RememberMe, new DeviceInfoDto(
            storedToken.DeviceId, storedToken.DeviceName, storedToken.Platform, storedToken.Browser, storedToken.UserAgent),
            cancellationToken, storedToken.Token);
    }

    public async Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _sessionRepository.GetByTokenAsync(refreshToken, cancellationToken)
            ?? throw new NotFoundException("Refresh token not found.");

        if (!token.IsActive)
            throw new UnauthorizedException("Token is not active.");

        await _sessionRepository.RevokeSessionAsync(token.Id, ipAddress, cancellationToken);
        await _auditService.LogSecurityEventAsync(token.UserId, "Logout", "User logged out current device", ipAddress, token.UserAgent, cancellationToken: cancellationToken);
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
        await _unitOfWork.Repository<PasswordResetToken>().AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddHours(_configuration.GetValue("Auth:PasswordResetExpirationHours", 1)),
            RequestedFromIp = ipAddress
        }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var language = user.Profile?.PreferredLanguage ?? "ar";
        var resetLink = await _emailService.SendPasswordResetAsync(user.Email, user.Profile?.FirstName ?? user.Email, rawToken, language, cancellationToken);
        await _auditService.LogSecurityEventAsync(user.Id, "PasswordResetRequested", "Password reset requested", ipAddress, null, cancellationToken: cancellationToken);

        return new MessageResponseDto(
            "If the email exists, a password reset link has been sent.",
            ShouldExposeAuthLinks() ? resetLink : null);
    }

    public async Task<MessageResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        _passwordPolicy.ValidatePassword(request.NewPassword);
        var storedToken = await FindPasswordResetTokenAsync(request.Token, cancellationToken)
            ?? throw new UnauthorizedException("Invalid or expired reset token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Invalid or expired reset token.");

        var user = storedToken.User;
        await _passwordPolicy.ValidatePasswordNotReusedAsync(user.Id, request.NewPassword, cancellationToken);

        var oldHash = user.PasswordHash;
        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.PasswordChangedAt = DateTime.UtcNow;
        storedToken.UsedAt = DateTime.UtcNow;

        _unitOfWork.Repository<User>().Update(user);
        _unitOfWork.Repository<PasswordResetToken>().Update(storedToken);
        await _identityRepository.AddPasswordHistoryAsync(new PasswordHistory
        {
            UserId = user.Id, PasswordHash = user.PasswordHash, ChangeReason = "Reset", ChangedByIp = ipAddress
        }, cancellationToken);
        await _sessionRepository.RevokeAllAsync(user.Id, ipAddress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(user.Id, "PasswordReset", "Password reset completed", ipAddress, null, cancellationToken: cancellationToken);
        await _auditService.LogAuditAsync("Users", "PasswordReset", user.Id.ToString(), user.Id, user.Email, null, null, ipAddress, cancellationToken);

        return new MessageResponseDto("Password has been reset successfully.");
    }

    public async Task<MessageResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        _passwordPolicy.ValidatePassword(request.NewPassword);
        var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is incorrect.");

        await _passwordPolicy.ValidatePasswordNotReusedAsync(userId, request.NewPassword, cancellationToken);

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.PasswordChangedAt = DateTime.UtcNow;
        _unitOfWork.Repository<User>().Update(user);
        await _identityRepository.AddPasswordHistoryAsync(new PasswordHistory
        {
            UserId = userId, PasswordHash = user.PasswordHash, ChangeReason = "Change", ChangedByIp = ipAddress
        }, cancellationToken);
        await _sessionRepository.RevokeAllAsync(userId, ipAddress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(userId, "PasswordChanged", "Password changed by user", ipAddress, null, cancellationToken: cancellationToken);

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
        if (user.Status == UserStatus.Pending) user.Status = UserStatus.Active;
        storedToken.VerifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<User>().Update(user);
        _unitOfWork.Repository<EmailVerificationToken>().Update(storedToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(user.Id, "EmailVerified", "Email verified", null, null, cancellationToken: cancellationToken);

        return new MessageResponseDto("Email verified successfully.");
    }

    public async Task<MessageResponseDto> AdminVerifyEmailAsync(Guid adminUserId, AdminVerifyEmailRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.UsersVerifyEmail, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        var user = await _identityRepository.GetUserByIdWithRolesAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.EmailVerifiedAt = DateTime.UtcNow;
        user.VerificationStatus = VerificationStatus.Verified;
        if (user.Status == UserStatus.Pending) user.Status = UserStatus.Active;
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(adminUserId, "AdminEmailVerified", $"Admin verified email for user {request.UserId}", null, null, cancellationToken: cancellationToken);

        return new MessageResponseDto("Email verified by administrator.");
    }

    public async Task<MessageResponseDto> ResendEmailVerificationAsync(ResendEmailVerificationRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null || user.IsEmailVerified)
            return new MessageResponseDto("If the email exists and is unverified, a verification email has been sent.");

        var verificationLink = await CreateAndSendEmailVerificationAsync(user, cancellationToken);
        return new MessageResponseDto(
            "If the email exists and is unverified, a verification email has been sent.",
            ShouldExposeAuthLinks() ? verificationLink : null);
    }

    public async Task<OtpSentResponseDto> SendPhoneOtpAsync(Guid userId, SendPhoneOtpRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (user.Phone != request.Phone)
            throw new ValidationException(["Phone number does not match your account."]);

        var maxRequests = _configuration.GetValue("Auth:MaxOtpRequestsPerHour", 5);
        if (await _authRepository.CountRecentOtpRequestsAsync(userId, TimeSpan.FromHours(1), cancellationToken) >= maxRequests)
            throw new ValidationException(["Too many OTP requests. Please try again later."]);

        var existing = await _authRepository.GetActivePhoneOtpAsync(userId, request.Phone, cancellationToken);
        if (existing != null) { existing.IsDeleted = true; existing.DeletedAt = DateTime.UtcNow; _unitOfWork.Repository<PhoneOtpToken>().Update(existing); }

        var otp = _otpService.GenerateOtp();
        var expiresMinutes = _configuration.GetValue("Auth:OtpExpirationMinutes", 10);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        await _unitOfWork.Repository<PhoneOtpToken>().AddAsync(new PhoneOtpToken
        {
            UserId = userId, Phone = request.Phone, OtpHash = _otpService.HashOtp(otp),
            ExpiresAt = expiresAt, RequestedFromIp = ipAddress
        }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _smsService.SendOtpAsync(request.Phone, otp, user.Profile?.PreferredLanguage ?? "ar", cancellationToken);
        return new OtpSentResponseDto("OTP sent successfully.", expiresAt, expiresMinutes * 60);
    }

    public async Task<MessageResponseDto> VerifyPhoneOtpAsync(Guid userId, VerifyPhoneOtpRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var otpToken = await _authRepository.GetActivePhoneOtpAsync(userId, request.Phone, cancellationToken)
            ?? throw new UnauthorizedException("Invalid or expired OTP.");

        var maxAttempts = _configuration.GetValue("Auth:MaxOtpAttempts", 5);
        if (otpToken.AttemptCount >= maxAttempts)
            throw new UnauthorizedException("Maximum OTP attempts exceeded.");

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
        await _auditService.LogSecurityEventAsync(userId, "PhoneVerified", "Phone verified via OTP", null, null, cancellationToken: cancellationToken);

        return new MessageResponseDto("Phone number verified successfully.");
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(
        User user, string? ipAddress, bool rememberMe, DeviceInfoDto? device,
        CancellationToken cancellationToken, string? replacedToken = null)
    {
        if (user.Profile == null)
            user = await _identityRepository.GetUserByIdWithRolesAsync(user.Id, cancellationToken) ?? user;

        var roles = await _identityRepository.GetUserRoleNamesAsync(user.Id, cancellationToken);
        if (roles.Count == 0 && user.PrimaryRole != null)
            roles = [user.PrimaryRole.Name];

        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);
        var (accessToken, jwtId, expiresAt) = _tokenService.GenerateAccessToken(
            user.Id, user.Email, roles, permissions, user.IsEmailVerified);

        var refreshDays = rememberMe
            ? _configuration.GetValue("Jwt:RememberMeRefreshTokenExpirationDays", 30)
            : _configuration.GetValue("Jwt:RefreshTokenExpirationDays", 7);

        var session = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            JwtId = jwtId,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays),
            CreatedByIp = ipAddress,
            ReplacedByToken = replacedToken,
            RememberMe = rememberMe,
            DeviceId = device?.DeviceId,
            DeviceName = device?.DeviceName,
            Platform = device?.Platform,
            Browser = device?.Browser,
            UserAgent = device?.UserAgent,
            LastActivityAt = DateTime.UtcNow,
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles;
        userDto.Permissions = permissions;
        userDto.PrimaryRole = roles.FirstOrDefault(r => r.Equals(RoleNames.SuperAdmin, StringComparison.OrdinalIgnoreCase))
            ?? roles.FirstOrDefault(r => r.Equals(RoleNames.Admin, StringComparison.OrdinalIgnoreCase))
            ?? roles.FirstOrDefault()
            ?? user.PrimaryRole?.Name
            ?? user.Role.ToString();
        userDto.RequiresEmailVerification = !user.IsEmailVerified;
        userDto.RequiresPhoneVerification = !user.IsPhoneVerified;

        return new AuthResponseDto(accessToken, session.Token, expiresAt, session.Id, userDto);
    }

    private async Task RecordLoginAsync(Guid? userId, string? email, bool success, string? reason, string? ip, DeviceInfoDto? device, Guid? sessionId, CancellationToken ct) =>
        await _identityRepository.AddLoginHistoryAsync(new LoginHistory
        {
            UserId = userId, Email = email, IsSuccessful = success, FailureReason = reason,
            IpAddress = ip, UserAgent = device?.UserAgent, DeviceId = device?.DeviceId,
            DeviceName = device?.DeviceName, Platform = device?.Platform, Browser = device?.Browser,
            SessionId = sessionId,
        }, ct);

    private async Task<string> CreateAndSendEmailVerificationAsync(User user, CancellationToken cancellationToken)
    {
        var existing = await _authRepository.GetActiveEmailVerificationTokenAsync(user.Id, cancellationToken);
        if (existing != null) { existing.IsDeleted = true; existing.DeletedAt = DateTime.UtcNow; _unitOfWork.Repository<EmailVerificationToken>().Update(existing); }

        var rawToken = _tokenService.GenerateSecureToken();
        await _unitOfWork.Repository<EmailVerificationToken>().AddAsync(new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddHours(_configuration.GetValue("Auth:EmailVerificationExpirationHours", 24))
        }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await _emailService.SendEmailVerificationAsync(user.Email, user.Profile?.FirstName ?? user.Email, rawToken, user.Profile?.PreferredLanguage ?? "ar", cancellationToken);
    }

    private bool ShouldExposeAuthLinks() =>
        _configuration.GetValue("App:ExposeAuthLinks", false) ||
        string.Equals(_configuration["Email:Provider"], "Development", StringComparison.OrdinalIgnoreCase);

    private async Task<EmailVerificationToken?> FindEmailVerificationTokenAsync(string rawToken, CancellationToken ct)
    {
        var tokens = await _context.EmailVerificationTokens.Include(t => t.User).ThenInclude(u => u.Profile)
            .Where(t => t.VerifiedAt == null && t.ExpiresAt > DateTime.UtcNow).ToListAsync(ct);
        return tokens.FirstOrDefault(t => _tokenService.VerifyToken(rawToken, t.TokenHash));
    }

    private async Task<PasswordResetToken?> FindPasswordResetTokenAsync(string rawToken, CancellationToken ct)
    {
        var tokens = await _context.PasswordResetTokens.Include(t => t.User).ThenInclude(u => u.Profile)
            .Where(t => t.UsedAt == null && t.ExpiresAt > DateTime.UtcNow).ToListAsync(ct);
        return tokens.FirstOrDefault(t => _tokenService.VerifyToken(rawToken, t.TokenHash));
    }

    private static UserRole MapToLegacyEnum(string roleName) => roleName switch
    {
        RoleNames.Craftsman => UserRole.Craftsman,
        RoleNames.StoreOwner => UserRole.Store,
        RoleNames.Admin or RoleNames.SuperAdmin => UserRole.Administrator,
        _ => UserRole.Customer,
    };

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var parameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true, ValidateIssuer = true, ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"], ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)),
            ValidateLifetime = false
        };
        try { return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _); }
        catch { return null; }
    }
}
