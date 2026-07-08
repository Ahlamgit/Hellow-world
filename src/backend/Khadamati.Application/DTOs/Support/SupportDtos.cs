namespace Khadamati.Application.DTOs.Support;

public class CreateComplaintDto
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
}

public class ComplaintDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateSupportTicketDto
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string Priority { get; set; } = "Normal";
}

public class SupportTicketDto
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SubmitReviewDto
{
    public int Rating { get; set; }
    public string? Review { get; set; }
}

public class AdvertisementDto
{
    public Guid Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Placement { get; set; } = string.Empty;
}
