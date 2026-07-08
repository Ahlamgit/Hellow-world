using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.DTOs.Support;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Support;

public record ValidateCouponQuery(string Code, decimal Amount) : IRequest<CouponValidationResultDto>;
public record CreateComplaintCommand(Guid UserId, CreateComplaintDto Request) : IRequest<ComplaintDto>;
public record GetMyComplaintsQuery(Guid UserId) : IRequest<IReadOnlyList<ComplaintDto>>;
public record CreateSupportTicketCommand(Guid UserId, CreateSupportTicketDto Request) : IRequest<SupportTicketDto>;
public record GetMySupportTicketsQuery(Guid UserId) : IRequest<IReadOnlyList<SupportTicketDto>>;
public record SubmitBookingReviewCommand(Guid BookingId, Guid CustomerId, SubmitReviewDto Request) : IRequest<Application.DTOs.Bookings.BookingDto>;
public record GetActiveAdsQuery(string Placement) : IRequest<IReadOnlyList<AdvertisementDto>>;

public class ValidateCouponQueryHandler : IRequestHandler<ValidateCouponQuery, CouponValidationResultDto>
{
    private readonly ICouponService _coupons;
    public ValidateCouponQueryHandler(ICouponService coupons) => _coupons = coupons;
    public Task<CouponValidationResultDto> Handle(ValidateCouponQuery request, CancellationToken ct) =>
        _coupons.ValidateAsync(request.Code, request.Amount, ct);
}

public class CreateComplaintCommandHandler : IRequestHandler<CreateComplaintCommand, ComplaintDto>
{
    private readonly ISupportService _support;
    public CreateComplaintCommandHandler(ISupportService support) => _support = support;
    public Task<ComplaintDto> Handle(CreateComplaintCommand request, CancellationToken ct) =>
        _support.CreateComplaintAsync(request.UserId, request.Request, ct);
}

public class GetMyComplaintsQueryHandler : IRequestHandler<GetMyComplaintsQuery, IReadOnlyList<ComplaintDto>>
{
    private readonly ISupportService _support;
    public GetMyComplaintsQueryHandler(ISupportService support) => _support = support;
    public Task<IReadOnlyList<ComplaintDto>> Handle(GetMyComplaintsQuery request, CancellationToken ct) =>
        _support.GetMyComplaintsAsync(request.UserId, ct);
}

public class CreateSupportTicketCommandHandler : IRequestHandler<CreateSupportTicketCommand, SupportTicketDto>
{
    private readonly ISupportService _support;
    public CreateSupportTicketCommandHandler(ISupportService support) => _support = support;
    public Task<SupportTicketDto> Handle(CreateSupportTicketCommand request, CancellationToken ct) =>
        _support.CreateTicketAsync(request.UserId, request.Request, ct);
}

public class GetMySupportTicketsQueryHandler : IRequestHandler<GetMySupportTicketsQuery, IReadOnlyList<SupportTicketDto>>
{
    private readonly ISupportService _support;
    public GetMySupportTicketsQueryHandler(ISupportService support) => _support = support;
    public Task<IReadOnlyList<SupportTicketDto>> Handle(GetMySupportTicketsQuery request, CancellationToken ct) =>
        _support.GetMyTicketsAsync(request.UserId, ct);
}

public class SubmitBookingReviewCommandHandler : IRequestHandler<SubmitBookingReviewCommand, Application.DTOs.Bookings.BookingDto>
{
    private readonly ISupportService _support;
    public SubmitBookingReviewCommandHandler(ISupportService support) => _support = support;
    public Task<Application.DTOs.Bookings.BookingDto> Handle(SubmitBookingReviewCommand request, CancellationToken ct) =>
        _support.SubmitReviewAsync(request.BookingId, request.CustomerId, request.Request, ct);
}

public class GetActiveAdsQueryHandler : IRequestHandler<GetActiveAdsQuery, IReadOnlyList<AdvertisementDto>>
{
    private readonly ISupportService _support;
    public GetActiveAdsQueryHandler(ISupportService support) => _support = support;
    public Task<IReadOnlyList<AdvertisementDto>> Handle(GetActiveAdsQuery request, CancellationToken ct) =>
        _support.GetActiveAdsAsync(request.Placement, ct);
}
