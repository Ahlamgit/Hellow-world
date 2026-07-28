using Khadamati.Domain.Common;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context) => _context = context;

    public async Task<ServiceRequest?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ServiceRequests.AsQueryable();
        if (includeDetails)
        {
            query = query
                .Include(b => b.Service)
                .Include(b => b.Customer).ThenInclude(c => c.Profile)
                .Include(b => b.Craftsman).ThenInclude(c => c.Profile)
                .Include(b => b.Payment)
                .Include(b => b.StatusHistory.OrderByDescending(h => h.CreatedAt));
        }
        return await query.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public Task<ServiceRequest?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default) =>
        _context.ServiceRequests.FirstOrDefaultAsync(b => b.BookingReference == reference, cancellationToken);

    public async Task<(IReadOnlyList<ServiceRequest> Items, int TotalCount)> SearchAsync(
        Guid? customerId, Guid? craftsmanId, ServiceRequestStatus? status,
        DateTime? fromDate, DateTime? toDate, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ServiceRequests
            .Include(b => b.Service)
            .Include(b => b.Customer).ThenInclude(c => c.Profile)
            .Include(b => b.Craftsman).ThenInclude(c => c.Profile)
            .Include(b => b.Payment)
            .AsQueryable();

        if (customerId.HasValue) query = query.Where(b => b.CustomerId == customerId.Value);
        if (craftsmanId.HasValue) query = query.Where(b => b.CraftsmanId == craftsmanId.Value);
        if (status.HasValue) query = query.Where(b => b.Status == status.Value);
        if (fromDate.HasValue) query = query.Where(b => b.ScheduledAt >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(b => b.ScheduledAt <= toDate.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<bool> IsSlotBookedAsync(Guid craftsmanId, DateTime slotStart, DateTime slotEnd, Guid? excludeBookingId = null, CancellationToken cancellationToken = default)
    {
        var activeStatuses = BookingStateMachine.ActiveSlotStatuses;

        var hasReservation = await _context.BookingSlotReservations.AnyAsync(r =>
            r.CraftsmanId == craftsmanId && r.IsActive &&
            r.SlotStart < slotEnd && r.SlotEnd > slotStart &&
            (excludeBookingId == null || r.ServiceRequestId != excludeBookingId.Value), cancellationToken);

        if (hasReservation) return true;

        return await _context.ServiceRequests.AnyAsync(b =>
            b.CraftsmanId == craftsmanId &&
            activeStatuses.Contains(b.Status) &&
            b.ScheduledAt < slotEnd && b.SlotEnd > slotStart &&
            (excludeBookingId == null || b.Id != excludeBookingId.Value), cancellationToken);
    }

    public Task<IReadOnlyList<BookingSlotReservation>> GetBookedSlotsAsync(Guid craftsmanId, DateTime from, DateTime to, CancellationToken cancellationToken = default) =>
        _context.BookingSlotReservations
            .Where(r => r.CraftsmanId == craftsmanId && r.IsActive && r.SlotStart < to && r.SlotEnd > from)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<BookingSlotReservation>)t.Result, cancellationToken);

    public Task<IReadOnlyList<CraftsmanWorkingHour>> GetWorkingHoursAsync(Guid craftsmanId, CancellationToken cancellationToken = default) =>
        _context.CraftsmanWorkingHours
            .Where(w => w.CraftsmanId == craftsmanId && w.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<CraftsmanWorkingHour>)t.Result, cancellationToken);

    public async Task<IReadOnlyList<CraftsmanProfile>> GetCraftsmenForServiceAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        return await _context.CraftsmanServices
            .Include(cs => cs.CraftsmanProfile).ThenInclude(cp => cp.User).ThenInclude(u => u.Profile)
            .Include(cs => cs.CraftsmanProfile).ThenInclude(cp => cp.Services)
            .Where(cs => cs.ServiceId == serviceId && cs.IsAvailable && !cs.IsDeleted &&
                         cs.CraftsmanProfile.IsAvailable && !cs.CraftsmanProfile.IsDeleted)
            .Select(cs => cs.CraftsmanProfile)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, Address>> GetDefaultAddressesForUsersAsync(
        IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var ids = userIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<Guid, Address>();

        var addresses = await _context.Addresses
            .Where(a => ids.Contains(a.UserId) && a.IsDefault && a.Latitude != null && a.Longitude != null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return addresses
            .GroupBy(a => a.UserId)
            .ToDictionary(g => g.Key, g => g.First());
    }

    public async Task AddAsync(ServiceRequest booking, CancellationToken cancellationToken = default) =>
        await _context.ServiceRequests.AddAsync(booking, cancellationToken);

    public void Update(ServiceRequest booking) => _context.ServiceRequests.Update(booking);

    public async Task AddPaymentAsync(BookingPayment payment, CancellationToken cancellationToken = default) =>
        await _context.BookingPayments.AddAsync(payment, cancellationToken);

    public Task<BookingPayment?> GetPaymentByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default) =>
        _context.BookingPayments.FirstOrDefaultAsync(p => p.ServiceRequestId == bookingId, cancellationToken);

    public async Task<BookingPayment?> GetPaymentByTransactionReferenceAsync(string transactionReference, CancellationToken cancellationToken = default)
    {
        var payment = await _context.BookingPayments
            .FirstOrDefaultAsync(p => p.TransactionReference == transactionReference, cancellationToken);
        if (payment is not null)
            return payment;

        var attempt = await _context.BookingPaymentAttempts
            .Include(a => a.BookingPayment)
            .FirstOrDefaultAsync(
                a => a.SessionId == transactionReference ||
                     a.ProviderTransactionId == transactionReference ||
                     a.ProviderUuid == transactionReference ||
                     a.MerchantTransactionId == transactionReference,
                cancellationToken);
        return attempt?.BookingPayment;
    }

    public async Task AddSlotReservationAsync(BookingSlotReservation reservation, CancellationToken cancellationToken = default) =>
        await _context.BookingSlotReservations.AddAsync(reservation, cancellationToken);

    public async Task ReleaseSlotReservationAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        var reservation = await _context.BookingSlotReservations
            .FirstOrDefaultAsync(r => r.ServiceRequestId == bookingId && r.IsActive, cancellationToken);
        if (reservation != null)
        {
            reservation.IsActive = false;
            reservation.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task AddStatusHistoryAsync(ServiceRequestStatusHistory history, CancellationToken cancellationToken = default) =>
        await _context.ServiceRequestStatusHistories.AddAsync(history, cancellationToken);

    public Task<IReadOnlyList<ServiceRequestStatusHistory>> GetStatusHistoryAsync(Guid bookingId, CancellationToken cancellationToken = default) =>
        _context.ServiceRequestStatusHistories
            .Where(h => h.ServiceRequestId == bookingId)
            .OrderByDescending(h => h.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<ServiceRequestStatusHistory>)t.Result, cancellationToken);

    public async Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default) =>
        await _context.Notifications.AddAsync(notification, cancellationToken);

    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetNotificationsAsync(
        Guid userId, bool unreadOnly, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);
        if (unreadOnly) query = query.Where(n => !n.IsRead);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
