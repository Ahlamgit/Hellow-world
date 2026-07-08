namespace Khadamati.Application.DTOs.Users;

public class AdminUserListQueryDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Role { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class AdminUserListItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PrimaryRole { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public string Status { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class AdminUserDetailDto : AdminUserListItemDto
{
    public string SubscriptionStatus { get; set; } = string.Empty;
    public DateTime? SubscriptionExpiresAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
}

public class CreateAdminUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "ar";
    public string Status { get; set; } = "Active";
    public bool SendVerificationEmail { get; set; } = true;
}

public class UpdateAdminUserDto
{
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "ar";
    public string Status { get; set; } = string.Empty;
}

public class AssignUserRolesDto
{
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public string PrimaryRole { get; set; } = string.Empty;
}

public class SuspendUserDto
{
    public string? Reason { get; set; }
}

public class UserActionResponseDto
{
    public Guid UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
