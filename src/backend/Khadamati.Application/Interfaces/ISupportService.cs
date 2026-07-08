using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Support;

namespace Khadamati.Application.Interfaces;

public interface ISupportService
{
    Task<ComplaintDto> CreateComplaintAsync(Guid userId, CreateComplaintDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComplaintDto>> GetMyComplaintsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SupportTicketDto> CreateTicketAsync(Guid userId, CreateSupportTicketDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupportTicketDto>> GetMyTicketsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BookingDto> SubmitReviewAsync(Guid bookingId, Guid customerId, SubmitReviewDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdvertisementDto>> GetActiveAdsAsync(string placement, CancellationToken cancellationToken = default);
}
