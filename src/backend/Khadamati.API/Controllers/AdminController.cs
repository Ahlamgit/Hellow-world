using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Features.Admin.Commands;
using Khadamati.Application.Features.Verification;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Enterprise administration dashboard API.</summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize]
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IPermissionService _permissions;

    public AdminController(IMediator mediator, ICurrentUserService currentUser, IPermissionService permissions)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _permissions = permissions;
    }

    private Guid RequireUserId() =>
        _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");

    [HttpGet("dashboard")]
    [SwaggerOperation(Summary = "Admin dashboard KPIs")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        await AdminAuthorization.EnsurePortalAccessAsync(_permissions, RequireUserId(), ct);
        return Ok(ApiResponse<AdminDashboardDto>.Ok(await _mediator.Send(new GetAdminDashboardQuery(), ct)));
    }

    [HttpGet("{module}")]
    [SwaggerOperation(Summary = "List admin module data with search, filter, sort, pagination")]
    public async Task<IActionResult> List(string module, [FromQuery] AdminListQueryDto query, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(
            _permissions, RequireUserId(), AdminPermissionMap.ViewPermissionForModule(module), ct);
        return Ok(ApiResponse<AdminListResultDto>.Ok(await _mediator.Send(new ListAdminModuleQuery(module, query), ct)));
    }

    [HttpPost("{module}/bulk")]
    [SwaggerOperation(Summary = "Bulk actions on module items")]
    public async Task<IActionResult> Bulk(string module, [FromBody] AdminBulkActionDto request, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(
            _permissions, RequireUserId(), AdminPermissionMap.BulkPermissionForModule(module, request.Action), ct);
        return Ok(ApiResponse<AdminBulkActionResultDto>.Ok(
            await _mediator.Send(new AdminBulkActionCommand(module, request, _currentUser.UserId?.ToString()), ct)));
    }

    [HttpGet("{module}/export")]
    [SwaggerOperation(Summary = "Export module data as Excel or PDF")]
    public async Task<IActionResult> Export(string module, [FromQuery] string format, [FromQuery] AdminListQueryDto query, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.ReportsExport, ct);
        var bytes = await _mediator.Send(new ExportAdminModuleQuery(module, format, query), ct);
        var contentType = format.Equals("pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var ext = format.Equals("pdf", StringComparison.OrdinalIgnoreCase) ? "pdf" : "xlsx";
        return File(bytes, contentType, $"khadamati-{module}-{DateTime.UtcNow:yyyyMMdd}.{ext}");
    }

    [HttpGet("analytics/data")]
    [SwaggerOperation(Summary = "Analytics charts data")]
    public async Task<IActionResult> Analytics(CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.ReportsView, ct);
        return Ok(ApiResponse<AdminAnalyticsDto>.Ok(await _mediator.Send(new GetAdminAnalyticsQuery(), ct)));
    }

    [HttpGet("reports/list")]
    [SwaggerOperation(Summary = "Available reports")]
    public async Task<IActionResult> Reports(CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.ReportsView, ct);
        return Ok(ApiResponse<IReadOnlyList<AdminReportDto>>.Ok(await _mediator.Send(new GetAdminReportsQuery(), ct)));
    }

    [HttpGet("system/health")]
    [SwaggerOperation(Summary = "System health check")]
    public async Task<IActionResult> SystemHealth(CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.SettingsManage, ct);
        return Ok(ApiResponse<AdminSystemHealthDto>.Ok(await _mediator.Send(new GetAdminSystemHealthQuery(), ct)));
    }

    [HttpGet("backup/list")]
    [SwaggerOperation(Summary = "List backup jobs")]
    public async Task<IActionResult> ListBackups(CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.SettingsManage, ct);
        return Ok(ApiResponse<IReadOnlyList<AdminBackupDto>>.Ok(await _mediator.Send(new ListAdminBackupsQuery(), ct)));
    }

    [HttpPost("backup/create")]
    [SwaggerOperation(Summary = "Create new backup")]
    public async Task<IActionResult> CreateBackup(CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.SettingsManage, ct);
        return Ok(ApiResponse<AdminBackupDto>.Ok(await _mediator.Send(new CreateAdminBackupCommand(_currentUser.UserId?.ToString()), ct)));
    }

    [HttpPost("backup/restore")]
    [SwaggerOperation(Summary = "Restore from backup")]
    public async Task<IActionResult> Restore([FromBody] AdminRestoreRequestDto request, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.SettingsManage, ct);
        return Ok(ApiResponse<AdminBulkActionResultDto>.Ok(
            await _mediator.Send(new RestoreAdminBackupCommand(request, _currentUser.UserId?.ToString()), ct)));
    }

    [HttpGet("backup/{id:guid}/download")]
    [SwaggerOperation(Summary = "Download backup file")]
    public async Task<IActionResult> DownloadBackup(Guid id, CancellationToken ct)
    {
        await AdminAuthorization.EnsurePermissionAsync(_permissions, RequireUserId(), PermissionCodes.SettingsManage, ct);
        var download = await _mediator.Send(new DownloadAdminBackupQuery(id), ct);
        return File(download.Stream, "application/gzip", download.FileName);
    }
}
