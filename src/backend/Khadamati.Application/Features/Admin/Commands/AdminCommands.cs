using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Interfaces;
using MediatR;
using Khadamati.Application.Common;

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
public record ListCategoriesQuery(CategoryListQueryDto Query) : IRequest<PagedResult<CategoryDto>>;
public record GetCategoryQuery(Guid Id) : IRequest<CategoryDto>;
public record CreateCategoryCommand(CreateCategoryDto Request, string? UserId) : IRequest<CategoryDto>;
public record UpdateCategoryCommand(Guid Id, UpdateCategoryDto Request, string? UserId) : IRequest<CategoryDto>;
public record DeleteCategoryCommand(Guid Id, string? UserId) : IRequest<Unit>;
public record ListServicesQuery(ServiceListQueryDto Query) : IRequest<PagedResult<ServiceDto>>;
public record GetServiceQuery(Guid Id) : IRequest<ServiceDto>;
public record CreateServiceCommand(CreateServiceDto Request, string? UserId) : IRequest<ServiceDto>;
public record UpdateServiceCommand(Guid Id, UpdateServiceDto Request, string? UserId) : IRequest<ServiceDto>;
public record DeleteServiceCommand(Guid Id, string? UserId) : IRequest<Unit>;
public record ListRegionsQuery(RegionListQueryDto Query) : IRequest<PagedResult<RegionDto>>;
public record GetRegionQuery(Guid Id) : IRequest<RegionDto>;
public record CreateRegionCommand(CreateRegionDto Request, string? UserId) : IRequest<RegionDto>;
public record UpdateRegionCommand(Guid Id, UpdateRegionDto Request, string? UserId) : IRequest<RegionDto>;
public record DeleteRegionCommand(Guid Id, string? UserId) : IRequest<Unit>;
public record ListCitiesQuery(CityListQueryDto Query) : IRequest<PagedResult<CityDto>>;
public record GetCityQuery(Guid Id) : IRequest<CityDto>;
public record CreateCityCommand(CreateCityDto Request, string? UserId) : IRequest<CityDto>;
public record UpdateCityCommand(Guid Id, UpdateCityDto Request, string? UserId) : IRequest<CityDto>;
public record DeleteCityCommand(Guid Id, string? UserId) : IRequest<Unit>;
public record ListCouponsQuery(CouponListQueryDto Query) : IRequest<PagedResult<CouponDto>>;
public record GetCouponQuery(Guid Id) : IRequest<CouponDto>;
public record CreateCouponCommand(CreateCouponDto Request, string? UserId) : IRequest<CouponDto>;
public record UpdateCouponCommand(Guid Id, UpdateCouponDto Request, string? UserId) : IRequest<CouponDto>;
public record DeleteCouponCommand(Guid Id, string? UserId) : IRequest<Unit>;
public record ListAdvertisementsQuery(AdvertisementListQueryDto Query) : IRequest<PagedResult<AdminAdvertisementDto>>;
public record GetAdvertisementQuery(Guid Id) : IRequest<AdminAdvertisementDto>;
public record CreateAdvertisementCommand(CreateAdvertisementDto Request, string? UserId) : IRequest<AdminAdvertisementDto>;
public record UpdateAdvertisementCommand(Guid Id, UpdateAdvertisementDto Request, string? UserId) : IRequest<AdminAdvertisementDto>;
public record DeleteAdvertisementCommand(Guid Id, string? UserId) : IRequest<Unit>;

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

public class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly IAdminService _admin;
    public ListCategoriesQueryHandler(IAdminService admin) => _admin = admin;
    public Task<PagedResult<CategoryDto>> Handle(ListCategoriesQuery request, CancellationToken ct) =>
        _admin.ListCategoriesAsync(request.Query, ct);
}

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
{
    private readonly IAdminService _admin;
    public GetCategoryQueryHandler(IAdminService admin) => _admin = admin;
    public Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken ct) =>
        _admin.GetCategoryAsync(request.Id, ct);
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IAdminService _admin;
    public CreateCategoryCommandHandler(IAdminService admin) => _admin = admin;
    public Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct) =>
        _admin.CreateCategoryAsync(request.Request, request.UserId, ct);
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IAdminService _admin;
    public UpdateCategoryCommandHandler(IAdminService admin) => _admin = admin;
    public Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct) =>
        _admin.UpdateCategoryAsync(request.Id, request.Request, request.UserId, ct);
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IAdminService _admin;
    public DeleteCategoryCommandHandler(IAdminService admin) => _admin = admin;
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        await _admin.DeleteCategoryAsync(request.Id, request.UserId, ct);
        return Unit.Value;
    }
}

public class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, PagedResult<ServiceDto>>
{
    private readonly IAdminService _admin;
    public ListServicesQueryHandler(IAdminService admin) => _admin = admin;
    public Task<PagedResult<ServiceDto>> Handle(ListServicesQuery request, CancellationToken ct) =>
        _admin.ListServicesAsync(request.Query, ct);
}

public class GetServiceQueryHandler : IRequestHandler<GetServiceQuery, ServiceDto>
{
    private readonly IAdminService _admin;
    public GetServiceQueryHandler(IAdminService admin) => _admin = admin;
    public Task<ServiceDto> Handle(GetServiceQuery request, CancellationToken ct) =>
        _admin.GetServiceAsync(request.Id, ct);
}

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IAdminService _admin;
    public CreateServiceCommandHandler(IAdminService admin) => _admin = admin;
    public Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken ct) =>
        _admin.CreateServiceAsync(request.Request, request.UserId, ct);
}

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ServiceDto>
{
    private readonly IAdminService _admin;
    public UpdateServiceCommandHandler(IAdminService admin) => _admin = admin;
    public Task<ServiceDto> Handle(UpdateServiceCommand request, CancellationToken ct) =>
        _admin.UpdateServiceAsync(request.Id, request.Request, request.UserId, ct);
}

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Unit>
{
    private readonly IAdminService _admin;
    public DeleteServiceCommandHandler(IAdminService admin) => _admin = admin;
    public async Task<Unit> Handle(DeleteServiceCommand request, CancellationToken ct)
    {
        await _admin.DeleteServiceAsync(request.Id, request.UserId, ct);
        return Unit.Value;
    }
}

public class ListRegionsQueryHandler : IRequestHandler<ListRegionsQuery, PagedResult<RegionDto>>
{
    private readonly IAdminService _admin;
    public ListRegionsQueryHandler(IAdminService admin) => _admin = admin;
    public Task<PagedResult<RegionDto>> Handle(ListRegionsQuery request, CancellationToken ct) =>
        _admin.ListRegionsAsync(request.Query, ct);
}

public class GetRegionQueryHandler : IRequestHandler<GetRegionQuery, RegionDto>
{
    private readonly IAdminService _admin;
    public GetRegionQueryHandler(IAdminService admin) => _admin = admin;
    public Task<RegionDto> Handle(GetRegionQuery request, CancellationToken ct) =>
        _admin.GetRegionAsync(request.Id, ct);
}

public class CreateRegionCommandHandler : IRequestHandler<CreateRegionCommand, RegionDto>
{
    private readonly IAdminService _admin;
    public CreateRegionCommandHandler(IAdminService admin) => _admin = admin;
    public Task<RegionDto> Handle(CreateRegionCommand request, CancellationToken ct) =>
        _admin.CreateRegionAsync(request.Request, request.UserId, ct);
}

public class UpdateRegionCommandHandler : IRequestHandler<UpdateRegionCommand, RegionDto>
{
    private readonly IAdminService _admin;
    public UpdateRegionCommandHandler(IAdminService admin) => _admin = admin;
    public Task<RegionDto> Handle(UpdateRegionCommand request, CancellationToken ct) =>
        _admin.UpdateRegionAsync(request.Id, request.Request, request.UserId, ct);
}

public class DeleteRegionCommandHandler : IRequestHandler<DeleteRegionCommand, Unit>
{
    private readonly IAdminService _admin;
    public DeleteRegionCommandHandler(IAdminService admin) => _admin = admin;
    public async Task<Unit> Handle(DeleteRegionCommand request, CancellationToken ct)
    {
        await _admin.DeleteRegionAsync(request.Id, request.UserId, ct);
        return Unit.Value;
    }
}

public class ListCitiesQueryHandler : IRequestHandler<ListCitiesQuery, PagedResult<CityDto>>
{
    private readonly IAdminService _admin;
    public ListCitiesQueryHandler(IAdminService admin) => _admin = admin;
    public Task<PagedResult<CityDto>> Handle(ListCitiesQuery request, CancellationToken ct) =>
        _admin.ListCitiesAsync(request.Query, ct);
}

public class GetCityQueryHandler : IRequestHandler<GetCityQuery, CityDto>
{
    private readonly IAdminService _admin;
    public GetCityQueryHandler(IAdminService admin) => _admin = admin;
    public Task<CityDto> Handle(GetCityQuery request, CancellationToken ct) =>
        _admin.GetCityAsync(request.Id, ct);
}

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, CityDto>
{
    private readonly IAdminService _admin;
    public CreateCityCommandHandler(IAdminService admin) => _admin = admin;
    public Task<CityDto> Handle(CreateCityCommand request, CancellationToken ct) =>
        _admin.CreateCityAsync(request.Request, request.UserId, ct);
}

public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, CityDto>
{
    private readonly IAdminService _admin;
    public UpdateCityCommandHandler(IAdminService admin) => _admin = admin;
    public Task<CityDto> Handle(UpdateCityCommand request, CancellationToken ct) =>
        _admin.UpdateCityAsync(request.Id, request.Request, request.UserId, ct);
}

public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Unit>
{
    private readonly IAdminService _admin;
    public DeleteCityCommandHandler(IAdminService admin) => _admin = admin;
    public async Task<Unit> Handle(DeleteCityCommand request, CancellationToken ct)
    {
        await _admin.DeleteCityAsync(request.Id, request.UserId, ct);
        return Unit.Value;
    }
}

public class ListCouponsQueryHandler : IRequestHandler<ListCouponsQuery, PagedResult<CouponDto>>
{
    private readonly IAdminService _admin;
    public ListCouponsQueryHandler(IAdminService admin) => _admin = admin;
    public Task<PagedResult<CouponDto>> Handle(ListCouponsQuery request, CancellationToken ct) =>
        _admin.ListCouponsAdminAsync(request.Query, ct);
}

public class GetCouponQueryHandler : IRequestHandler<GetCouponQuery, CouponDto>
{
    private readonly IAdminService _admin;
    public GetCouponQueryHandler(IAdminService admin) => _admin = admin;
    public Task<CouponDto> Handle(GetCouponQuery request, CancellationToken ct) =>
        _admin.GetCouponAsync(request.Id, ct);
}

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, CouponDto>
{
    private readonly IAdminService _admin;
    public CreateCouponCommandHandler(IAdminService admin) => _admin = admin;
    public Task<CouponDto> Handle(CreateCouponCommand request, CancellationToken ct) =>
        _admin.CreateCouponAsync(request.Request, request.UserId, ct);
}

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, CouponDto>
{
    private readonly IAdminService _admin;
    public UpdateCouponCommandHandler(IAdminService admin) => _admin = admin;
    public Task<CouponDto> Handle(UpdateCouponCommand request, CancellationToken ct) =>
        _admin.UpdateCouponAsync(request.Id, request.Request, request.UserId, ct);
}

public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, Unit>
{
    private readonly IAdminService _admin;
    public DeleteCouponCommandHandler(IAdminService admin) => _admin = admin;
    public async Task<Unit> Handle(DeleteCouponCommand request, CancellationToken ct)
    {
        await _admin.DeleteCouponAsync(request.Id, request.UserId, ct);
        return Unit.Value;
    }
}

public class ListAdvertisementsQueryHandler : IRequestHandler<ListAdvertisementsQuery, PagedResult<AdminAdvertisementDto>>
{
    private readonly IAdminService _admin;
    public ListAdvertisementsQueryHandler(IAdminService admin) => _admin = admin;
    public Task<PagedResult<AdminAdvertisementDto>> Handle(ListAdvertisementsQuery request, CancellationToken ct) =>
        _admin.ListAdvertisementsAdminAsync(request.Query, ct);
}

public class GetAdvertisementQueryHandler : IRequestHandler<GetAdvertisementQuery, AdminAdvertisementDto>
{
    private readonly IAdminService _admin;
    public GetAdvertisementQueryHandler(IAdminService admin) => _admin = admin;
    public Task<AdminAdvertisementDto> Handle(GetAdvertisementQuery request, CancellationToken ct) =>
        _admin.GetAdvertisementAsync(request.Id, ct);
}

public class CreateAdvertisementCommandHandler : IRequestHandler<CreateAdvertisementCommand, AdminAdvertisementDto>
{
    private readonly IAdminService _admin;
    public CreateAdvertisementCommandHandler(IAdminService admin) => _admin = admin;
    public Task<AdminAdvertisementDto> Handle(CreateAdvertisementCommand request, CancellationToken ct) =>
        _admin.CreateAdvertisementAsync(request.Request, request.UserId, ct);
}

public class UpdateAdvertisementCommandHandler : IRequestHandler<UpdateAdvertisementCommand, AdminAdvertisementDto>
{
    private readonly IAdminService _admin;
    public UpdateAdvertisementCommandHandler(IAdminService admin) => _admin = admin;
    public Task<AdminAdvertisementDto> Handle(UpdateAdvertisementCommand request, CancellationToken ct) =>
        _admin.UpdateAdvertisementAsync(request.Id, request.Request, request.UserId, ct);
}

public class DeleteAdvertisementCommandHandler : IRequestHandler<DeleteAdvertisementCommand, Unit>
{
    private readonly IAdminService _admin;
    public DeleteAdvertisementCommandHandler(IAdminService admin) => _admin = admin;
    public async Task<Unit> Handle(DeleteAdvertisementCommand request, CancellationToken ct)
    {
        await _admin.DeleteAdvertisementAsync(request.Id, request.UserId, ct);
        return Unit.Value;
    }
}
