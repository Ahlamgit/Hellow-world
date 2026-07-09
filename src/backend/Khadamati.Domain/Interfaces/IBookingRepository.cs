using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Interfaces;

public interface IBookingRepository
{
    Task<ServiceRequest?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<ServiceRequest?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ServiceRequest> Items, int TotalCount)> SearchAsync(
        Guid? customerId, Guid? craftsmanId, ServiceRequestStatus? status,
        DateTime? fromDate, DateTime? toDate, int page, int pageSize,
        CancellationToken cancellationToken = default);
    Task<bool> IsSlotBookedAsync(Guid craftsmanId, DateTime slotStart, DateTime slotEnd, Guid? excludeBookingId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingSlotReservation>> GetBookedSlotsAsync(Guid craftsmanId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CraftsmanWorkingHour>> GetWorkingHoursAsync(Guid craftsmanId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CraftsmanProfile>> GetCraftsmenForServiceAsync(Guid serviceId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, Address>> GetDefaultAddressesForUsersAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
    Task AddAsync(ServiceRequest booking, CancellationToken cancellationToken = default);
    void Update(ServiceRequest booking);
    Task AddPaymentAsync(BookingPayment payment, CancellationToken cancellationToken = default);
    Task<BookingPayment?> GetPaymentByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<BookingPayment?> GetPaymentByTransactionReferenceAsync(string transactionReference, CancellationToken cancellationToken = default);
    Task AddSlotReservationAsync(BookingSlotReservation reservation, CancellationToken cancellationToken = default);
    Task ReleaseSlotReservationAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task AddStatusHistoryAsync(ServiceRequestStatusHistory history, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceRequestStatusHistory>> GetStatusHistoryAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetNotificationsAsync(Guid userId, bool unreadOnly, int page, int pageSize, CancellationToken cancellationToken = default);
}
