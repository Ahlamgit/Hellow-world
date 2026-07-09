using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Admin.Commands;

public record GetAdminDashboardQuery() : IRequest<AdminDashboardDto>;
public record ListAdminModuleQuery(string Module, AdminListQueryDto Query) : IRequest<AdminListResultDto>;
public record AdminBulkActionCommand(string Module, AdminBulkActionDto Request, string? UserId) : IRequest<AdminBulkActionResultDto>;
public record ExportAdminModuleQuery(string Module, string Format, AdminListQueryDto Query) : IRequest<byte[]>;
public record GetAdminAnalyticsQuery() : IRequest<AdminAnalyticsDto>;
public record GetAdminReportsQuery() : IRequest<IReadOnlyList<AdminReportDto>>;
public record GetAdminSystemHealthQuery() : IRequest<AdminSystemHealthDto>;
public record ListAdminBackupsQuery() : IRequest<IReadOnlyList<AdminBackupDto>>;
public record CreateAdminBackupCommand(string? UserId) : IRequest<AdminBackupDto>;
public record RestoreAdminBackupCommand(AdminRestoreRequestDto Request, string? UserId) : IRequest<AdminBulkActionResultDto>;
public record ListSystemSettingsQuery() : IRequest<IReadOnlyList<SystemSettingDto>>;
public record UpdateSystemSettingCommand(Guid Id, UpdateSystemSettingDto Request, string? UserId) : IRequest<SystemSettingDto>;
public record ListRbacRolesQuery() : IRequest<IReadOnlyList<AdminRoleDto>>;
public record GetRolePermissionMatrixQuery(Guid RoleId) : IRequest<RolePermissionMatrixDto>;
public record UpdateRolePermissionsCommand(Guid RoleId, UpdateRolePermissionsDto Request, string? UserId) : IRequest<RolePermissionMatrixDto>;

public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IAdminService _admin;
    public GetAdminDashboardQueryHandler(IAdminService admin) => _admin = admin;
    public Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken ct) => _admin.GetDashboardAsync(ct);
}

public class ListAdminModuleQueryHandler : IRequestHandler<ListAdminModuleQuery, AdminListResultDto>
{
    private readonly IAdminService _admin;
    public ListAdminModuleQueryHandler(IAdminService admin) => _admin = admin;
    public Task<AdminListResultDto> Handle(ListAdminModuleQuery request, CancellationToken ct) => _admin.ListModuleAsync(request.Module, request.Query, ct);
}

public class AdminBulkActionCommandHandler : IRequestHandler<AdminBulkActionCommand, AdminBulkActionResultDto>
{
    private readonly IAdminService _admin;
    public AdminBulkActionCommandHandler(IAdminService admin) => _admin = admin;
    public Task<AdminBulkActionResultDto> Handle(AdminBulkActionCommand request, CancellationToken ct) => _admin.BulkActionAsync(request.Module, request.Request, request.UserId, ct);
}

public class ExportAdminModuleQueryHandler : IRequestHandler<ExportAdminModuleQuery, byte[]>
{
    private readonly IAdminService _admin;
    public ExportAdminModuleQueryHandler(IAdminService admin) => _admin = admin;
    public Task<byte[]> Handle(ExportAdminModuleQuery request, CancellationToken ct) => _admin.ExportAsync(request.Module, request.Format, request.Query, ct);
}

public class GetAdminAnalyticsQueryHandler : IRequestHandler<GetAdminAnalyticsQuery, AdminAnalyticsDto>
{
    private readonly IAdminService _admin;
    public GetAdminAnalyticsQueryHandler(IAdminService admin) => _admin = admin;
    public Task<AdminAnalyticsDto> Handle(GetAdminAnalyticsQuery request, CancellationToken ct) => _admin.GetAnalyticsAsync(ct);
}

public class GetAdminReportsQueryHandler : IRequestHandler<GetAdminReportsQuery, IReadOnlyList<AdminReportDto>>
{
    private readonly IAdminService _admin;
    public GetAdminReportsQueryHandler(IAdminService admin) => _admin = admin;
    public Task<IReadOnlyList<AdminReportDto>> Handle(GetAdminReportsQuery request, CancellationToken ct) => _admin.GetReportsAsync(ct);
}

public class GetAdminSystemHealthQueryHandler : IRequestHandler<GetAdminSystemHealthQuery, AdminSystemHealthDto>
{
    private readonly IAdminService _admin;
    public GetAdminSystemHealthQueryHandler(IAdminService admin) => _admin = admin;
    public Task<AdminSystemHealthDto> Handle(GetAdminSystemHealthQuery request, CancellationToken ct) => _admin.GetSystemHealthAsync(ct);
}

public class ListAdminBackupsQueryHandler : IRequestHandler<ListAdminBackupsQuery, IReadOnlyList<AdminBackupDto>>
{
    private readonly IAdminService _admin;
    public ListAdminBackupsQueryHandler(IAdminService admin) => _admin = admin;
    public Task<IReadOnlyList<AdminBackupDto>> Handle(ListAdminBackupsQuery request, CancellationToken ct) => _admin.ListBackupsAsync(ct);
}

public class CreateAdminBackupCommandHandler : IRequestHandler<CreateAdminBackupCommand, AdminBackupDto>
{
    private readonly IAdminService _admin;
    public CreateAdminBackupCommandHandler(IAdminService admin) => _admin = admin;
    public Task<AdminBackupDto> Handle(CreateAdminBackupCommand request, CancellationToken ct) => _admin.CreateBackupAsync(request.UserId, ct);
}

public class RestoreAdminBackupCommandHandler : IRequestHandler<RestoreAdminBackupCommand, AdminBulkActionResultDto>
{
    private readonly IAdminService _admin;
    public RestoreAdminBackupCommandHandler(IAdminService admin) => _admin = admin;
    public Task<AdminBulkActionResultDto> Handle(RestoreAdminBackupCommand request, CancellationToken ct) => _admin.RestoreBackupAsync(request.Request, request.UserId, ct);
}

public class ListSystemSettingsQueryHandler : IRequestHandler<ListSystemSettingsQuery, IReadOnlyList<SystemSettingDto>>
{
    private readonly IAdminService _admin;
    public ListSystemSettingsQueryHandler(IAdminService admin) => _admin = admin;
    public Task<IReadOnlyList<SystemSettingDto>> Handle(ListSystemSettingsQuery request, CancellationToken ct) => _admin.ListSettingsAsync(ct);
}

public class UpdateSystemSettingCommandHandler : IRequestHandler<UpdateSystemSettingCommand, SystemSettingDto>
{
    private readonly IAdminService _admin;
    public UpdateSystemSettingCommandHandler(IAdminService admin) => _admin = admin;
    public Task<SystemSettingDto> Handle(UpdateSystemSettingCommand request, CancellationToken ct) =>
        _admin.UpdateSettingAsync(request.Id, request.Request, request.UserId, ct);
}

public class ListRbacRolesQueryHandler : IRequestHandler<ListRbacRolesQuery, IReadOnlyList<AdminRoleDto>>
{
    private readonly IAdminService _admin;
    public ListRbacRolesQueryHandler(IAdminService admin) => _admin = admin;
    public Task<IReadOnlyList<AdminRoleDto>> Handle(ListRbacRolesQuery request, CancellationToken ct) => _admin.ListRbacRolesAsync(ct);
}

public class GetRolePermissionMatrixQueryHandler : IRequestHandler<GetRolePermissionMatrixQuery, RolePermissionMatrixDto>
{
    private readonly IAdminService _admin;
    public GetRolePermissionMatrixQueryHandler(IAdminService admin) => _admin = admin;
    public Task<RolePermissionMatrixDto> Handle(GetRolePermissionMatrixQuery request, CancellationToken ct) =>
        _admin.GetRolePermissionMatrixAsync(request.RoleId, ct);
}

public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, RolePermissionMatrixDto>
{
    private readonly IAdminService _admin;
    public UpdateRolePermissionsCommandHandler(IAdminService admin) => _admin = admin;
    public Task<RolePermissionMatrixDto> Handle(UpdateRolePermissionsCommand request, CancellationToken ct) =>
        _admin.UpdateRolePermissionsAsync(request.RoleId, request.Request, request.UserId, ct);
}
