using System.Text;
using System.Text.Json;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Features.Payments.Commands;
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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IMediator _mediator;

    public AreebaWebhookController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [SwaggerOperation(Summary = "Areeba payment webhook", Description = "Confirms booking payments when Areeba reports a successful status.")]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;

        AreebaWebhookDto payload;
        try
        {
            payload = JsonSerializer.Deserialize<AreebaWebhookDto>(rawBody, JsonOptions)
                ?? new AreebaWebhookDto();
        }
        catch (JsonException)
        {
            return BadRequest(ApiResponse<object>.Fail("Invalid webhook payload."));
        }

        var signature = Request.Headers["X-Areeba-Signature"].FirstOrDefault()
            ?? Request.Headers["X-Notification-Secret"].FirstOrDefault()
            ?? Request.Headers["X-Webhook-Secret"].FirstOrDefault();

        var result = await _mediator.Send(new ProcessAreebaWebhookCommand(payload, signature, rawBody), ct);
        return Ok(ApiResponse<PaymentWebhookResultDto>.Ok(result));
    }
}
