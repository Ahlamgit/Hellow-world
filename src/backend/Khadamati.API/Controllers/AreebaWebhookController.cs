using System.Text;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Features.Payments.Commands;
using Khadamati.Infrastructure.Services.Payments.Areeba;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/webhooks/areeba")]
[AllowAnonymous]
public class AreebaWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public AreebaWebhookController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Handle(CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(ct);
        if (string.IsNullOrWhiteSpace(rawBody))
            return BadRequest("Empty webhook payload.");

        var payload = System.Text.Json.JsonSerializer.Deserialize<AreebaWebhookDto>(
            rawBody,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (payload is null)
            return BadRequest("Invalid webhook payload.");

        var context = new AreebaWebhookContext
        {
            Payload = payload,
            RawBody = rawBody,
            Signature = Request.Headers[AreebaOptions.DefaultSignatureHeaderName].FirstOrDefault(),
            DateHeader = Request.Headers.Date.FirstOrDefault(),
            ContentType = Request.ContentType ?? "application/json; charset=utf-8",
            RequestUri = Request.Path.Value ?? "/api/v1/webhooks/areeba",
        };

        await _mediator.Send(new ProcessAreebaWebhookCommand(context), ct);
        return Content("OK", "text/plain", Encoding.UTF8);
    }
}
