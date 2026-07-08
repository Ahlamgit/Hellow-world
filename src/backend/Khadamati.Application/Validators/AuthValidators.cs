using FluentValidation;
using Khadamati.Application.DTOs.Auth;

namespace Khadamati.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    private static readonly string[] AllowedRoles = ["Customer", "Craftsman", "Store"];

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[0-9]{8,15}$");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("Password must contain at least one special character.");
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Role).NotEmpty().Must(r => AllowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.PreferredLanguage).Must(l => l is "ar" or "en");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestValidator() =>
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128)
            .Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128)
            .Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        RuleFor(x => x.NewPassword).NotEqual(x => x.CurrentPassword).WithMessage("New password must differ from current password.");
    }
}

public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequestDto>
{
    public VerifyEmailRequestValidator() => RuleFor(x => x.Token).NotEmpty();
}

public class ResendEmailVerificationRequestValidator : AbstractValidator<ResendEmailVerificationRequestDto>
{
    public ResendEmailVerificationRequestValidator() =>
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
}

public class SendPhoneOtpRequestValidator : AbstractValidator<SendPhoneOtpRequestDto>
{
    public SendPhoneOtpRequestValidator() =>
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[0-9]{8,15}$");
}

public class VerifyPhoneOtpRequestValidator : AbstractValidator<VerifyPhoneOtpRequestDto>
{
    public VerifyPhoneOtpRequestValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[0-9]{8,15}$");
        RuleFor(x => x.Otp).NotEmpty().Length(6).Matches(@"^\d{6}$");
    }
}

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequestDto>
{
    public RevokeTokenRequestValidator() => RuleFor(x => x.RefreshToken).NotEmpty();
}
