using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Bookings;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Bookings.Commands;

public record GetCraftsmenForServiceQuery(Guid ServiceId) : IRequest<IReadOnlyList<CraftsmanOptionDto>>;
public record GetNearbyCraftsmenForServiceQuery(Guid ServiceId, double Latitude, double Longitude, double RadiusKm = 25) : IRequest<IReadOnlyList<CraftsmanOptionDto>>;
public record GetAvailableSlotsQuery(Guid CraftsmanId, Guid ServiceId, DateTime Date) : IRequest<IReadOnlyList<TimeSlotDto>>;
public record CreateBookingCommand(Guid CustomerId, CreateBookingDto Request) : IRequest<BookingDto>;
public record ConfirmBookingCommand(Guid BookingId, Guid UserId, ConfirmBookingDto Request) : IRequest<BookingDto>;
public record InitiatePaymentCommand(Guid BookingId, Guid UserId, InitiatePaymentDto Request) : IRequest<BookingPaymentDto>;
public record ConfirmPaymentCommand(Guid BookingId, Guid UserId, ConfirmPaymentDto Request) : IRequest<BookingDto>;
public record AcceptBookingCommand(Guid BookingId, Guid CraftsmanId) : IRequest<BookingDto>;
public record RejectBookingCommand(Guid BookingId, Guid CraftsmanId, RejectBookingDto Request) : IRequest<BookingDto>;
public record CancelBookingCommand(Guid BookingId, Guid UserId, string Role, CancelBookingDto Request) : IRequest<BookingDto>;
public record CompleteBookingCommand(Guid BookingId, Guid CraftsmanId) : IRequest<BookingDto>;
public record MarkNoShowCommand(Guid BookingId, Guid CraftsmanId) : IRequest<BookingDto>;
public record RescheduleBookingCommand(Guid BookingId, Guid UserId, string Role, RescheduleBookingDto Request) : IRequest<BookingDto>;
public record GetBookingQuery(Guid BookingId, Guid UserId, string Role) : IRequest<BookingDto>;
public record ListBookingsQuery(Guid UserId, string Role, BookingListQueryDto Query) : IRequest<PagedResult<BookingDto>>;
public record AdminListBookingsQuery(BookingListQueryDto Query) : IRequest<PagedResult<BookingDto>>;
public record GetAdminBookingStatsQuery() : IRequest<AdminBookingStatsDto>;
public record GetNotificationsQuery(Guid UserId, bool UnreadOnly, int Page, int PageSize) : IRequest<PagedResult<NotificationDto>>;
public record MarkNotificationReadCommand(Guid NotificationId, Guid UserId) : IRequest<Unit>;

public class GetCraftsmenForServiceQueryHandler : IRequestHandler<GetCraftsmenForServiceQuery, IReadOnlyList<CraftsmanOptionDto>>
{
    private readonly IBookingService _service;
    public GetCraftsmenForServiceQueryHandler(IBookingService service) => _service = service;
    public Task<IReadOnlyList<CraftsmanOptionDto>> Handle(GetCraftsmenForServiceQuery request, CancellationToken ct) =>
        _service.GetCraftsmenForServiceAsync(request.ServiceId, ct);
}

public class GetNearbyCraftsmenForServiceQueryHandler : IRequestHandler<GetNearbyCraftsmenForServiceQuery, IReadOnlyList<CraftsmanOptionDto>>
{
    private readonly IBookingService _service;
    public GetNearbyCraftsmenForServiceQueryHandler(IBookingService service) => _service = service;
    public Task<IReadOnlyList<CraftsmanOptionDto>> Handle(GetNearbyCraftsmenForServiceQuery request, CancellationToken ct) =>
        _service.GetNearbyCraftsmenForServiceAsync(request.ServiceId, request.Latitude, request.Longitude, request.RadiusKm, ct);
}

public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, IReadOnlyList<TimeSlotDto>>
{
    private readonly IBookingService _service;
    public GetAvailableSlotsQueryHandler(IBookingService service) => _service = service;
    public Task<IReadOnlyList<TimeSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken ct) =>
        _service.GetAvailableSlotsAsync(request.CraftsmanId, request.ServiceId, request.Date, ct);
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingService _service;
    public CreateBookingCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken ct) =>
        _service.CreateBookingAsync(request.CustomerId, request.Request, ct);
}

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, BookingDto>
{
    private readonly IBookingService _service;
    public ConfirmBookingCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(ConfirmBookingCommand request, CancellationToken ct) =>
        _service.ConfirmBookingAsync(request.BookingId, request.UserId, request.Request, ct);
}

public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, BookingPaymentDto>
{
    private readonly IBookingService _service;
    public InitiatePaymentCommandHandler(IBookingService service) => _service = service;
    public Task<BookingPaymentDto> Handle(InitiatePaymentCommand request, CancellationToken ct) =>
        _service.InitiatePaymentAsync(request.BookingId, request.UserId, request.Request, ct);
}

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, BookingDto>
{
    private readonly IBookingService _service;
    public ConfirmPaymentCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(ConfirmPaymentCommand request, CancellationToken ct) =>
        _service.ConfirmPaymentAsync(request.BookingId, request.UserId, request.Request, ct);
}

public class AcceptBookingCommandHandler : IRequestHandler<AcceptBookingCommand, BookingDto>
{
    private readonly IBookingService _service;
    public AcceptBookingCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(AcceptBookingCommand request, CancellationToken ct) =>
        _service.AcceptBookingAsync(request.BookingId, request.CraftsmanId, ct);
}

public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, BookingDto>
{
    private readonly IBookingService _service;
    public RejectBookingCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(RejectBookingCommand request, CancellationToken ct) =>
        _service.RejectBookingAsync(request.BookingId, request.CraftsmanId, request.Request, ct);
}

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, BookingDto>
{
    private readonly IBookingService _service;
    public CancelBookingCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(CancelBookingCommand request, CancellationToken ct) =>
        _service.CancelBookingAsync(request.BookingId, request.UserId, request.Role, request.Request, ct);
}

public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, BookingDto>
{
    private readonly IBookingService _service;
    public CompleteBookingCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(CompleteBookingCommand request, CancellationToken ct) =>
        _service.CompleteBookingAsync(request.BookingId, request.CraftsmanId, ct);
}

public class MarkNoShowCommandHandler : IRequestHandler<MarkNoShowCommand, BookingDto>
{
    private readonly IBookingService _service;
    public MarkNoShowCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(MarkNoShowCommand request, CancellationToken ct) =>
        _service.MarkNoShowAsync(request.BookingId, request.CraftsmanId, ct);
}

public class RescheduleBookingCommandHandler : IRequestHandler<RescheduleBookingCommand, BookingDto>
{
    private readonly IBookingService _service;
    public RescheduleBookingCommandHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(RescheduleBookingCommand request, CancellationToken ct) =>
        _service.RescheduleBookingAsync(request.BookingId, request.UserId, request.Role, request.Request, ct);
}

public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, BookingDto>
{
    private readonly IBookingService _service;
    public GetBookingQueryHandler(IBookingService service) => _service = service;
    public Task<BookingDto> Handle(GetBookingQuery request, CancellationToken ct) =>
        _service.GetBookingAsync(request.BookingId, request.UserId, request.Role, ct);
}

public class ListBookingsQueryHandler : IRequestHandler<ListBookingsQuery, PagedResult<BookingDto>>
{
    private readonly IBookingService _service;
    public ListBookingsQueryHandler(IBookingService service) => _service = service;
    public Task<PagedResult<BookingDto>> Handle(ListBookingsQuery request, CancellationToken ct) =>
        _service.ListBookingsAsync(request.UserId, request.Role, request.Query, ct);
}

public class AdminListBookingsQueryHandler : IRequestHandler<AdminListBookingsQuery, PagedResult<BookingDto>>
{
    private readonly IBookingService _service;
    public AdminListBookingsQueryHandler(IBookingService service) => _service = service;
    public Task<PagedResult<BookingDto>> Handle(AdminListBookingsQuery request, CancellationToken ct) =>
        _service.AdminListBookingsAsync(request.Query, ct);
}

public class GetAdminBookingStatsQueryHandler : IRequestHandler<GetAdminBookingStatsQuery, AdminBookingStatsDto>
{
    private readonly IBookingService _service;
    public GetAdminBookingStatsQueryHandler(IBookingService service) => _service = service;
    public Task<AdminBookingStatsDto> Handle(GetAdminBookingStatsQuery request, CancellationToken ct) =>
        _service.GetAdminStatsAsync(ct);
}

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PagedResult<NotificationDto>>
{
    private readonly IBookingService _service;
    public GetNotificationsQueryHandler(IBookingService service) => _service = service;
    public Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken ct) =>
        _service.GetNotificationsAsync(request.UserId, request.UnreadOnly, request.Page, request.PageSize, ct);
}

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Unit>
{
    private readonly IBookingService _service;
    public MarkNotificationReadCommandHandler(IBookingService service) => _service = service;
    public async Task<Unit> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        await _service.MarkNotificationReadAsync(request.NotificationId, request.UserId, ct);
        return Unit.Value;
    }
}
