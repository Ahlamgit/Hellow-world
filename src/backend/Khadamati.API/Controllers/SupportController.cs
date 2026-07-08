using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.DTOs.Support;
using Khadamati.Application.Features.Support;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>User complaints, support tickets, and reviews.</summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class SupportController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public SupportController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpPost("complaints")]
    [Authorize]
    [SwaggerOperation(Summary = "Submit a complaint")]
    public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintDto request, CancellationToken ct) =>
        Ok(ApiResponse<ComplaintDto>.Ok(await _mediator.Send(new CreateComplaintCommand(RequireUserId(), request), ct)));

    [HttpGet("complaints/mine")]
    [Authorize]
    public async Task<IActionResult> MyComplaints(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<ComplaintDto>>.Ok(await _mediator.Send(new GetMyComplaintsQuery(RequireUserId()), ct)));

    [HttpPost("support-tickets")]
    [Authorize]
    [SwaggerOperation(Summary = "Create a support ticket")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateSupportTicketDto request, CancellationToken ct) =>
        Ok(ApiResponse<SupportTicketDto>.Ok(await _mediator.Send(new CreateSupportTicketCommand(RequireUserId(), request), ct)));

    [HttpGet("support-tickets/mine")]
    [Authorize]
    public async Task<IActionResult> MyTickets(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<SupportTicketDto>>.Ok(await _mediator.Send(new GetMySupportTicketsQuery(RequireUserId()), ct)));

    [HttpPost("bookings/{id:guid}/review")]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Submit a review for a completed booking")]
    public async Task<IActionResult> SubmitReview(Guid id, [FromBody] SubmitReviewDto request, CancellationToken ct) =>
        Ok(ApiResponse<object>.Ok(await _mediator.Send(new SubmitBookingReviewCommand(id, RequireUserId(), request), ct)));

    [HttpGet("coupons/validate")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Validate a coupon code for an amount")]
    public async Task<IActionResult> ValidateCoupon([FromQuery] string code, [FromQuery] decimal amount, CancellationToken ct) =>
        Ok(ApiResponse<CouponValidationResultDto>.Ok(await _mediator.Send(new ValidateCouponQuery(code, amount), ct)));

    [HttpGet("advertisements")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get active advertisements by placement")]
    public async Task<IActionResult> GetAds([FromQuery] string placement = "HomePage", CancellationToken ct = default) =>
        Ok(ApiResponse<IReadOnlyList<AdvertisementDto>>.Ok(await _mediator.Send(new GetActiveAdsQuery(placement), ct)));
}
