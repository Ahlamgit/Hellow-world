namespace Khadamati.Domain.Enums;

public enum ServiceRequestStatus
{
    Pending = 1,
    AwaitingPayment = 2,
    PaymentConfirmed = 3,
    PendingCraftsmanConfirmation = 4,
    Confirmed = 5,
    Completed = 6,
    Cancelled = 7,
    Rejected = 8,
    Expired = 9,
    NoShow = 10,
    Rescheduled = 11
}
