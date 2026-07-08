using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Features.Admin.Commands;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Enterprise administration dashboard API.</summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = "AdminOnly")]
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("dashboard")]
    [SwaggerOperation(Summary = "Admin dashboard KPIs")]
    public async Task<IActionResult> Dashboard(CancellationToken ct) =>
        Ok(ApiResponse<AdminDashboardDto>.Ok(await _mediator.Send(new GetAdminDashboardQuery(), ct)));

    [HttpGet("{module}")]
    [SwaggerOperation(Summary = "List admin module data with search, filter, sort, pagination")]
    public async Task<IActionResult> List(string module, [FromQuery] AdminListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<AdminListResultDto>.Ok(await _mediator.Send(new ListAdminModuleQuery(module, query), ct)));

    [HttpPost("{module}/bulk")]
    [SwaggerOperation(Summary = "Bulk actions on module items")]
    public async Task<IActionResult> Bulk(string module, [FromBody] AdminBulkActionDto request, CancellationToken ct) =>
        Ok(ApiResponse<AdminBulkActionResultDto>.Ok(await _mediator.Send(new AdminBulkActionCommand(module, request, _currentUser.UserId?.ToString()), ct)));

    [HttpGet("{module}/export")]
    [SwaggerOperation(Summary = "Export module data as Excel or PDF")]
    public async Task<IActionResult> Export(string module, [FromQuery] string format, [FromQuery] AdminListQueryDto query, CancellationToken ct)
    {
        var bytes = await _mediator.Send(new ExportAdminModuleQuery(module, format, query), ct);
        var contentType = format.Equals("pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var ext = format.Equals("pdf", StringComparison.OrdinalIgnoreCase) ? "pdf" : "xlsx";
        return File(bytes, contentType, $"khadamati-{module}-{DateTime.UtcNow:yyyyMMdd}.{ext}");
    }

    [HttpGet("analytics/data")]
    [SwaggerOperation(Summary = "Analytics charts data")]
    public async Task<IActionResult> Analytics(CancellationToken ct) =>
        Ok(ApiResponse<AdminAnalyticsDto>.Ok(await _mediator.Send(new GetAdminAnalyticsQuery(), ct)));

    [HttpGet("reports/list")]
    [SwaggerOperation(Summary = "Available reports")]
    public async Task<IActionResult> Reports(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<AdminReportDto>>.Ok(await _mediator.Send(new GetAdminReportsQuery(), ct)));

    [HttpGet("system/health")]
    [SwaggerOperation(Summary = "System health check")]
    public async Task<IActionResult> SystemHealth(CancellationToken ct) =>
        Ok(ApiResponse<AdminSystemHealthDto>.Ok(await _mediator.Send(new GetAdminSystemHealthQuery(), ct)));

    [HttpGet("backup/list")]
    [SwaggerOperation(Summary = "List backup jobs")]
    public async Task<IActionResult> ListBackups(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<AdminBackupDto>>.Ok(await _mediator.Send(new ListAdminBackupsQuery(), ct)));

    [HttpPost("backup/create")]
    [SwaggerOperation(Summary = "Create new backup")]
    public async Task<IActionResult> CreateBackup(CancellationToken ct) =>
        Ok(ApiResponse<AdminBackupDto>.Ok(await _mediator.Send(new CreateAdminBackupCommand(_currentUser.UserId?.ToString()), ct)));

    [HttpPost("backup/restore")]
    [SwaggerOperation(Summary = "Restore from backup")]
    public async Task<IActionResult> Restore([FromBody] AdminRestoreRequestDto request, CancellationToken ct) =>
        Ok(ApiResponse<AdminBulkActionResultDto>.Ok(await _mediator.Send(new RestoreAdminBackupCommand(request, _currentUser.UserId?.ToString()), ct)));
}
