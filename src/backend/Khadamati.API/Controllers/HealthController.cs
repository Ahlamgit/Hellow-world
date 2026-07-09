using Khadamati.Application.Common;
using Khadamati.Application.DTOs;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    private readonly IIntegrationReadinessService _integrationReadiness;
    private readonly ApplicationDbContext _dbContext;

    public HealthController(
        IIntegrationReadinessService integrationReadiness,
        ApplicationDbContext dbContext)
    {
        _integrationReadiness = integrationReadiness;
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Get() =>
        Ok(new { status = "healthy", service = "KHADAMATI API", timestamp = DateTime.UtcNow });

    [HttpGet("ready")]
    public async Task<IActionResult> Ready(CancellationToken cancellationToken)
    {
        try
        {
            if (!await _dbContext.Database.CanConnectAsync(cancellationToken))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "not_ready",
                    reason = "database_unreachable",
                    timestamp = DateTime.UtcNow,
                });
            }

            if (_dbContext.Database.IsRelational())
                await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

            return Ok(new
            {
                status = "ready",
                service = "KHADAMATI API",
                database = "connected",
                timestamp = DateTime.UtcNow,
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "not_ready",
                reason = "database_check_failed",
                timestamp = DateTime.UtcNow,
            });
        }
    }

    [HttpGet("integrations")]
    public IActionResult Integrations() =>
        Ok(ApiResponse<object>.Ok(_integrationReadiness.GetReport()));
}
