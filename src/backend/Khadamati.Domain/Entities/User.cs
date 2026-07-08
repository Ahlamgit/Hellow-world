using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Pending;
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Unverified;
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.None;
    public DateTime? SubscriptionExpiresAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? PhoneVerifiedAt { get; set; }
    public bool IsEmailVerified => EmailVerifiedAt.HasValue;
    public bool IsPhoneVerified => PhoneVerifiedAt.HasValue;

    public UserProfile? Profile { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    public ICollection<PhoneOtpToken> PhoneOtpTokens { get; set; } = new List<PhoneOtpToken>();
    public CraftsmanProfile? CraftsmanProfile { get; set; }
    public StoreProfile? StoreProfile { get; set; }
    public ICollection<ServiceRequest> CustomerRequests { get; set; } = new List<ServiceRequest>();
    public ICollection<ServiceRequest> AssignedRequests { get; set; } = new List<ServiceRequest>();
}
