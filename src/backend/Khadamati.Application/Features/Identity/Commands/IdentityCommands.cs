using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Identity.Commands;

public record GetSessionsQuery(Guid UserId, Guid? CurrentSessionId) : IRequest<IReadOnlyList<SessionDto>>;
public record RevokeSessionCommand(Guid UserId, Guid SessionId, string? IpAddress, bool IsAdmin) : IRequest<Unit>;
public record RevokeAllOtherSessionsCommand(Guid UserId, Guid CurrentSessionId, string? IpAddress) : IRequest<Unit>;
public record RevokeAllSessionsCommand(Guid UserId, string? IpAddress) : IRequest<Unit>;
public record GetProfileQuery(Guid UserId) : IRequest<ProfileDto>;
public record UpdateProfileCommand(Guid UserId, UpdateProfileRequestDto Request) : IRequest<ProfileDto>;
public record GetLoginHistoryQuery(Guid UserId, int Page, int PageSize) : IRequest<PagedLoginHistoryDto>;
public record AdminVerifyEmailCommand(Guid AdminUserId, AdminVerifyEmailRequestDto Request) : IRequest<MessageResponseDto>;
public record GetMyPermissionsQuery(Guid UserId) : IRequest<IReadOnlyList<string>>;

public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, IReadOnlyList<SessionDto>>
{
    private readonly ISessionService _sessions;
    public GetSessionsQueryHandler(ISessionService sessions) => _sessions = sessions;
    public Task<IReadOnlyList<SessionDto>> Handle(GetSessionsQuery request, CancellationToken ct) =>
        _sessions.GetActiveSessionsAsync(request.UserId, request.CurrentSessionId, ct);
}

public class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand, Unit>
{
    private readonly ISessionService _sessions;
    public RevokeSessionCommandHandler(ISessionService sessions) => _sessions = sessions;
    public async Task<Unit> Handle(RevokeSessionCommand request, CancellationToken ct)
    {
        await _sessions.RevokeSessionAsync(request.UserId, request.SessionId, request.IpAddress, request.IsAdmin, ct);
        return Unit.Value;
    }
}

public class RevokeAllOtherSessionsCommandHandler : IRequestHandler<RevokeAllOtherSessionsCommand, Unit>
{
    private readonly ISessionService _sessions;
    public RevokeAllOtherSessionsCommandHandler(ISessionService sessions) => _sessions = sessions;
    public async Task<Unit> Handle(RevokeAllOtherSessionsCommand request, CancellationToken ct)
    {
        await _sessions.RevokeAllOtherSessionsAsync(request.UserId, request.CurrentSessionId, request.IpAddress, ct);
        return Unit.Value;
    }
}

public class RevokeAllSessionsCommandHandler : IRequestHandler<RevokeAllSessionsCommand, Unit>
{
    private readonly ISessionService _sessions;
    public RevokeAllSessionsCommandHandler(ISessionService sessions) => _sessions = sessions;
    public async Task<Unit> Handle(RevokeAllSessionsCommand request, CancellationToken ct)
    {
        await _sessions.RevokeAllSessionsAsync(request.UserId, request.IpAddress, ct);
        return Unit.Value;
    }
}

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto>
{
    private readonly IProfileService _profile;
    public GetProfileQueryHandler(IProfileService profile) => _profile = profile;
    public Task<ProfileDto> Handle(GetProfileQuery request, CancellationToken ct) => _profile.GetProfileAsync(request.UserId, ct);
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    private readonly IProfileService _profile;
    public UpdateProfileCommandHandler(IProfileService profile) => _profile = profile;
    public Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken ct) => _profile.UpdateProfileAsync(request.UserId, request.Request, ct);
}

public class GetLoginHistoryQueryHandler : IRequestHandler<GetLoginHistoryQuery, PagedLoginHistoryDto>
{
    private readonly ILoginHistoryService _history;
    public GetLoginHistoryQueryHandler(ILoginHistoryService history) => _history = history;
    public Task<PagedLoginHistoryDto> Handle(GetLoginHistoryQuery request, CancellationToken ct) =>
        _history.GetUserLoginHistoryAsync(request.UserId, request.Page, request.PageSize, ct);
}

public class AdminVerifyEmailCommandHandler : IRequestHandler<AdminVerifyEmailCommand, MessageResponseDto>
{
    private readonly IAuthService _auth;
    public AdminVerifyEmailCommandHandler(IAuthService auth) => _auth = auth;
    public Task<MessageResponseDto> Handle(AdminVerifyEmailCommand request, CancellationToken ct) =>
        _auth.AdminVerifyEmailAsync(request.AdminUserId, request.Request, ct);
}

public class GetMyPermissionsQueryHandler : IRequestHandler<GetMyPermissionsQuery, IReadOnlyList<string>>
{
    private readonly IPermissionService _permissions;
    public GetMyPermissionsQueryHandler(IPermissionService permissions) => _permissions = permissions;
    public Task<IReadOnlyList<string>> Handle(GetMyPermissionsQuery request, CancellationToken ct) =>
        _permissions.GetUserPermissionsAsync(request.UserId, ct);
}
