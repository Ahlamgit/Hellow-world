using System.Text;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Features.Payments.Commands;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/webhooks/areeba")]
[AllowAnonymous]
[Produces("application/json")]
public class AreebaWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public AreebaWebhookController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [SwaggerOperation(Summary = "Areeba payment webhook", Description = "Confirms booking payments when Areeba reports paid status.")]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);
        if (string.IsNullOrWhiteSpace(rawBody))
            return BadRequest(ApiResponse<object>.Fail("Empty webhook payload."));

        var payload = System.Text.Json.JsonSerializer.Deserialize<AreebaWebhookDto>(
            rawBody,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (payload is null)
            return BadRequest(ApiResponse<object>.Fail("Invalid webhook payload."));

        var signature = Request.Headers[AreebaOptions.DefaultSignatureHeaderName].FirstOrDefault();
        var result = await _mediator.Send(new ProcessAreebaWebhookCommand(payload, signature, rawBody), ct);
        return Ok(ApiResponse<PaymentWebhookResultDto>.Ok(result));
    }
}
