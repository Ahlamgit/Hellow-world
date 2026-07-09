using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicyService _passwordPolicy;
    private readonly IPermissionService _permissionService;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly IAuditService _auditService;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        IUserRepository userRepository,
        IIdentityRepository identityRepository,
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IPasswordPolicyService passwordPolicy,
        IPermissionService permissionService,
        IEmailService emailService,
        ITokenService tokenService,
        IAuditService auditService,
        ILogger<UserManagementService> logger)
    {
        _userRepository = userRepository;
        _identityRepository = identityRepository;
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
        _permissionService = permissionService;
        _emailService = emailService;
        _tokenService = tokenService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<AdminUserDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("User not found.");
        return await MapDetailAsync(user, cancellationToken);
    }

    public async Task<PagedResult<AdminUserListItemDto>> SearchAsync(AdminUserListQueryDto query, CancellationToken cancellationToken = default)
    {
        UserStatus? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<UserStatus>(query.Status, true, out var parsedStatus))
            status = parsedStatus;

        var roleName = string.IsNullOrWhiteSpace(query.Role) ? null : RoleNames.All
            .FirstOrDefault(r => r.Equals(query.Role, StringComparison.OrdinalIgnoreCase));

        var (items, totalCount) = await _userRepository.SearchAsync(
            query.Search,
            status,
            roleName,
            query.FromDate,
            query.ToDate,
            query.SortBy,
            query.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase),
            query.Page,
            query.PageSize,
            cancellationToken);

        return new PagedResult<AdminUserListItemDto>
        {
            Items = items.Select(MapListItem).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<AdminUserDetailDto> CreateAsync(CreateAdminUserDto dto, Guid adminUserId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.UsersCreate, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        _passwordPolicy.ValidatePassword(dto.Password);

        if (await _userRepository.EmailExistsAsync(dto.Email, cancellationToken: cancellationToken))
            throw new ConflictException("Email is already registered.");
        if (await _userRepository.PhoneExistsAsync(dto.Phone, cancellationToken: cancellationToken))
            throw new ConflictException("Phone number is already registered.");

        var roleName = RoleNames.All.FirstOrDefault(r => r.Equals(dto.Role, StringComparison.OrdinalIgnoreCase))
            ?? throw new ValidationException(["Invalid role."]);
        var role = await _identityRepository.GetRoleByNameAsync(roleName, cancellationToken)
            ?? throw new ValidationException(["Role not found."]);

        if (!Enum.TryParse<UserStatus>(dto.Status, true, out var status))
            throw new ValidationException(["Invalid status."]);

        var user = new User
        {
            Email = dto.Email.ToLowerInvariant(),
            Phone = dto.Phone,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            Role = RoleNames.MapToLegacyEnum(roleName),
            PrimaryRoleId = role.Id,
            Status = status,
            VerificationStatus = VerificationStatus.Unverified,
            CreatedBy = adminUserId.ToString(),
            Profile = new UserProfile
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PreferredLanguage = dto.PreferredLanguage,
            },
        };

        if (roleName == RoleNames.Craftsman)
            user.CraftsmanProfile = new CraftsmanProfile();
        else if (roleName == RoleNames.StoreOwner)
            user.StoreProfile = new StoreProfile { StoreName = $"{dto.FirstName} {dto.LastName}" };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _identityRepository.AssignRoleAsync(user.Id, role.Id, isPrimary: true, assignedBy: adminUserId.ToString(), cancellationToken);
        await _identityRepository.AddPasswordHistoryAsync(new PasswordHistory
        {
            UserId = user.Id,
            PasswordHash = user.PasswordHash,
            ChangeReason = "AdminCreated",
            ChangedByIp = ipAddress,
        }, cancellationToken);

        if (dto.SendVerificationEmail)
            await SendEmailVerificationAsync(user, cancellationToken);

        await _auditService.LogSecurityEventAsync(adminUserId, "UserCreated", $"Admin created user {user.Id} as {roleName}", ipAddress, null, cancellationToken: cancellationToken);
        _logger.LogInformation("Admin {AdminId} created user {UserId}", adminUserId, user.Id);

        return await GetByIdAsync(user.Id, cancellationToken);
    }

    public async Task<AdminUserDetailDto> UpdateAsync(Guid id, UpdateAdminUserDto dto, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.UsersEdit, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        var user = await _userRepository.GetByIdAsync(id, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (await _userRepository.PhoneExistsAsync(dto.Phone, id, cancellationToken))
            throw new ConflictException("Phone number is already registered.");

        if (!Enum.TryParse<UserStatus>(dto.Status, true, out var status))
            throw new ValidationException(["Invalid status."]);

        user.Phone = dto.Phone;
        user.Status = status;
        user.UpdatedBy = adminUserId.ToString();
        user.Profile ??= new UserProfile { UserId = user.Id };
        user.Profile.FirstName = dto.FirstName;
        user.Profile.LastName = dto.LastName;
        user.Profile.PreferredLanguage = dto.PreferredLanguage;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(adminUserId, "UserUpdated", $"Admin updated user {id}", null, null, cancellationToken: cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.UsersDelete, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        if (id == adminUserId)
            throw new ValidationException(["You cannot delete your own account."]);

        var user = await _userRepository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("User not found.");

        _userRepository.SoftDelete(user, adminUserId.ToString());
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(adminUserId, "UserDeleted", $"Admin deleted user {id}", null, null, cancellationToken: cancellationToken);
    }

    public async Task<UserActionResponseDto> SuspendAsync(Guid id, SuspendUserDto dto, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.UsersSuspend, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        if (id == adminUserId)
            throw new ValidationException(["You cannot suspend your own account."]);

        var user = await _userRepository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.Status = UserStatus.Suspended;
        user.UpdatedBy = adminUserId.ToString();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var reason = string.IsNullOrWhiteSpace(dto.Reason) ? "Suspended by administrator" : dto.Reason;
        await _auditService.LogSecurityEventAsync(adminUserId, "UserSuspended", $"Admin suspended user {id}: {reason}", null, null, cancellationToken: cancellationToken);

        return new UserActionResponseDto
        {
            UserId = id,
            Status = user.Status.ToString(),
            Message = "User suspended successfully.",
        };
    }

    public async Task<UserActionResponseDto> ActivateAsync(Guid id, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.UsersEdit, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        var user = await _userRepository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.Status = UserStatus.Active;
        user.UpdatedBy = adminUserId.ToString();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(adminUserId, "UserActivated", $"Admin activated user {id}", null, null, cancellationToken: cancellationToken);

        return new UserActionResponseDto
        {
            UserId = id,
            Status = user.Status.ToString(),
            Message = "User activated successfully.",
        };
    }

    public async Task<AdminUserDetailDto> AssignRolesAsync(Guid id, AssignUserRolesDto dto, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.UsersEdit, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        var user = await _userRepository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var normalizedRoles = dto.Roles.Select(r => RoleNames.All.First(x => x.Equals(r, StringComparison.OrdinalIgnoreCase))).Distinct().ToList();
        var primary = RoleNames.All.FirstOrDefault(r => r.Equals(dto.PrimaryRole, StringComparison.OrdinalIgnoreCase))
            ?? throw new ValidationException(["Invalid primary role."]);
        if (!normalizedRoles.Contains(primary, StringComparer.OrdinalIgnoreCase))
            throw new ValidationException(["Primary role must be included in roles list."]);

        await _identityRepository.SetUserRolesAsync(id, normalizedRoles, primary, adminUserId.ToString(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditService.LogSecurityEventAsync(adminUserId, "UserRolesAssigned", $"Admin assigned roles to user {id}", null, null, cancellationToken: cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<UserPermissionMatrixDto> GetPermissionMatrixAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = await _userRepository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var roleIds = await _identityRepository.GetUserRoleIdsAsync(id, cancellationToken);
        var allPermissions = await _identityRepository.GetAllPermissionsAsync(cancellationToken);
        var rolePermissionIds = await _identityRepository.GetRolePermissionIdsAsync(roleIds, cancellationToken);
        var overrides = await _identityRepository.GetUserPermissionOverridesAsync(id, cancellationToken);
        var overrideMap = overrides.ToDictionary(o => o.PermissionId);

        var entries = allPermissions.Select(p =>
        {
            var (permId, code, nameEn, module) = p;
            var fromRole = rolePermissionIds.Contains(permId);
            overrideMap.TryGetValue(permId, out var ov);
            var effective = ov != null ? ov.IsGranted : fromRole;
            return new UserPermissionEntryDto
            {
                PermissionId = permId,
                Code = code,
                NameEn = nameEn,
                Module = module,
                FromRole = fromRole,
                Override = ov?.IsGranted,
                Effective = effective,
            };
        }).ToList();

        return new UserPermissionMatrixDto { UserId = id, Permissions = entries };
    }

    public async Task<UserPermissionMatrixDto> UpdatePermissionsAsync(
        Guid id, UpdateUserPermissionsDto dto, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.UserHasPermissionAsync(adminUserId, PermissionCodes.PermissionsManage, cancellationToken))
            throw new UnauthorizedException("Permission denied.");

        _ = await _userRepository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("User not found.");

        await _identityRepository.SetUserPermissionOverridesAsync(
            id,
            dto.Overrides.Select(o => (o.PermissionId, o.IsGranted)).ToList(),
            adminUserId.ToString(),
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _permissionService.InvalidateCache(id);
        await _auditService.LogSecurityEventAsync(
            adminUserId, "UserPermissionsUpdated", $"Admin updated permission overrides for user {id}", null, null, cancellationToken: cancellationToken);

        return await GetPermissionMatrixAsync(id, cancellationToken);
    }

    private async Task<AdminUserDetailDto> MapDetailAsync(User user, CancellationToken cancellationToken)
    {
        var roles = await _identityRepository.GetUserRoleNamesAsync(user.Id, cancellationToken);
        var permissions = await _identityRepository.GetUserPermissionCodesAsync(user.Id, cancellationToken);
        var item = MapListItem(user);
        return new AdminUserDetailDto
        {
            Id = item.Id,
            Email = item.Email,
            Phone = item.Phone,
            FirstName = item.FirstName,
            LastName = item.LastName,
            PrimaryRole = item.PrimaryRole,
            Roles = roles,
            Status = item.Status,
            VerificationStatus = item.VerificationStatus,
            EmailVerified = item.EmailVerified,
            PhoneVerified = item.PhoneVerified,
            CreatedAt = item.CreatedAt,
            LastLoginAt = item.LastLoginAt,
            SubscriptionStatus = user.SubscriptionStatus.ToString(),
            SubscriptionExpiresAt = user.SubscriptionExpiresAt,
            EmailVerifiedAt = user.EmailVerifiedAt,
            PhoneVerifiedAt = user.PhoneVerifiedAt,
            PreferredLanguage = user.Profile?.PreferredLanguage ?? "ar",
            Permissions = permissions,
        };
    }

    private static AdminUserListItemDto MapListItem(User user)
    {
        var roles = user.UserRoles?.Select(ur => ur.Role.Name).Distinct().ToList()
            ?? (user.PrimaryRole != null ? new List<string> { user.PrimaryRole.Name } : new List<string>());

        return new AdminUserListItemDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            FirstName = user.Profile?.FirstName ?? string.Empty,
            LastName = user.Profile?.LastName ?? string.Empty,
            PrimaryRole = user.PrimaryRole?.Name ?? user.Role.ToString(),
            Roles = roles,
            Status = user.Status.ToString(),
            VerificationStatus = user.VerificationStatus.ToString(),
            EmailVerified = user.EmailVerifiedAt.HasValue,
            PhoneVerified = user.PhoneVerifiedAt.HasValue,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
        };
    }

    private async Task SendEmailVerificationAsync(User user, CancellationToken cancellationToken)
    {
        var existing = await _authRepository.GetActiveEmailVerificationTokenAsync(user.Id, cancellationToken);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            _unitOfWork.Repository<EmailVerificationToken>().Update(existing);
        }

        var rawToken = _tokenService.GenerateSecureToken();
        await _unitOfWork.Repository<EmailVerificationToken>().AddAsync(new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddHours(24),
        }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _emailService.SendEmailVerificationAsync(
            user.Email,
            user.Profile?.FirstName ?? user.Email,
            rawToken,
            user.Profile?.PreferredLanguage ?? "ar",
            cancellationToken);
    }
}
