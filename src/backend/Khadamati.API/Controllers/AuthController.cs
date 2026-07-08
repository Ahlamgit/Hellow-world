using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Auth;
using Khadamati.Application.Features.Auth.Commands;
using Khadamati.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Khadamati.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints for KHADAMATI platform.
/// Supports registration, login, JWT tokens, email/phone verification, and password management.
/// </summary>
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

    /// <summary>Register a new Customer, Craftsman, or Store account.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Register new user", Description = "Creates account with BCrypt-hashed password. Sends email verification. Returns JWT tokens.")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RegisterCommand(request, _currentUser.IpAddress), cancellationToken);
        return Created(string.Empty, ApiResponse<AuthResponseDto>.Ok(result, "Registration successful. Please verify your email."));
    }

    /// <summary>Authenticate with email and password. Supports Remember Me for extended refresh token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login", Description = "Returns JWT access token and refresh token. Set RememberMe=true for 30-day refresh token.")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request, _currentUser.IpAddress), cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    /// <summary>Refresh an expired access token using a valid refresh token.</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Refresh JWT token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request, _currentUser.IpAddress), cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    /// <summary>Revoke a refresh token (logout).</summary>
    [HttpPost("revoke")]
    [Authorize]
    [SwaggerOperation(Summary = "Revoke refresh token (logout)")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequestDto request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RevokeTokenCommand(request.RefreshToken, _currentUser.IpAddress), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Token revoked successfully."));
    }

    /// <summary>Request a password reset email.</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Forgot password", Description = "Sends password reset link if email exists. Always returns success to prevent enumeration.")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ForgotPasswordCommand(request, _currentUser.IpAddress), cancellationToken);
        return Ok(ApiResponse<MessageResponseDto>.Ok(result));
    }

    /// <summary>Reset password using token from email.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Reset password")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ResetPasswordCommand(request, _currentUser.IpAddress), cancellationToken);
        return Ok(ApiResponse<MessageResponseDto>.Ok(result));
    }

    /// <summary>Change password for authenticated user.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [SwaggerOperation(Summary = "Change password", Description = "Requires current password. Revokes all refresh tokens.")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("User not authenticated.");
        var result = await _mediator.Send(new ChangePasswordCommand(userId, request), cancellationToken);
        return Ok(ApiResponse<MessageResponseDto>.Ok(result));
    }

    /// <summary>Verify email address using token from verification email.</summary>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Verify email address")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new VerifyEmailCommand(request), cancellationToken);
        return Ok(ApiResponse<MessageResponseDto>.Ok(result));
    }

    /// <summary>Resend email verification link.</summary>
    [HttpPost("resend-email-verification")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Resend email verification")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendEmailVerification([FromBody] ResendEmailVerificationRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ResendEmailVerificationCommand(request, _currentUser.IpAddress), cancellationToken);
        return Ok(ApiResponse<MessageResponseDto>.Ok(result));
    }

    /// <summary>Send OTP to phone number for verification.</summary>
    [HttpPost("phone/send-otp")]
    [Authorize]
    [SwaggerOperation(Summary = "Send phone OTP", Description = "Sends 6-digit OTP via SMS. Rate limited to 5 requests per hour.")]
    [ProducesResponseType(typeof(ApiResponse<OtpSentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendPhoneOtp([FromBody] SendPhoneOtpRequestDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("User not authenticated.");
        var result = await _mediator.Send(new SendPhoneOtpCommand(userId, request, _currentUser.IpAddress), cancellationToken);
        return Ok(ApiResponse<OtpSentResponseDto>.Ok(result));
    }

    /// <summary>Verify phone number using OTP code.</summary>
    [HttpPost("phone/verify-otp")]
    [Authorize]
    [SwaggerOperation(Summary = "Verify phone OTP")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyPhoneOtp([FromBody] VerifyPhoneOtpRequestDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException("User not authenticated.");
        var result = await _mediator.Send(new VerifyPhoneOtpCommand(userId, request), cancellationToken);
        return Ok(ApiResponse<MessageResponseDto>.Ok(result));
    }

    /// <summary>Get current authenticated user info from JWT.</summary>
    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Get current auth user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser()
    {
        return Ok(ApiResponse<object>.Ok(new
        {
            _currentUser.UserId,
            _currentUser.Email,
            _currentUser.Role
        }));
    }
}
