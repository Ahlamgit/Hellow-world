using Khadamati.Application.Authorization;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Features.Admin.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/admin/categories")]
[Authorize]
[Produces("application/json")]
public class AdminCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminCategoriesController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "List service categories")]
    public async Task<IActionResult> List([FromQuery] CategoryListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<CategoryDto>>.Ok(await _mediator.Send(new ListCategoriesQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Get category by ID")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<CategoryDto>.Ok(await _mediator.Send(new GetCategoryQuery(id), ct)));

    [HttpPost]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Create category")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCategoryCommand(request, _currentUser.UserId?.ToString()), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CategoryDto>.Ok(result, "Category created."));
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Update category")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto request, CancellationToken ct) =>
        Ok(ApiResponse<CategoryDto>.Ok(
            await _mediator.Send(new UpdateCategoryCommand(id, request, _currentUser.UserId?.ToString()), ct),
            "Category updated."));

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Soft-delete category")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCategoryCommand(id, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Category deleted."));
    }
}

[ApiController]
[Route("api/v1/admin/services")]
[Authorize]
[Produces("application/json")]
public class AdminServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AdminServicesController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "List services")]
    public async Task<IActionResult> List([FromQuery] ServiceListQueryDto query, CancellationToken ct) =>
        Ok(ApiResponse<PagedResult<ServiceDto>>.Ok(await _mediator.Send(new ListServicesQuery(query), ct)));

    [HttpGet("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Get service by ID")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<ServiceDto>.Ok(await _mediator.Send(new GetServiceQuery(id), ct)));

    [HttpPost]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Create service")]
    public async Task<IActionResult> Create([FromBody] CreateServiceDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateServiceCommand(request, _currentUser.UserId?.ToString()), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ServiceDto>.Ok(result, "Service created."));
    }

    [HttpPut("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Update service")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceDto request, CancellationToken ct) =>
        Ok(ApiResponse<ServiceDto>.Ok(
            await _mediator.Send(new UpdateServiceCommand(id, request, _currentUser.UserId?.ToString()), ct),
            "Service updated."));

    [HttpDelete("{id:guid}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    [SwaggerOperation(Summary = "Soft-delete service")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteServiceCommand(id, _currentUser.UserId?.ToString()), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Service deleted."));
    }
}
