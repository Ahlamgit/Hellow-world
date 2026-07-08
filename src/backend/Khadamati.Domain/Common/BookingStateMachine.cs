using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Common;

public static class BookingStateMachine
{
    private static readonly Dictionary<ServiceRequestStatus, HashSet<ServiceRequestStatus>> AllowedTransitions = new()
    {
        [ServiceRequestStatus.Pending] = [ServiceRequestStatus.AwaitingPayment, ServiceRequestStatus.Cancelled, ServiceRequestStatus.Expired],
        [ServiceRequestStatus.AwaitingPayment] = [ServiceRequestStatus.PaymentConfirmed, ServiceRequestStatus.Cancelled, ServiceRequestStatus.Expired],
        [ServiceRequestStatus.PaymentConfirmed] = [ServiceRequestStatus.PendingCraftsmanConfirmation],
        [ServiceRequestStatus.PendingCraftsmanConfirmation] = [ServiceRequestStatus.Confirmed, ServiceRequestStatus.Rejected],
        [ServiceRequestStatus.Confirmed] = [ServiceRequestStatus.Completed, ServiceRequestStatus.Cancelled, ServiceRequestStatus.Rescheduled, ServiceRequestStatus.NoShow],
        [ServiceRequestStatus.Rescheduled] = [ServiceRequestStatus.AwaitingPayment, ServiceRequestStatus.Cancelled],
        [ServiceRequestStatus.Rejected] = [],
        [ServiceRequestStatus.Completed] = [],
        [ServiceRequestStatus.Cancelled] = [],
        [ServiceRequestStatus.Expired] = [],
        [ServiceRequestStatus.NoShow] = []
    };

    public static bool CanTransition(ServiceRequestStatus from, ServiceRequestStatus to) =>
        AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static void ValidateTransition(ServiceRequestStatus from, ServiceRequestStatus to)
    {
        if (!CanTransition(from, to))
            throw new InvalidOperationException($"Cannot transition booking from {from} to {to}.");
    }

    public static readonly ServiceRequestStatus[] ActiveSlotStatuses =
    [
        ServiceRequestStatus.PaymentConfirmed,
        ServiceRequestStatus.PendingCraftsmanConfirmation,
        ServiceRequestStatus.Confirmed,
        ServiceRequestStatus.Rescheduled
    ];
}
