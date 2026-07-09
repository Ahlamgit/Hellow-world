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
}

public interface IAdminExportService
{
    byte[] ToExcel(string sheetName, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows);
    byte[] ToPdf(string title, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows);
}
