namespace Khadamati.Domain.Enums;

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5,
    Cancelled = 6,
    /// <summary>Customer returned from checkout or waiting on webhook/verify.</summary>
    AwaitingGatewayConfirmation = 7,
    /// <summary>Payment window elapsed without successful confirmation.</summary>
    Expired = 8
}
