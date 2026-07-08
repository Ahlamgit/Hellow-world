namespace Khadamati.Domain.Entities.Identity;

public class LoginHistory
{
    public long Id { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? Platform { get; set; }
    public string? Browser { get; set; }
    public Guid? SessionId { get; set; }
    public DateTime LoginAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
