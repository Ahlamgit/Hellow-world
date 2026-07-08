using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Support;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class SupportService : ISupportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingService _bookingService;

    public SupportService(IUnitOfWork unitOfWork, IBookingService bookingService)
    {
        _unitOfWork = unitOfWork;
        _bookingService = bookingService;
    }

    public async Task<ComplaintDto> CreateComplaintAsync(Guid userId, CreateComplaintDto dto, CancellationToken cancellationToken = default)
    {
        var complaint = new Complaint
        {
            ComplainantUserId = userId,
            Subject = dto.Subject.Trim(),
            Description = dto.Description.Trim(),
            Status = "Open",
            Priority = dto.Priority,
        };
        await _unitOfWork.Repository<Complaint>().AddAsync(complaint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapComplaint(complaint);
    }

    public async Task<IReadOnlyList<ComplaintDto>> GetMyComplaintsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Repository<Complaint>()
            .FindAsync(c => c.ComplainantUserId == userId && !c.IsDeleted, cancellationToken);
        return items.OrderByDescending(c => c.CreatedAt).Select(MapComplaint).ToList();
    }

    public async Task<SupportTicketDto> CreateTicketAsync(Guid userId, CreateSupportTicketDto dto, CancellationToken cancellationToken = default)
    {
        var count = await _unitOfWork.Repository<SupportTicket>().CountAsync(cancellationToken: cancellationToken);
        var ticket = new SupportTicket
        {
            UserId = userId,
            TicketNumber = $"TKT-{count + 1:D5}",
            Subject = dto.Subject.Trim(),
            Description = dto.Description.Trim(),
            Status = "Open",
            Priority = dto.Priority,
            Category = dto.Category,
        };
        await _unitOfWork.Repository<SupportTicket>().AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapTicket(ticket);
    }

    public async Task<IReadOnlyList<SupportTicketDto>> GetMyTicketsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Repository<SupportTicket>()
            .FindAsync(t => t.UserId == userId && !t.IsDeleted, cancellationToken);
        return items.OrderByDescending(t => t.CreatedAt).Select(MapTicket).ToList();
    }

    public async Task<BookingDto> SubmitReviewAsync(Guid bookingId, Guid customerId, SubmitReviewDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Rating is < 1 or > 5)
            throw new ValidationException(["Rating must be between 1 and 5."]);

        var booking = await _unitOfWork.Repository<ServiceRequest>().GetByIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");

        if (booking.CustomerId != customerId)
            throw new UnauthorizedException("Only the customer can submit a review.");

        if (booking.Status != ServiceRequestStatus.Completed)
            throw new ConflictException("Reviews can only be submitted for completed bookings.");

        booking.CustomerRating = dto.Rating;
        booking.CustomerReview = dto.Review?.Trim();
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<ServiceRequest>().Update(booking);

        var profile = await _unitOfWork.Repository<CraftsmanProfile>()
            .FirstOrDefaultAsync(cp => cp.UserId == booking.CraftsmanId && !cp.IsDeleted, cancellationToken);

        if (profile is not null)
        {
            var completed = await _unitOfWork.Repository<ServiceRequest>()
                .FindAsync(b => b.CraftsmanId == booking.CraftsmanId && b.Status == ServiceRequestStatus.Completed && b.CustomerRating != null, cancellationToken);

            profile.TotalReviews = completed.Count;
            profile.Rating = completed.Count > 0
                ? (decimal)completed.Average(b => b.CustomerRating!.Value)
                : dto.Rating;
            profile.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<CraftsmanProfile>().Update(profile);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await _bookingService.GetBookingAsync(bookingId, customerId, "Customer", cancellationToken);
    }

    public async Task<IReadOnlyList<AdvertisementDto>> GetActiveAdsAsync(string placement, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var ads = await _unitOfWork.Repository<Advertisement>()
            .FindAsync(a => a.IsActive && !a.IsDeleted
                && a.Placement == placement
                && a.StartDate <= now
                && a.EndDate >= now, cancellationToken);

        return ads.Select(a => new AdvertisementDto
        {
            Id = a.Id,
            TitleEn = a.TitleEn,
            TitleAr = a.TitleAr,
            Placement = a.Placement,
        }).ToList();
    }

    private static ComplaintDto MapComplaint(Complaint c) => new()
    {
        Id = c.Id,
        Subject = c.Subject,
        Description = c.Description,
        Status = c.Status,
        Priority = c.Priority,
        CreatedAt = c.CreatedAt,
    };

    private static SupportTicketDto MapTicket(SupportTicket t) => new()
    {
        Id = t.Id,
        TicketNumber = t.TicketNumber,
        Subject = t.Subject,
        Description = t.Description,
        Status = t.Status,
        Priority = t.Priority,
        Category = t.Category,
        CreatedAt = t.CreatedAt,
    };
}
