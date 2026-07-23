using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Common;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;

namespace Khadamati.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IPermissionService _permissionService;

    public BookingService(
        IBookingRepository repository,
        IUnitOfWork unitOfWork,
        IPaymentGateway paymentGateway,
        IPushNotificationService pushNotificationService,
        IPermissionService permissionService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
        _pushNotificationService = pushNotificationService;
        _permissionService = permissionService;
    }

    public async Task<IReadOnlyList<CraftsmanOptionDto>> GetCraftsmenForServiceAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        var craftsmen = await _repository.GetCraftsmenForServiceAsync(serviceId, cancellationToken);
        return craftsmen.Select(cp => MapCraftsmanOption(cp, serviceId, null)).ToList();
    }

    public async Task<IReadOnlyList<CraftsmanOptionDto>> GetNearbyCraftsmenForServiceAsync(
        Guid serviceId, double latitude, double longitude, double radiusKm = 25, CancellationToken cancellationToken = default)
    {
        var craftsmen = await _repository.GetCraftsmenForServiceAsync(serviceId, cancellationToken);
        var addresses = await _repository.GetDefaultAddressesForUsersAsync(craftsmen.Select(c => c.UserId), cancellationToken);

        return craftsmen
            .Select(cp =>
            {
                double? distance = null;
                if (addresses.TryGetValue(cp.UserId, out var address) &&
                    address.Latitude.HasValue && address.Longitude.HasValue)
                {
                    distance = HaversineKm(
                        latitude, longitude,
                        (double)address.Latitude.Value, (double)address.Longitude.Value);
                }
                var craftsmanRadius = cp.ServiceRadiusKm is > 0 ? (double)cp.ServiceRadiusKm.Value : radiusKm;
                var effectiveRadius = Math.Min(radiusKm, craftsmanRadius);
                return (Profile: cp, Distance: distance, EffectiveRadius: effectiveRadius);
            })
            .Where(x => x.Distance.HasValue && x.Distance.Value <= x.EffectiveRadius)
            .OrderBy(x => x.Distance)
            .Select(x => MapCraftsmanOption(x.Profile, serviceId, x.Distance))
            .ToList();
    }

    private static CraftsmanOptionDto MapCraftsmanOption(CraftsmanProfile cp, Guid serviceId, double? distanceKm) => new()
    {
        Id = cp.UserId,
        FirstName = cp.User.Profile?.FirstName ?? string.Empty,
        LastName = cp.User.Profile?.LastName ?? string.Empty,
        Specialization = cp.Specialization,
        Rating = cp.Rating,
        TotalReviews = cp.TotalReviews,
        CompletedJobs = cp.CompletedJobs,
        Price = cp.Services.FirstOrDefault(s => s.ServiceId == serviceId)?.CustomPrice ?? 0,
        IsAvailable = cp.IsAvailable,
        DistanceKm = distanceKm,
    };

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    public async Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(Guid craftsmanId, Guid serviceId, DateTime date, CancellationToken cancellationToken = default)
    {
        var service = await _unitOfWork.Repository<Service>().GetByIdAsync(serviceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        var duration = TimeSpan.FromMinutes(Math.Max(service.EstimatedDurationMinutes, 30));
        var dayStart = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        var workingHours = await _repository.GetWorkingHoursAsync(craftsmanId, cancellationToken);
        var dayOfWeek = dayStart.DayOfWeek;
        var hours = workingHours.Where(w => w.DayOfWeek == dayOfWeek).ToList();

        TimeOnly workStart = hours.Count > 0 ? hours.Min(h => h.StartTime) : new TimeOnly(9, 0);
        TimeOnly workEnd = hours.Count > 0 ? hours.Max(h => h.EndTime) : new TimeOnly(18, 0);

        var slotStart = dayStart.Add(workStart.ToTimeSpan());
        var workEndDt = dayStart.Add(workEnd.ToTimeSpan());

        var bookedSlots = await _repository.GetBookedSlotsAsync(craftsmanId, dayStart, dayEnd, cancellationToken);
        var slots = new List<TimeSlotDto>();

        while (slotStart.Add(duration) <= workEndDt)
        {
            var slotEnd = slotStart.Add(duration);
            var isBooked = bookedSlots.Any(b => b.SlotStart < slotEnd && b.SlotEnd > slotStart);
            if (!isBooked)
                isBooked = await _repository.IsSlotBookedAsync(craftsmanId, slotStart, slotEnd, cancellationToken: cancellationToken);

            var isAvailable = !isBooked && slotStart > DateTime.UtcNow;
            slots.Add(new TimeSlotDto { Start = slotStart, End = slotEnd, IsAvailable = isAvailable });
            slotStart = slotStart.Add(duration);
        }

        return slots;
    }

    public async Task<BookingDto> CreateBookingAsync(Guid customerId, CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var service = await _unitOfWork.Repository<Service>().GetByIdAsync(dto.ServiceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        var craftsmanProfile = await _unitOfWork.Repository<CraftsmanProfile>()
            .FirstOrDefaultAsync(cp => cp.UserId == dto.CraftsmanId && !cp.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Craftsman not found.");

        var craftsmanService = await _unitOfWork.Repository<Domain.Entities.CraftsmanService>()
            .FirstOrDefaultAsync(cs => cs.CraftsmanProfileId == craftsmanProfile.Id && cs.ServiceId == dto.ServiceId && cs.IsAvailable, cancellationToken)
            ?? throw new ConflictException("Craftsman does not offer this service.");

        var slotEnd = dto.ScheduledAt.AddMinutes(service.EstimatedDurationMinutes);
        if (await _repository.IsSlotBookedAsync(dto.CraftsmanId, dto.ScheduledAt, slotEnd, cancellationToken: cancellationToken))
            throw new ConflictException("Selected time slot is no longer available.");

        var booking = new ServiceRequest
        {
            BookingReference = GenerateReference(),
            CustomerId = customerId,
            ServiceId = dto.ServiceId,
            CraftsmanId = dto.CraftsmanId,
            AddressId = dto.AddressId,
            Description = dto.Description,
            Notes = dto.Notes,
            ScheduledAt = dto.ScheduledAt,
            SlotEnd = slotEnd,
            EstimatedPrice = craftsmanService.CustomPrice > 0 ? craftsmanService.CustomPrice : service.BasePrice,
            Status = ServiceRequestStatus.Pending
        };

        await _repository.AddAsync(booking, cancellationToken);
        await AddHistoryAsync(booking, null, ServiceRequestStatus.Pending, customerId, "Booking created", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        booking.Service = service;
        booking.Customer = await _unitOfWork.Repository<User>().GetByIdAsync(customerId, cancellationToken) ?? new User();
        booking.Craftsman = await _unitOfWork.Repository<User>().GetByIdAsync(dto.CraftsmanId, cancellationToken) ?? new User();

        return MapToDto(booking);
    }

    public async Task<BookingDto> ConfirmBookingAsync(Guid bookingId, Guid userId, ConfirmBookingDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingForCustomerAsync(bookingId, userId, cancellationToken);
        await TransitionAsync(booking, ServiceRequestStatus.AwaitingPayment, userId, "Booking confirmed, awaiting payment", cancellationToken);

        booking.PaymentDueAt = DateTime.UtcNow.AddMinutes(30);
        booking.ExpiresAt = DateTime.UtcNow.AddMinutes(30);
        if (!string.IsNullOrWhiteSpace(dto.Notes)) booking.Notes = dto.Notes;

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));
    }

    public async Task<BookingPaymentDto> InitiatePaymentAsync(Guid bookingId, Guid userId, InitiatePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingForCustomerAsync(bookingId, userId, cancellationToken);
        if (booking.Status != ServiceRequestStatus.AwaitingPayment)
            throw new ConflictException("Booking is not awaiting payment.");

        var payment = await _repository.GetPaymentByBookingIdAsync(bookingId, cancellationToken);
        if (payment is { Status: PaymentStatus.Completed })
            throw new ConflictException("Payment is already completed and cannot be overwritten.");

        // Return current open attempt checkout details without creating a duplicate session.
        if (payment is { Status: PaymentStatus.Processing or PaymentStatus.AwaitingGatewayConfirmation }
            && !string.IsNullOrWhiteSpace(payment.GatewaySessionId ?? payment.TransactionReference))
        {
            return MapPaymentDto(
                payment,
                payment.GatewaySessionId ?? payment.TransactionReference,
                checkoutUrl: null,
                provider: payment.PaymentProvider);
        }

        if (payment is null)
        {
            payment = new BookingPayment
            {
                ServiceRequestId = bookingId,
                PayerUserId = userId,
                PayeeUserId = booking.CraftsmanId,
                Amount = booking.EstimatedPrice,
                Currency = "SAR",
                Status = PaymentStatus.Pending,
                PaymentMethod = dto.PaymentMethod,
                PaymentProvider = _paymentGateway.ProviderName,
            };
            await _repository.AddPaymentAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (payment.Status is PaymentStatus.Failed or PaymentStatus.Cancelled or PaymentStatus.Expired or PaymentStatus.Pending)
        {
            payment.PaymentMethod = dto.PaymentMethod;
            payment.FailureReason = null;
            payment.FailedAt = null;
            _unitOfWork.Repository<BookingPayment>().Update(payment);
        }
        else if (payment.Status is not (PaymentStatus.Processing or PaymentStatus.AwaitingGatewayConfirmation))
        {
            throw new ConflictException("Payment cannot be initiated in the current state.");
        }

        var attemptNumber = await _repository.GetNextPaymentAttemptNumberAsync(payment.Id, cancellationToken);
        var attempt = new BookingPaymentAttempt
        {
            BookingPaymentId = payment.Id,
            PaymentProvider = _paymentGateway.ProviderName,
            AttemptNumber = attemptNumber,
            Status = PaymentStatus.Pending,
            Amount = payment.Amount,
            Currency = payment.Currency,
            RequestDate = DateTime.UtcNow,
        };
        await _repository.AddPaymentAttemptAsync(attempt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var customer = await _unitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken);
        var session = await _paymentGateway.CreateSessionAsync(new PaymentSessionRequest
        {
            PaymentId = payment.Id,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Description = $"Booking {booking.BookingReference}",
            CustomerEmail = customer?.Email ?? string.Empty,
        }, cancellationToken);

        attempt.Status = PaymentStatus.Processing;
        attempt.GatewaySessionId = session.GatewaySessionId ?? session.SessionId;
        attempt.GatewayTransactionId = session.SessionId;
        attempt.PaymentProvider = session.Provider;
        attempt.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<BookingPaymentAttempt>().Update(attempt);

        payment.Status = PaymentStatus.Processing;
        payment.PaymentProvider = session.Provider;
        payment.CurrentAttemptId = attempt.Id;
        payment.GatewaySessionId = session.GatewaySessionId ?? session.SessionId;
        payment.GatewayTransactionId = session.SessionId;
        payment.TransactionReference = session.SessionId;
        _unitOfWork.Repository<BookingPayment>().Update(payment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapPaymentDto(payment, session.SessionId, session.CheckoutUrl, session.Provider);
    }

    public async Task<BookingDto> ConfirmPaymentAsync(Guid bookingId, Guid userId, ConfirmPaymentDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingForCustomerAsync(bookingId, userId, cancellationToken);
        if (booking.Status != ServiceRequestStatus.AwaitingPayment)
            throw new ConflictException("Booking is not awaiting payment.");

        var payment = await _repository.GetPaymentByBookingIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException("Payment not found.");

        if (payment.Status == PaymentStatus.Completed)
            return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));

        // Clients are not the completion authority. For Development only, allow confirm-after-checkout
        // so local/CI flows work. Production gateways finalize via webhook + VerifyAsync.
        if (!string.Equals(_paymentGateway.ProviderName, "Development", StringComparison.OrdinalIgnoreCase))
        {
            payment.Status = PaymentStatus.AwaitingGatewayConfirmation;
            _unitOfWork.Repository<BookingPayment>().Update(payment);
            if (payment.CurrentAttemptId is Guid attemptId)
            {
                var attempt = payment.Attempts.FirstOrDefault(a => a.Id == attemptId)
                    ?? await _unitOfWork.Repository<BookingPaymentAttempt>().GetByIdAsync(attemptId, cancellationToken);
                if (attempt is not null
                    && attempt.Status is PaymentStatus.Processing or PaymentStatus.Pending)
                {
                    attempt.Status = PaymentStatus.AwaitingGatewayConfirmation;
                    attempt.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<BookingPaymentAttempt>().Update(attempt);
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new ConflictException(
                "Payment completion is confirmed by the payment gateway webhook. Refresh booking status shortly.");
        }

        if (payment.Status is not (PaymentStatus.Pending or PaymentStatus.Processing or PaymentStatus.AwaitingGatewayConfirmation))
            throw new ConflictException("Payment cannot be confirmed.");

        return await FinalizePaymentAsync(booking, payment, dto.TransactionReference, userId, webhookEventId: null, cancellationToken);
    }

    public async Task<BookingDto> ConfirmPaymentFromWebhookAsync(string transactionReference, CancellationToken cancellationToken = default)
        => await ConfirmPaymentFromWebhookAsync(transactionReference, webhookEventId: null, cancellationToken);

    public async Task<BookingDto> ConfirmPaymentFromWebhookAsync(
        string transactionReference, string? webhookEventId, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(webhookEventId))
        {
            var existingEvent = await _repository.GetPaymentAttemptByWebhookEventIdAsync(webhookEventId, cancellationToken);
            if (existingEvent is not null)
            {
                return MapToDto(await ReloadBookingAsync(existingEvent.BookingPayment.ServiceRequestId, cancellationToken));
            }
        }

        var attempt = await _repository.GetPaymentAttemptBySessionOrTransactionAsync(transactionReference, cancellationToken);
        BookingPayment payment;
        if (attempt is not null)
        {
            payment = attempt.BookingPayment;
        }
        else
        {
            payment = await _repository.GetPaymentByTransactionReferenceAsync(transactionReference, cancellationToken)
                ?? throw new NotFoundException("Payment not found for transaction reference.");
        }

        if (payment.Status == PaymentStatus.Completed)
            return MapToDto(await ReloadBookingAsync(payment.ServiceRequestId, cancellationToken));

        var booking = await _repository.GetByIdAsync(payment.ServiceRequestId, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Booking not found.");

        if (booking.Status != ServiceRequestStatus.AwaitingPayment)
            throw new ConflictException("Booking is not awaiting payment.");

        if (payment.Status is not (
            PaymentStatus.Pending or PaymentStatus.Processing or PaymentStatus.AwaitingGatewayConfirmation))
            throw new ConflictException("Payment cannot be confirmed.");

        return await FinalizePaymentAsync(
            booking, payment, transactionReference, payment.PayerUserId, webhookEventId, cancellationToken);
    }

    private async Task<BookingDto> FinalizePaymentAsync(
        ServiceRequest booking,
        BookingPayment payment,
        string transactionReference,
        Guid actorUserId,
        string? webhookEventId,
        CancellationToken cancellationToken)
    {
        if (payment.Status == PaymentStatus.Completed)
            return MapToDto(await ReloadBookingAsync(booking.Id, cancellationToken));

        if (await _repository.IsSlotBookedAsync(booking.CraftsmanId, booking.ScheduledAt, booking.SlotEnd, booking.Id, cancellationToken))
            throw new ConflictException("Time slot was booked by another customer.");

        var verification = await _paymentGateway.VerifyAsync(
            transactionReference, payment.Amount, payment.Currency, cancellationToken);

        if (!verification.IsSuccessful)
        {
            var failReason = verification.FailureReason ?? "Payment verification failed.";
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = failReason;
            payment.FailedAt = DateTime.UtcNow;
            _unitOfWork.Repository<BookingPayment>().Update(payment);

            var failedAttempt = payment.Attempts.FirstOrDefault(a =>
                    a.GatewayTransactionId == transactionReference
                    || a.GatewaySessionId == transactionReference
                    || a.Id == payment.CurrentAttemptId)
                ?? payment.CurrentAttempt;
            if (failedAttempt is not null)
            {
                failedAttempt.Status = PaymentStatus.Failed;
                failedAttempt.FailureReason = failReason.Length > 1000 ? failReason[..1000] : failReason;
                failedAttempt.FailedDate = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(webhookEventId))
                    failedAttempt.WebhookEventId ??= webhookEventId;
                failedAttempt.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<BookingPaymentAttempt>().Update(failedAttempt);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new ConflictException(failReason);
        }

        if (payment.Attempts.Any(a => a.Status == PaymentStatus.Completed && a.Id != payment.CurrentAttemptId))
            throw new ConflictException("A completed payment attempt already exists for this booking.");

        var completedRef = verification.TransactionReference ?? transactionReference;
        var now = DateTime.UtcNow;

        var activeAttempt = payment.Attempts.FirstOrDefault(a =>
                a.GatewayTransactionId == transactionReference
                || a.GatewaySessionId == transactionReference
                || a.Id == payment.CurrentAttemptId)
            ?? payment.CurrentAttempt;

        if (activeAttempt is null)
            throw new ConflictException("No payment attempt found to complete.");

        if (activeAttempt.Status == PaymentStatus.Completed)
            return MapToDto(await ReloadBookingAsync(booking.Id, cancellationToken));

        activeAttempt.Status = PaymentStatus.Completed;
        activeAttempt.GatewayTransactionId = completedRef;
        activeAttempt.CompletedDate = now;
        activeAttempt.FailedDate = null;
        activeAttempt.FailureReason = null;
        if (!string.IsNullOrWhiteSpace(webhookEventId))
            activeAttempt.WebhookEventId = webhookEventId;
        activeAttempt.UpdatedAt = now;
        _unitOfWork.Repository<BookingPaymentAttempt>().Update(activeAttempt);

        payment.Status = PaymentStatus.Completed;
        payment.TransactionReference = completedRef;
        payment.GatewayTransactionId = completedRef;
        payment.GatewaySessionId = activeAttempt.GatewaySessionId;
        payment.CurrentAttemptId = activeAttempt.Id;
        payment.PaymentProvider = activeAttempt.PaymentProvider;
        payment.PaidAt = now;
        payment.FailureReason = null;
        payment.FailedAt = null;
        _unitOfWork.Repository<BookingPayment>().Update(payment);

        await TransitionAsync(booking, ServiceRequestStatus.PaymentConfirmed, actorUserId, "Payment confirmed", cancellationToken);
        await TransitionAsync(booking, ServiceRequestStatus.PendingCraftsmanConfirmation, actorUserId, "Awaiting craftsman confirmation", cancellationToken);

        await _repository.AddSlotReservationAsync(new BookingSlotReservation
        {
            CraftsmanId = booking.CraftsmanId,
            ServiceRequestId = booking.Id,
            SlotStart = booking.ScheduledAt,
            SlotEnd = booking.SlotEnd,
            IsActive = true,
        }, cancellationToken);

        await NotifyAsync(booking.CraftsmanId, "New Booking Request", "طلب حجز جديد",
            $"New booking {booking.BookingReference} requires your confirmation.",
            $"حجز جديد {booking.BookingReference} يتطلب تأكيدك.",
            "BookingConfirmation", booking.Id, cancellationToken);

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(booking.Id, cancellationToken));
    }

    public async Task<BookingDto> AcceptBookingAsync(Guid bookingId, Guid craftsmanId, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingForCraftsmanAsync(bookingId, craftsmanId, cancellationToken);
        if (booking.Status != ServiceRequestStatus.PendingCraftsmanConfirmation)
            throw new ConflictException("Booking is not awaiting craftsman confirmation.");
        await TransitionAsync(booking, ServiceRequestStatus.Confirmed, craftsmanId, "Craftsman accepted booking", cancellationToken);

        await NotifyAsync(booking.CustomerId, "Booking Confirmed", "تم تأكيد الحجز",
            $"Your booking {booking.BookingReference} has been confirmed.",
            $"تم تأكيد حجزك {booking.BookingReference}.",
            "BookingConfirmed", bookingId, cancellationToken);

        await NotifyAsync(booking.CraftsmanId, "Booking Confirmed", "تم تأكيد الحجز",
            $"You confirmed booking {booking.BookingReference}.",
            $"لقد أكدت الحجز {booking.BookingReference}.",
            "BookingConfirmed", bookingId, cancellationToken);

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));
    }

    public async Task<BookingDto> RejectBookingAsync(Guid bookingId, Guid craftsmanId, RejectBookingDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingForCraftsmanAsync(bookingId, craftsmanId, cancellationToken);
        if (booking.Status != ServiceRequestStatus.PendingCraftsmanConfirmation)
            throw new ConflictException("Booking is not awaiting craftsman confirmation.");
        booking.RejectionReason = dto.Reason;
        await TransitionAsync(booking, ServiceRequestStatus.Rejected, craftsmanId, dto.Reason, cancellationToken);
        await _repository.ReleaseSlotReservationAsync(bookingId, cancellationToken);

        await NotifyAsync(booking.CustomerId, "Booking Rejected", "تم رفض الحجز",
            $"Your booking {booking.BookingReference} was rejected. You can book another time.",
            $"تم رفض حجزك {booking.BookingReference}. يمكنك حجز وقت آخر.",
            "BookingRejected", bookingId, cancellationToken);

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));
    }

    public async Task<BookingDto> CancelBookingAsync(Guid bookingId, Guid userId, string role, CancelBookingDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await GetAuthorizedBookingAsync(bookingId, userId, role, cancellationToken);
        booking.CancellationReason = dto.Reason;
        await TransitionAsync(booking, ServiceRequestStatus.Cancelled, userId, dto.Reason, cancellationToken);
        await _repository.ReleaseSlotReservationAsync(bookingId, cancellationToken);

        var otherParty = role == "Customer" ? booking.CraftsmanId : booking.CustomerId;
        await NotifyAsync(otherParty, "Booking Cancelled", "تم إلغاء الحجز",
            $"Booking {booking.BookingReference} was cancelled.",
            $"تم إلغاء الحجز {booking.BookingReference}.",
            "BookingCancelled", bookingId, cancellationToken);

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));
    }

    public async Task<BookingDto> CompleteBookingAsync(Guid bookingId, Guid craftsmanId, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingForCraftsmanAsync(bookingId, craftsmanId, cancellationToken);
        if (booking.Status != ServiceRequestStatus.Confirmed)
            throw new ConflictException("Only confirmed bookings can be completed.");
        booking.CompletedAt = DateTime.UtcNow;
        booking.FinalPrice = booking.EstimatedPrice;
        await TransitionAsync(booking, ServiceRequestStatus.Completed, craftsmanId, "Service completed", cancellationToken);

        await NotifyAsync(booking.CustomerId, "Booking Completed", "اكتمل الحجز",
            $"Your booking {booking.BookingReference} is complete.",
            $"اكتمل حجزك {booking.BookingReference}.",
            "BookingCompleted", bookingId, cancellationToken);

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));
    }

    public async Task<BookingDto> MarkNoShowAsync(Guid bookingId, Guid craftsmanId, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingForCraftsmanAsync(bookingId, craftsmanId, cancellationToken);
        if (booking.Status != ServiceRequestStatus.Confirmed)
            throw new ConflictException("Only confirmed bookings can be marked as no-show.");
        await TransitionAsync(booking, ServiceRequestStatus.NoShow, craftsmanId, "Customer no-show", cancellationToken);
        await _repository.ReleaseSlotReservationAsync(bookingId, cancellationToken);

        await NotifyAsync(booking.CustomerId, "No Show Recorded", "تسجيل عدم حضور",
            $"No-show recorded for booking {booking.BookingReference}.",
            $"تم تسجيل عدم حضور للحجز {booking.BookingReference}.",
            "BookingNoShow", bookingId, cancellationToken);

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));
    }

    public async Task<BookingDto> RescheduleBookingAsync(Guid bookingId, Guid userId, string role, RescheduleBookingDto dto, CancellationToken cancellationToken = default)
    {
        var booking = await GetAuthorizedBookingAsync(bookingId, userId, role, cancellationToken);
        var service = await _unitOfWork.Repository<Service>().GetByIdAsync(booking.ServiceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        var newEnd = dto.NewScheduledAt.AddMinutes(service.EstimatedDurationMinutes);
        if (await _repository.IsSlotBookedAsync(booking.CraftsmanId, dto.NewScheduledAt, newEnd, bookingId, cancellationToken))
            throw new ConflictException("Selected time slot is not available.");

        await _repository.ReleaseSlotReservationAsync(bookingId, cancellationToken);
        booking.ScheduledAt = dto.NewScheduledAt;
        booking.SlotEnd = newEnd;
        booking.RescheduledFromId = booking.Id;
        await TransitionAsync(booking, ServiceRequestStatus.Rescheduled, userId, dto.Reason ?? "Rescheduled", cancellationToken);
        await TransitionAsync(booking, ServiceRequestStatus.AwaitingPayment, userId, "Rescheduled, awaiting payment", cancellationToken);

        booking.PaymentDueAt = DateTime.UtcNow.AddMinutes(30);
        booking.ExpiresAt = DateTime.UtcNow.AddMinutes(30);

        _repository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(await ReloadBookingAsync(bookingId, cancellationToken));
    }

    public async Task<BookingDto> GetBookingAsync(Guid bookingId, Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var booking = await GetAuthorizedBookingAsync(bookingId, userId, role, cancellationToken);
        return MapToDto(booking);
    }

    public async Task<PagedResult<BookingDto>> ListBookingsAsync(Guid userId, string role, BookingListQueryDto query, CancellationToken cancellationToken = default)
    {
        ServiceRequestStatus? status = ParseStatus(query.Status);
        Guid? customerId = role == "Customer" ? userId : null;
        Guid? craftsmanId = role == "Craftsman" ? userId : null;

        var (items, total) = await _repository.SearchAsync(customerId, craftsmanId, status, query.FromDate, query.ToDate, query.Page, query.PageSize, cancellationToken);
        return new PagedResult<BookingDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<PagedResult<BookingDto>> AdminListBookingsAsync(BookingListQueryDto query, CancellationToken cancellationToken = default)
    {
        var status = ParseStatus(query.Status);
        var (items, total) = await _repository.SearchAsync(null, null, status, query.FromDate, query.ToDate, query.Page, query.PageSize, cancellationToken);
        return new PagedResult<BookingDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<AdminBookingStatsDto> GetAdminStatsAsync(CancellationToken cancellationToken = default)
    {
        var all = await _unitOfWork.Repository<ServiceRequest>().GetAllAsync(cancellationToken);
        return new AdminBookingStatsDto
        {
            TotalBookings = all.Count,
            PendingPayment = all.Count(b => b.Status == ServiceRequestStatus.AwaitingPayment),
            AwaitingConfirmation = all.Count(b => b.Status == ServiceRequestStatus.PendingCraftsmanConfirmation),
            Confirmed = all.Count(b => b.Status == ServiceRequestStatus.Confirmed),
            Completed = all.Count(b => b.Status == ServiceRequestStatus.Completed),
            Cancelled = all.Count(b => b.Status == ServiceRequestStatus.Cancelled),
            Rejected = all.Count(b => b.Status == ServiceRequestStatus.Rejected)
        };
    }

    public async Task<BookingDto> AdminGetBookingAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await _repository.GetByIdAsync(bookingId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");
        return MapToDto(booking);
    }

    public async Task<PagedResult<NotificationDto>> GetNotificationsAsync(Guid userId, bool unreadOnly, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repository.GetNotificationsAsync(userId, unreadOnly, page, pageSize, cancellationToken);
        return new PagedResult<NotificationDto>
        {
            Items = items.Select(n => new NotificationDto
            {
                Id = n.Id,
                TitleEn = n.TitleEn,
                TitleAr = n.TitleAr,
                MessageEn = n.MessageEn,
                MessageAr = n.MessageAr,
                NotificationType = n.NotificationType,
                ReferenceId = n.ReferenceId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task MarkNotificationReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(notificationId, cancellationToken)
            ?? throw new NotFoundException("Notification not found.");
        if (notification.UserId != userId) throw new UnauthorizedException("Access denied.");
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        _unitOfWork.Repository<Notification>().Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<ServiceRequest> GetBookingForCustomerAsync(Guid bookingId, Guid userId, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(bookingId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");
        if (booking.CustomerId != userId) throw new UnauthorizedException("Access denied.");
        return booking;
    }

    private async Task<ServiceRequest> GetBookingForCraftsmanAsync(Guid bookingId, Guid craftsmanId, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(bookingId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");
        if (booking.CraftsmanId != craftsmanId) throw new UnauthorizedException("Access denied.");
        return booking;
    }

    private async Task<ServiceRequest> GetAuthorizedBookingAsync(Guid bookingId, Guid userId, string role, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(bookingId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Booking not found.");

        if (role == "Customer" && booking.CustomerId == userId) return booking;
        if (role == "Craftsman" && booking.CraftsmanId == userId) return booking;
        if (await _permissionService.UserHasPermissionAsync(userId, PermissionCodes.BookingsView, cancellationToken))
            return booking;

        throw new UnauthorizedException("Access denied.");
    }

    private async Task<ServiceRequest> ReloadBookingAsync(Guid bookingId, CancellationToken cancellationToken) =>
        await _repository.GetByIdAsync(bookingId, includeDetails: true, cancellationToken)
        ?? throw new NotFoundException("Booking not found.");

    private async Task TransitionAsync(ServiceRequest booking, ServiceRequestStatus to, Guid? userId, string? notes, CancellationToken cancellationToken)
    {
        BookingStateMachine.ValidateTransition(booking.Status, to);
        var old = booking.Status;
        booking.Status = to;
        await AddHistoryAsync(booking, old, to, userId, notes, cancellationToken);
    }

    private async Task AddHistoryAsync(ServiceRequest booking, ServiceRequestStatus? oldStatus, ServiceRequestStatus newStatus, Guid? userId, string? notes, CancellationToken cancellationToken)
    {
        await _repository.AddStatusHistoryAsync(new ServiceRequestStatusHistory
        {
            ServiceRequestId = booking.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedByUserId = userId,
            Notes = notes
        }, cancellationToken);
    }

    private async Task NotifyAsync(Guid userId, string titleEn, string titleAr, string msgEn, string msgAr, string type, Guid refId, CancellationToken cancellationToken)
    {
        await _repository.AddNotificationAsync(new Notification
        {
            UserId = userId,
            TitleEn = titleEn,
            TitleAr = titleAr,
            MessageEn = msgEn,
            MessageAr = msgAr,
            NotificationType = type,
            ReferenceId = refId
        }, cancellationToken);

        await _pushNotificationService.SendAsync(new Application.DTOs.Messaging.PushNotificationPayload
        {
            UserId = userId,
            TitleEn = titleEn,
            TitleAr = titleAr,
            MessageEn = msgEn,
            MessageAr = msgAr,
            NotificationType = type,
            ReferenceId = refId,
        }, cancellationToken);
    }

    private static string GenerateReference() => $"KHD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";

    private static ServiceRequestStatus? ParseStatus(string? status) =>
        string.IsNullOrWhiteSpace(status) ? null :
        Enum.TryParse<ServiceRequestStatus>(status, true, out var s) ? s : null;

    private static BookingPaymentDto MapPaymentDto(
        BookingPayment p,
        string? sessionId = null,
        string? checkoutUrl = null,
        string? provider = null) => new()
    {
        Id = p.Id,
        Amount = p.Amount,
        Currency = p.Currency,
        Status = p.Status.ToString(),
        PaymentMethod = p.PaymentMethod,
        TransactionReference = p.Status == PaymentStatus.Completed ? p.TransactionReference : null,
        SessionId = sessionId ?? (p.Status != PaymentStatus.Completed ? p.TransactionReference : null),
        CheckoutUrl = checkoutUrl,
        Provider = provider,
        PaidAt = p.PaidAt
    };

    private static BookingDto MapToDto(ServiceRequest b) => new()
    {
        Id = b.Id,
        BookingReference = b.BookingReference,
        ServiceId = b.ServiceId,
        ServiceName = b.Service?.NameEn ?? string.Empty,
        CustomerId = b.CustomerId,
        CustomerName = b.Customer?.Profile != null ? $"{b.Customer.Profile.FirstName} {b.Customer.Profile.LastName}" : string.Empty,
        CraftsmanId = b.CraftsmanId,
        CraftsmanName = b.Craftsman?.Profile != null ? $"{b.Craftsman.Profile.FirstName} {b.Craftsman.Profile.LastName}" : string.Empty,
        Status = b.Status.ToString(),
        Description = b.Description,
        ScheduledAt = b.ScheduledAt,
        SlotEnd = b.SlotEnd,
        CompletedAt = b.CompletedAt,
        PaymentDueAt = b.PaymentDueAt,
        ExpiresAt = b.ExpiresAt,
        EstimatedPrice = b.EstimatedPrice,
        FinalPrice = b.FinalPrice,
        Notes = b.Notes,
        RejectionReason = b.RejectionReason,
        CancellationReason = b.CancellationReason,
        CustomerRating = b.CustomerRating,
        CustomerReview = b.CustomerReview,
        Payment = b.Payment != null ? MapPaymentDto(b.Payment) : null,
        StatusHistory = b.StatusHistory?.Select(h => new BookingStatusHistoryDto
        {
            OldStatus = h.OldStatus?.ToString(),
            NewStatus = h.NewStatus.ToString(),
            Notes = h.Notes,
            CreatedAt = h.CreatedAt
        }).ToList() ?? new List<BookingStatusHistoryDto>()
    };
}
