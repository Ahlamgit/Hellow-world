using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class VerificationDocument : BaseEntity
{
    public Guid UserId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public VerificationStatus Status { get; set; } = VerificationStatus.PendingReview;
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }

    public User User { get; set; } = null!;
    public User? ReviewedBy { get; set; }
}
