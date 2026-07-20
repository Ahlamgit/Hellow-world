using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Interfaces;

namespace Khadamati.Infrastructure.Services.Identity;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IPermissionService _permissionService;
    private readonly IAuditService _auditService;

    public SessionService(ISessionRepository sessionRepository, IPermissionService permissionService, IAuditService auditService)
    {
        _sessionRepository = sessionRepository;
        _permissionService = permissionService;
        _auditService = auditService;
    }

    public async Task<IReadOnlyList<SessionDto>> GetActiveSessionsAsync(Guid userId, Guid? currentSessionId, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionRepository.GetActiveSessionsAsync(userId, cancellationToken);
        return sessions.Select(s => new SessionDto
        {
            Id = s.Id,
            DeviceName = s.DeviceName,
            Platform = s.Platform,
            Browser = s.Browser,
            IpAddress = s.CreatedByIp,
            RememberMe = s.RememberMe,
            CreatedAt = s.CreatedAt,
            LastActivityAt = s.LastActivityAt,
            ExpiresAt = s.ExpiresAt,
            IsCurrent = currentSessionId.HasValue && s.Id == currentSessionId.Value,
        }).ToList();
    }

    public async Task RevokeSessionAsync(Guid userId, Guid sessionId, string? ipAddress, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken)
            ?? throw new NotFoundException("Session not found.");

        if (!isAdmin && session.UserId != userId)
            throw new UnauthorizedException("Cannot revoke another user's session.");

        if (isAdmin && session.UserId != userId &&
            !await _permissionService.UserHasPermissionAsync(userId, PermissionCodes.SessionsRevoke, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        await _sessionRepository.RevokeSessionAsync(sessionId, ipAddress, cancellationToken);
        await _auditService.LogSecurityEventAsync(userId, "SessionRevoked", $"Session {sessionId} revoked", ipAddress, session.UserAgent, cancellationToken: cancellationToken);
    }

    public async Task RevokeAllOtherSessionsAsync(Guid userId, Guid currentSessionId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        await _sessionRepository.RevokeAllExceptAsync(userId, currentSessionId, ipAddress, cancellationToken);
        await _auditService.LogSecurityEventAsync(userId, "SessionsRevokedOthers", "All other sessions revoked", ipAddress, null, cancellationToken: cancellationToken);
    }

    public async Task RevokeAllSessionsAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        await _sessionRepository.RevokeAllAsync(userId, ipAddress, cancellationToken);
        await _auditService.LogSecurityEventAsync(userId, "SessionsRevokedAll", "All sessions revoked", ipAddress, null, cancellationToken: cancellationToken);
    }
}

public class ProfileService : IProfileService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public ProfileService(IIdentityRepository identityRepository, IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _identityRepository = identityRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<ProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");
        return MapProfile(user);
    }

    public async Task<ProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _identityRepository.GetUserByIdWithRolesAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (user.Profile == null)
        {
            user.Profile = new Domain.Entities.UserProfile { UserId = userId };
            await _unitOfWork.Repository<Domain.Entities.UserProfile>().AddAsync(user.Profile, cancellationToken);
        }

        user.Profile.FirstName = request.FirstName;
        user.Profile.LastName = request.LastName;
        user.Profile.Gender = request.Gender;
        user.Profile.BirthDate = request.BirthDate;
        user.Profile.Nationality = request.Nationality;
        user.Profile.ProfilePictureUrl = request.ProfilePictureUrl;
        user.Profile.AddressLine = request.AddressLine;
        user.Profile.Country = PlatformConstants.DefaultCountryCode;
        user.Profile.City = request.City;
        user.Profile.Region = request.Region;
        user.Profile.Latitude = request.Latitude;
        user.Profile.Longitude = request.Longitude;
        user.Profile.PreferredLanguage = request.PreferredLanguage;
        user.Profile.Timezone = request.Timezone;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Domain.Entities.UserProfile>().Update(user.Profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogAuditAsync("UserProfiles", "Update", userId.ToString(), userId, user.Email, null, null, null, cancellationToken);

        return MapProfile(user);
    }

    private static ProfileDto MapProfile(Domain.Entities.User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Phone = user.Phone,
        FirstName = user.Profile?.FirstName ?? "",
        LastName = user.Profile?.LastName ?? "",
        FullName = user.Profile?.FullName ?? "",
        Gender = user.Profile?.Gender,
        BirthDate = user.Profile?.BirthDate,
        Nationality = user.Profile?.Nationality,
        ProfilePictureUrl = user.Profile?.ProfilePictureUrl,
        AddressLine = user.Profile?.AddressLine,
        Country = user.Profile?.Country,
        City = user.Profile?.City,
        Region = user.Profile?.Region,
        Latitude = user.Profile?.Latitude,
        Longitude = user.Profile?.Longitude,
        PreferredLanguage = user.Profile?.PreferredLanguage ?? "ar",
        Timezone = user.Profile?.Timezone ?? "Asia/Riyadh",
        EmailVerified = user.IsEmailVerified,
        PhoneVerified = user.IsPhoneVerified,
    };
}

public class LoginHistoryService : ILoginHistoryService
{
    private readonly IIdentityRepository _identityRepository;

    public LoginHistoryService(IIdentityRepository identityRepository) => _identityRepository = identityRepository;

    public async Task<PagedLoginHistoryDto> GetUserLoginHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = await _identityRepository.GetLoginHistoryAsync(userId, page, pageSize, cancellationToken);
        var total = await _identityRepository.CountLoginHistoryAsync(userId, cancellationToken);
        return new PagedLoginHistoryDto
        {
            Items = items.Select(l => new LoginHistoryDto
            {
                Id = l.Id, IsSuccessful = l.IsSuccessful, FailureReason = l.FailureReason,
                IpAddress = l.IpAddress, DeviceName = l.DeviceName, Platform = l.Platform,
                Browser = l.Browser, LoginAt = l.LoginAt,
            }).ToList(),
            TotalCount = total, Page = page, PageSize = pageSize,
        };
    }
}
