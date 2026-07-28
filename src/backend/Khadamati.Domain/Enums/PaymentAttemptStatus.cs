namespace Khadamati.Domain.Enums;

public enum PaymentAttemptStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    AwaitingGatewayConfirmation = 6,
    Expired = 7,
    Abandoned = 8
}
