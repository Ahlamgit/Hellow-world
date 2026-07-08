using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Services;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Features.Services.Queries;
using Khadamati.Application.Features.Users.Queries;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khadamati.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public UsersController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("User not authenticated.");
        var result = await _mediator.Send(new GetUserProfileQuery(userId), cancellationToken);
        return Ok(ApiResponse<UserProfileDto>.Ok(result));
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("User not authenticated.");
        var result = await _mediator.Send(new UpdateUserProfileCommand(userId, request), cancellationToken);
        return Ok(ApiResponse<UserProfileDto>.Ok(result, "Profile updated successfully."));
    }

    [HttpPost("me/addresses")]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("User not authenticated.");
        var result = await _mediator.Send(new AddAddressCommand(userId, request), cancellationToken);
        return Created(string.Empty, ApiResponse<AddressDto>.Ok(result));
    }
}

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ServicesController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ServiceCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetServiceCategoriesQuery(), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ServiceCategoryDto>>.Ok(result));
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ServiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices([FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetServicesQuery(categoryId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ServiceDto>>.Ok(result));
    }
}

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get() => Ok(new { status = "healthy", service = "KHADAMATI API", timestamp = DateTime.UtcNow });
}
