using FluentValidation;
using Khadamati.Application.DTOs.Bookings;

namespace Khadamati.Application.Validators;

public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.CraftsmanId).NotEmpty();
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Scheduled time must be in the future.");
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class InitiatePaymentValidator : AbstractValidator<InitiatePaymentDto>
{
    private static readonly string[] ValidMethods = ["Card", "Mada", "ApplePay", "Cash"];

    public InitiatePaymentValidator()
    {
        RuleFor(x => x.PaymentMethod).NotEmpty()
            .Must(m => ValidMethods.Contains(m, StringComparer.OrdinalIgnoreCase));
    }
}

public class ConfirmPaymentValidator : AbstractValidator<ConfirmPaymentDto>
{
    public ConfirmPaymentValidator()
    {
        RuleFor(x => x.TransactionReference).NotEmpty().MaximumLength(200);
    }
}

public class RejectBookingValidator : AbstractValidator<RejectBookingDto>
{
    public RejectBookingValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class CancelBookingValidator : AbstractValidator<CancelBookingDto>
{
    public CancelBookingValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class RescheduleBookingValidator : AbstractValidator<RescheduleBookingDto>
{
    public RescheduleBookingValidator()
    {
        RuleFor(x => x.NewScheduledAt).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}

public class BookingListQueryValidator : AbstractValidator<BookingListQueryDto>
{
    public BookingListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
