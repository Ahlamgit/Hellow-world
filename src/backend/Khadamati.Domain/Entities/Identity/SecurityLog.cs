namespace Khadamati.Domain.Entities.Identity;

public class SecurityLog
{
    public long Id { get; set; }
    public Guid? UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
