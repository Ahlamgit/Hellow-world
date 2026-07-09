namespace Khadamati.Application.DTOs.Verification;

public class SubmitVerificationDocumentDto
{
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
}

public class VerificationDocumentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class RejectVerificationDocumentDto
{
    public string Reason { get; set; } = string.Empty;
}

public class VerificationDocumentListQueryDto
{
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
