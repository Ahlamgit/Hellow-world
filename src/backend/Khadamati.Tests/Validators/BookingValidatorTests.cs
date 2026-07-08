using FluentAssertions;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.Validators;
using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Tests.Validators;

public class BookingValidatorTests
{
    private readonly CreateBookingValidator _createValidator = new();

    [Fact]
    public async Task Create_WithValidData_ShouldPass()
    {
        var dto = new CreateBookingDto
        {
            ServiceId = Guid.NewGuid(),
            CraftsmanId = Guid.NewGuid(),
            ScheduledAt = DateTime.UtcNow.AddDays(1)
        };
        var result = await _createValidator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Create_WithPastDate_ShouldFail()
    {
        var dto = new CreateBookingDto
        {
            ServiceId = Guid.NewGuid(),
            CraftsmanId = Guid.NewGuid(),
            ScheduledAt = DateTime.UtcNow.AddDays(-1)
        };
        var result = await _createValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}

public class BookingStateMachineTests
{
    [Fact]
    public void Pending_To_AwaitingPayment_ShouldBeAllowed()
    {
        BookingStateMachine.CanTransition(ServiceRequestStatus.Pending, ServiceRequestStatus.AwaitingPayment).Should().BeTrue();
    }

    [Fact]
    public void Pending_To_Confirmed_ShouldBeDenied()
    {
        BookingStateMachine.CanTransition(ServiceRequestStatus.Pending, ServiceRequestStatus.Confirmed).Should().BeFalse();
    }

    [Fact]
    public void PaymentConfirmed_To_PendingCraftsmanConfirmation_ShouldBeAllowed()
    {
        BookingStateMachine.CanTransition(ServiceRequestStatus.PaymentConfirmed, ServiceRequestStatus.PendingCraftsmanConfirmation).Should().BeTrue();
    }
}
