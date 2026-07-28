using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;

namespace Khadamati.Application.Interfaces;

public interface IBookingService
{
    Task<IReadOnlyList<CraftsmanOptionDto>> GetCraftsmenForServiceAsync(Guid serviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CraftsmanOptionDto>> GetNearbyCraftsmenForServiceAsync(
        Guid serviceId, double latitude, double longitude, double radiusKm = 25, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(Guid craftsmanId, Guid serviceId, DateTime date, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateBookingAsync(Guid customerId, CreateBookingDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> ConfirmBookingAsync(Guid bookingId, Guid userId, ConfirmBookingDto dto, CancellationToken cancellationToken = default);
    Task<BookingPaymentDto> InitiatePaymentAsync(Guid bookingId, Guid userId, InitiatePaymentDto dto, CancellationToken cancellationToken = default);
    Task<AuthorizePaymentResultDto> AuthorizePaymentAsync(Guid bookingId, Guid userId, AuthorizePaymentDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> ConfirmPaymentAsync(Guid bookingId, Guid userId, ConfirmPaymentDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> ConfirmPaymentFromWebhookAsync(string transactionReference, CancellationToken cancellationToken = default);
    Task<BookingDto> AcceptBookingAsync(Guid bookingId, Guid craftsmanId, CancellationToken cancellationToken = default);
    Task<BookingDto> RejectBookingAsync(Guid bookingId, Guid craftsmanId, RejectBookingDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> CancelBookingAsync(Guid bookingId, Guid userId, string role, CancelBookingDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> CompleteBookingAsync(Guid bookingId, Guid craftsmanId, CancellationToken cancellationToken = default);
    Task<BookingDto> MarkNoShowAsync(Guid bookingId, Guid craftsmanId, CancellationToken cancellationToken = default);
    Task<BookingDto> RescheduleBookingAsync(Guid bookingId, Guid userId, string role, RescheduleBookingDto dto, CancellationToken cancellationToken = default);
    Task<BookingDto> GetBookingAsync(Guid bookingId, Guid userId, string role, CancellationToken cancellationToken = default);
    Task<PagedResult<BookingDto>> ListBookingsAsync(Guid userId, string role, BookingListQueryDto query, CancellationToken cancellationToken = default);
    Task<PagedResult<BookingDto>> AdminListBookingsAsync(BookingListQueryDto query, CancellationToken cancellationToken = default);
    Task<AdminBookingStatsDto> GetAdminStatsAsync(CancellationToken cancellationToken = default);
    Task<BookingDto> AdminGetBookingAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<PagedResult<NotificationDto>> GetNotificationsAsync(Guid userId, bool unreadOnly, int page, int pageSize, CancellationToken cancellationToken = default);
    Task MarkNotificationReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);
}
