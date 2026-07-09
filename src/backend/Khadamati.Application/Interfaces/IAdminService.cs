using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;

namespace Khadamati.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<AdminListResultDto> ListModuleAsync(string module, AdminListQueryDto query, CancellationToken cancellationToken = default);
    Task<AdminBulkActionResultDto> BulkActionAsync(string module, AdminBulkActionDto request, string? userId, CancellationToken cancellationToken = default);
    Task<byte[]> ExportAsync(string module, string format, AdminListQueryDto query, CancellationToken cancellationToken = default);
    Task<AdminAnalyticsDto> GetAnalyticsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminReportDto>> GetReportsAsync(CancellationToken cancellationToken = default);
    Task<AdminSystemHealthDto> GetSystemHealthAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminBackupDto>> ListBackupsAsync(CancellationToken cancellationToken = default);
    Task<AdminBackupDto> CreateBackupAsync(string? userId, CancellationToken cancellationToken = default);
    Task<AdminBulkActionResultDto> RestoreBackupAsync(AdminRestoreRequestDto request, string? userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SystemSettingDto>> ListSettingsAsync(CancellationToken cancellationToken = default);
    Task<SystemSettingDto> UpdateSettingAsync(Guid id, UpdateSystemSettingDto request, string? userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminRoleDto>> ListRbacRolesAsync(CancellationToken cancellationToken = default);
    Task<RolePermissionMatrixDto> GetRolePermissionMatrixAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<RolePermissionMatrixDto> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsDto request, string? userId, CancellationToken cancellationToken = default);
    Task<PagedResult<CategoryDto>> ListCategoriesAsync(CategoryListQueryDto query, CancellationToken cancellationToken = default);
    Task<CategoryDto> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request, string? userId, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto request, string? userId, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, string? userId, CancellationToken cancellationToken = default);
    Task<PagedResult<ServiceDto>> ListServicesAsync(ServiceListQueryDto query, CancellationToken cancellationToken = default);
    Task<ServiceDto> GetServiceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto request, string? userId, CancellationToken cancellationToken = default);
    Task<ServiceDto> UpdateServiceAsync(Guid id, UpdateServiceDto request, string? userId, CancellationToken cancellationToken = default);
    Task DeleteServiceAsync(Guid id, string? userId, CancellationToken cancellationToken = default);
    Task<PagedResult<RegionDto>> ListRegionsAsync(RegionListQueryDto query, CancellationToken cancellationToken = default);
    Task<RegionDto> GetRegionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RegionDto> CreateRegionAsync(CreateRegionDto request, string? userId, CancellationToken cancellationToken = default);
    Task<RegionDto> UpdateRegionAsync(Guid id, UpdateRegionDto request, string? userId, CancellationToken cancellationToken = default);
    Task DeleteRegionAsync(Guid id, string? userId, CancellationToken cancellationToken = default);
    Task<PagedResult<CityDto>> ListCitiesAsync(CityListQueryDto query, CancellationToken cancellationToken = default);
    Task<CityDto> GetCityAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CityDto> CreateCityAsync(CreateCityDto request, string? userId, CancellationToken cancellationToken = default);
    Task<CityDto> UpdateCityAsync(Guid id, UpdateCityDto request, string? userId, CancellationToken cancellationToken = default);
    Task DeleteCityAsync(Guid id, string? userId, CancellationToken cancellationToken = default);
}

public interface IAdminExportService
{
    byte[] ToExcel(string sheetName, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows);
    byte[] ToPdf(string title, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows);
}
