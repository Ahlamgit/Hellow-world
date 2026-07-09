using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.Features.Bookings.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Customer and craftsman booking endpoints.</summary>
[ApiController]
[Route("api/v1/bookings")]
[Authorize]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public BookingsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>List craftsmen available for a service.</summary>
    [HttpGet("craftsmen")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get craftsmen for service")]
    public async Task<IActionResult> GetCraftsmen([FromQuery] Guid serviceId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCraftsmenForServiceQuery(serviceId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CraftsmanOptionDto>>.Ok(result));
    }

    /// <summary>List craftsmen near a GPS coordinate for a service.</summary>
    [HttpGet("craftsmen/nearby")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get nearby craftsmen for service")]
    public async Task<IActionResult> GetNearbyCraftsmen(
        [FromQuery] Guid serviceId,
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetNearbyCraftsmenForServiceQuery(serviceId, latitude, longitude, radiusKm), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CraftsmanOptionDto>>.Ok(result));
    }

    /// <summary>Get available time slots for a craftsman on a date (prevents double booking).</summary>
    [HttpGet("availability")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get available time slots")]
    public async Task<IActionResult> GetAvailability([FromQuery] Guid craftsmanId, [FromQuery] Guid serviceId, [FromQuery] DateTime date, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAvailableSlotsQuery(craftsmanId, serviceId, date), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TimeSlotDto>>.Ok(result));
    }

    /// <summary>Create a new booking (Customer).</summary>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Create booking")]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new CreateBookingCommand(userId, request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<BookingDto>.Ok(result, "Booking created."));
    }

    /// <summary>Get booking by ID.</summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get booking details")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var role = _currentUser.Role ?? "Customer";
        var result = await _mediator.Send(new GetBookingQuery(id, userId, role), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result));
    }

    /// <summary>List bookings for current user.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List my bookings")]
    public async Task<IActionResult> List([FromQuery] BookingListQueryDto query, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var role = _currentUser.Role ?? "Customer";
        var result = await _mediator.Send(new ListBookingsQuery(userId, role, query), cancellationToken);
        return Ok(ApiResponse<PagedResult<BookingDto>>.Ok(result));
    }

    /// <summary>Customer confirms booking details.</summary>
    [HttpPost("{id:guid}/confirm")]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Confirm booking", Description = "Moves booking to Awaiting Payment.")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmBookingDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new ConfirmBookingCommand(id, userId, request), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Please proceed to payment."));
    }

    /// <summary>Initiate payment for booking.</summary>
    [HttpPost("{id:guid}/payment")]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Initiate payment", Description = "Payment status becomes Pending.")]
    public async Task<IActionResult> InitiatePayment(Guid id, [FromBody] InitiatePaymentDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new InitiatePaymentCommand(id, userId, request), cancellationToken);
        return Ok(ApiResponse<BookingPaymentDto>.Ok(result, "Payment initiated."));
    }

    /// <summary>Confirm payment after gateway success.</summary>
    [HttpPost("{id:guid}/payment/confirm")]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Confirm payment", Description = "Reserves slot and notifies craftsman.")]
    public async Task<IActionResult> ConfirmPayment(Guid id, [FromBody] ConfirmPaymentDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new ConfirmPaymentCommand(id, userId, request), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Payment confirmed. Awaiting craftsman confirmation."));
    }

    /// <summary>Craftsman accepts booking.</summary>
    [HttpPost("{id:guid}/accept")]
    [Authorize(Roles = "Craftsman")]
    [SwaggerOperation(Summary = "Accept booking")]
    public async Task<IActionResult> Accept(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new AcceptBookingCommand(id, userId), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Booking confirmed."));
    }

    /// <summary>Craftsman rejects booking.</summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Craftsman")]
    [SwaggerOperation(Summary = "Reject booking")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectBookingDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new RejectBookingCommand(id, userId, request), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Booking rejected."));
    }

    /// <summary>Cancel booking.</summary>
    [HttpPost("{id:guid}/cancel")]
    [SwaggerOperation(Summary = "Cancel booking")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelBookingDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var role = _currentUser.Role ?? "Customer";
        var result = await _mediator.Send(new CancelBookingCommand(id, userId, role, request), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Booking cancelled."));
    }

    /// <summary>Mark booking as completed (Craftsman).</summary>
    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "Craftsman")]
    [SwaggerOperation(Summary = "Complete booking")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new CompleteBookingCommand(id, userId), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Booking completed."));
    }

    /// <summary>Mark customer as no-show (Craftsman).</summary>
    [HttpPost("{id:guid}/no-show")]
    [Authorize(Roles = "Craftsman")]
    [SwaggerOperation(Summary = "Mark no-show")]
    public async Task<IActionResult> NoShow(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new MarkNoShowCommand(id, userId), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result));
    }

    /// <summary>Reschedule booking to a new time.</summary>
    [HttpPost("{id:guid}/reschedule")]
    [SwaggerOperation(Summary = "Reschedule booking")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleBookingDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var role = _currentUser.Role ?? "Customer";
        var result = await _mediator.Send(new RescheduleBookingCommand(id, userId, role, request), cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(result, "Booking rescheduled. Please complete payment."));
    }
}

/// <summary>Administrator booking monitoring.</summary>
[ApiController]
[Route("api/v1/admin/bookings")]
[Authorize]
[Produces("application/json")]
public class AdminBookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminBookingsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [HasPermission(PermissionCodes.BookingsView)]
    [SwaggerOperation(Summary = "Monitor all bookings")]
    public async Task<IActionResult> List([FromQuery] BookingListQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AdminListBookingsQuery(query), cancellationToken);
        return Ok(ApiResponse<PagedResult<BookingDto>>.Ok(result));
    }

    [HttpGet("stats")]
    [HasPermission(PermissionCodes.BookingsView)]
    [SwaggerOperation(Summary = "Booking statistics dashboard")]
    public async Task<IActionResult> Stats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAdminBookingStatsQuery(), cancellationToken);
        return Ok(ApiResponse<AdminBookingStatsDto>.Ok(result));
    }
}

/// <summary>User notifications for booking events.</summary>
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get notifications")]
    public async Task<IActionResult> List([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var result = await _mediator.Send(new GetNotificationsQuery(userId, unreadOnly, page, pageSize), cancellationToken);
        return Ok(ApiResponse<PagedResult<NotificationDto>>.Ok(result));
    }

    [HttpPost("{id:guid}/read")]
    [SwaggerOperation(Summary = "Mark notification as read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        await _mediator.Send(new MarkNotificationReadCommand(id, userId), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Notification marked as read."));
    }
}
