using Khadamati.Application.DTOs;
using Khadamati.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    private readonly IIntegrationReadinessService _integrationReadiness;

    public HealthController(IIntegrationReadinessService integrationReadiness) =>
        _integrationReadiness = integrationReadiness;

    [HttpGet]
    public IActionResult Get() =>
        Ok(new { status = "healthy", service = "KHADAMATI API", timestamp = DateTime.UtcNow });

    [HttpGet("integrations")]
    public IActionResult Integrations() =>
        Ok(ApiResponse<object>.Ok(_integrationReadiness.GetReport()));
}
