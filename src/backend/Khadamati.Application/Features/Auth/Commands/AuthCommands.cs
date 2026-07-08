using AutoMapper;
using FluentValidation;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Auth;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequestDto Request, string? IpAddress) : IRequest<AuthResponseDto>;
public record LoginCommand(LoginRequestDto Request, string? IpAddress) : IRequest<AuthResponseDto>;
public record RefreshTokenCommand(RefreshTokenRequestDto Request, string? IpAddress) : IRequest<AuthResponseDto>;
public record RevokeTokenCommand(string RefreshToken, string? IpAddress) : IRequest<Unit>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequestDto> _validator;

    public RegisterCommandHandler(IAuthService authService, IValidator<RegisterRequestDto> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request.Request, cancellationToken);
        if (!validation.IsValid)
            throw new Common.ValidationException(validation.Errors.Select(e => e.ErrorMessage));

        return await _authService.RegisterAsync(request.Request, request.IpAddress, cancellationToken);
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequestDto> _validator;

    public LoginCommandHandler(IAuthService authService, IValidator<LoginRequestDto> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request.Request, cancellationToken);
        if (!validation.IsValid)
            throw new Common.ValidationException(validation.Errors.Select(e => e.ErrorMessage));

        return await _authService.LoginAsync(request.Request, request.IpAddress, cancellationToken);
    }
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
