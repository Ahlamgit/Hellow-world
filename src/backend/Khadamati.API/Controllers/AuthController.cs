using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Features.Auth.Commands;
using Khadamati.Application.Features.Identity.Commands;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>Authentication and identity endpoints for KHADAMATI platform.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Register new user (Customer, Craftsman, StoreOwner)")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterCommand(request, _currentUser.IpAddress), ct);
        return Created(string.Empty, ApiResponse<AuthResponseDto>.Ok(result, "Registration successful."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login with email/password. Supports RememberMe and device registration.")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct) =>
        Ok(ApiResponse<AuthResponseDto>.Ok(await _mediator.Send(new LoginCommand(request, _currentUser.IpAddress), ct)));

    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Refresh JWT with token rotation")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken ct) =>
        Ok(ApiResponse<AuthResponseDto>.Ok(await _mediator.Send(new RefreshTokenCommand(request, _currentUser.IpAddress), ct)));

    [HttpPost("revoke")]
    [Authorize]
    [SwaggerOperation(Summary = "Logout current device")]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequestDto request, CancellationToken ct)
    {
        await _mediator.Send(new RevokeTokenCommand(request.RefreshToken, _currentUser.IpAddress), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Token revoked."));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request, CancellationToken ct) =>
        Ok(ApiResponse<MessageResponseDto>.Ok(await _mediator.Send(new ForgotPasswordCommand(request, _currentUser.IpAddress), ct)));

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request, CancellationToken ct) =>
        Ok(ApiResponse<MessageResponseDto>.Ok(await _mediator.Send(new ResetPasswordCommand(request, _currentUser.IpAddress), ct)));

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        return Ok(ApiResponse<MessageResponseDto>.Ok(
            await _mediator.Send(new ChangePasswordCommand(userId, request, _currentUser.IpAddress), ct)));
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request, CancellationToken ct) =>
        Ok(ApiResponse<MessageResponseDto>.Ok(await _mediator.Send(new VerifyEmailCommand(request), ct)));

    [HttpPost("resend-email-verification")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmailVerification([FromBody] ResendEmailVerificationRequestDto request, CancellationToken ct) =>
        Ok(ApiResponse<MessageResponseDto>.Ok(await _mediator.Send(new ResendEmailVerificationCommand(request, _currentUser.IpAddress), ct)));

    [HttpPost("admin/verify-email")]
    [Authorize]
    [SwaggerOperation(Summary = "Administrator manually verifies user email")]
    public async Task<IActionResult> AdminVerifyEmail([FromBody] AdminVerifyEmailRequestDto request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        return Ok(ApiResponse<MessageResponseDto>.Ok(await _mediator.Send(new AdminVerifyEmailCommand(adminId, request), ct)));
    }

    [HttpPost("phone/send-otp")]
    [Authorize]
    public async Task<IActionResult> SendPhoneOtp([FromBody] SendPhoneOtpRequestDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        return Ok(ApiResponse<OtpSentResponseDto>.Ok(await _mediator.Send(new SendPhoneOtpCommand(userId, request, _currentUser.IpAddress), ct)));
    }

    [HttpPost("phone/verify-otp")]
    [Authorize]
    public async Task<IActionResult> VerifyPhoneOtp([FromBody] VerifyPhoneOtpRequestDto request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        return Ok(ApiResponse<MessageResponseDto>.Ok(await _mediator.Send(new VerifyPhoneOtpCommand(userId, request), ct)));
    }

    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Current authenticated user with roles and permissions")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        var permissions = await _mediator.Send(new GetMyPermissionsQuery(userId), ct);
        return Ok(ApiResponse<object>.Ok(new
        {
            _currentUser.UserId,
            _currentUser.Email,
            PrimaryRole = _currentUser.PrimaryRole,
            Roles = User?.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value) ?? [],
            Permissions = permissions,
            EmailVerified = _currentUser.EmailVerified,
            SessionId = _currentUser.SessionId,
        }));
    }

    [HttpGet("permissions")]
    [Authorize]
    public async Task<IActionResult> GetPermissions(CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("Not authenticated.");
        return Ok(ApiResponse<IReadOnlyList<string>>.Ok(await _mediator.Send(new GetMyPermissionsQuery(userId), ct)));
    }
}
