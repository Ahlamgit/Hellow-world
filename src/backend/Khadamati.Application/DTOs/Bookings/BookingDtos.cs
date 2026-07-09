namespace Khadamati.Application.DTOs.Bookings;

public class CraftsmanOptionDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public decimal Rating { get; set; }
    public int TotalReviews { get; set; }
    public int CompletedJobs { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public double? DistanceKm { get; set; }
}

public class TimeSlotDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsAvailable { get; set; }
}

public class CreateBookingDto
{
    public Guid ServiceId { get; set; }
    public Guid CraftsmanId { get; set; }
    public Guid? AddressId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

public class ConfirmBookingDto
{
    public string? Notes { get; set; }
}

public class InitiatePaymentDto
{
    public string PaymentMethod { get; set; } = "Card";
}

public class ConfirmPaymentDto
{
    public string TransactionReference { get; set; } = string.Empty;
}

public class RejectBookingDto
{
    public string Reason { get; set; } = string.Empty;
}

public class CancelBookingDto
{
    public string Reason { get; set; } = string.Empty;
}

public class RescheduleBookingDto
{
    public DateTime NewScheduledAt { get; set; }
    public string? Reason { get; set; }
}

public class BookingPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "SAR";
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? SessionId { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? Provider { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class BookingStatusHistoryDto
{
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingDto
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid CraftsmanId { get; set; }
    public string CraftsmanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime SlotEnd { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? PaymentDueAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public decimal EstimatedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public string? CancellationReason { get; set; }
    public int? CustomerRating { get; set; }
    public string? CustomerReview { get; set; }
    public BookingPaymentDto? Payment { get; set; }
    public List<BookingStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class BookingListQueryDto
{
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string MessageEn { get; set; } = string.Empty;
    public string MessageAr { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminBookingStatsDto
{
    public int TotalBookings { get; set; }
    public int PendingPayment { get; set; }
    public int AwaitingConfirmation { get; set; }
    public int Confirmed { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int Rejected { get; set; }
}
