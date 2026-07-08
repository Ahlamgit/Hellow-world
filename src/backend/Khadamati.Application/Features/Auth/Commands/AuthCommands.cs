using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Auth;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequestDto Request, string? IpAddress) : IRequest<AuthResponseDto>;
public record LoginCommand(LoginRequestDto Request, string? IpAddress) : IRequest<AuthResponseDto>;
public record RefreshTokenCommand(RefreshTokenRequestDto Request, string? IpAddress) : IRequest<AuthResponseDto>;
public record RevokeTokenCommand(string RefreshToken, string? IpAddress) : IRequest<Unit>;
public record ForgotPasswordCommand(ForgotPasswordRequestDto Request, string? IpAddress) : IRequest<MessageResponseDto>;
public record ResetPasswordCommand(ResetPasswordRequestDto Request, string? IpAddress) : IRequest<MessageResponseDto>;
public record ChangePasswordCommand(Guid UserId, ChangePasswordRequestDto Request) : IRequest<MessageResponseDto>;
public record VerifyEmailCommand(VerifyEmailRequestDto Request) : IRequest<MessageResponseDto>;
public record ResendEmailVerificationCommand(ResendEmailVerificationRequestDto Request, string? IpAddress) : IRequest<MessageResponseDto>;
public record SendPhoneOtpCommand(Guid UserId, SendPhoneOtpRequestDto Request, string? IpAddress) : IRequest<OtpSentResponseDto>;
public record VerifyPhoneOtpCommand(Guid UserId, VerifyPhoneOtpRequestDto Request) : IRequest<MessageResponseDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    public RegisterCommandHandler(IAuthService authService) => _authService = authService;
    public Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken) =>
        _authService.RegisterAsync(request.Request, request.IpAddress, cancellationToken);
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    public LoginCommandHandler(IAuthService authService) => _authService = authService;
    public Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken) =>
        _authService.LoginAsync(request.Request, request.IpAddress, cancellationToken);
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    public RefreshTokenCommandHandler(IAuthService authService) => _authService = authService;
    public Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        _authService.RefreshTokenAsync(request.Request, request.IpAddress, cancellationToken);
}

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly IAuthService _authService;
    public RevokeTokenCommandHandler(IAuthService authService) => _authService = authService;
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        await _authService.RevokeTokenAsync(request.RefreshToken, request.IpAddress, cancellationToken);
        return Unit.Value;
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, MessageResponseDto>
{
    private readonly IAuthService _authService;
    public ForgotPasswordCommandHandler(IAuthService authService) => _authService = authService;
    public Task<MessageResponseDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken) =>
        _authService.ForgotPasswordAsync(request.Request, request.IpAddress, cancellationToken);
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, MessageResponseDto>
{
    private readonly IAuthService _authService;
    public ResetPasswordCommandHandler(IAuthService authService) => _authService = authService;
    public Task<MessageResponseDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken) =>
        _authService.ResetPasswordAsync(request.Request, request.IpAddress, cancellationToken);
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, MessageResponseDto>
{
    private readonly IAuthService _authService;
    public ChangePasswordCommandHandler(IAuthService authService) => _authService = authService;
    public Task<MessageResponseDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken) =>
        _authService.ChangePasswordAsync(request.UserId, request.Request, cancellationToken);
}

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, MessageResponseDto>
{
    private readonly IAuthService _authService;
    public VerifyEmailCommandHandler(IAuthService authService) => _authService = authService;
    public Task<MessageResponseDto> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) =>
        _authService.VerifyEmailAsync(request.Request, cancellationToken);
}

public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, MessageResponseDto>
{
    private readonly IAuthService _authService;
    public ResendEmailVerificationCommandHandler(IAuthService authService) => _authService = authService;
    public Task<MessageResponseDto> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken) =>
        _authService.ResendEmailVerificationAsync(request.Request, request.IpAddress, cancellationToken);
}

public class SendPhoneOtpCommandHandler : IRequestHandler<SendPhoneOtpCommand, OtpSentResponseDto>
{
    private readonly IAuthService _authService;
    public SendPhoneOtpCommandHandler(IAuthService authService) => _authService = authService;
    public Task<OtpSentResponseDto> Handle(SendPhoneOtpCommand request, CancellationToken cancellationToken) =>
        _authService.SendPhoneOtpAsync(request.UserId, request.Request, request.IpAddress, cancellationToken);
}

public class VerifyPhoneOtpCommandHandler : IRequestHandler<VerifyPhoneOtpCommand, MessageResponseDto>
{
    private readonly IAuthService _authService;
    public VerifyPhoneOtpCommandHandler(IAuthService authService) => _authService = authService;
    public Task<MessageResponseDto> Handle(VerifyPhoneOtpCommand request, CancellationToken cancellationToken) =>
        _authService.VerifyPhoneOtpAsync(request.UserId, request.Request, cancellationToken);
}
