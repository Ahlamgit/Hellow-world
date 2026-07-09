namespace Khadamati.Application.DTOs.Messaging;

public class RegisterPushTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
}

public class PushNotificationPayload
{
    public Guid UserId { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string MessageEn { get; set; } = string.Empty;
    public string MessageAr { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
}
