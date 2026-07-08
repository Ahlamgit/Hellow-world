using FluentValidation;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;

namespace Khadamati.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator(IPasswordPolicyService passwordPolicy)
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[0-9]{10,15}$");
        RuleFor(x => x.Password).NotEmpty().Custom((p, ctx) =>
        {
            try { passwordPolicy.ValidatePassword(p); }
            catch (Exception ex) { ctx.AddFailure(ex.Message); }
        });
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Role).NotEmpty()
            .Must(r => RoleNames.SelfRegistrationRoles.Contains(RoleNames.MapLegacyRole(r), StringComparer.OrdinalIgnoreCase));
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
    public ForgotPasswordRequestValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator(IPasswordPolicyService passwordPolicy)
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().Custom((p, ctx) =>
        {
            try { passwordPolicy.ValidatePassword(p); }
            catch (Exception ex) { ctx.AddFailure(ex.Message); }
        });
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword);
    }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator(IPasswordPolicyService passwordPolicy)
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().Custom((p, ctx) =>
        {
            try { passwordPolicy.ValidatePassword(p); }
            catch (Exception ex) { ctx.AddFailure(ex.Message); }
        });
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword);
        RuleFor(x => x.NewPassword).NotEqual(x => x.CurrentPassword);
    }
}

public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequestDto>
{
    public VerifyEmailRequestValidator() => RuleFor(x => x.Token).NotEmpty();
}

public class ResendEmailVerificationRequestValidator : AbstractValidator<ResendEmailVerificationRequestDto>
{
    public ResendEmailVerificationRequestValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}

public class SendPhoneOtpRequestValidator : AbstractValidator<SendPhoneOtpRequestDto>
{
    public SendPhoneOtpRequestValidator() => RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[0-9]{10,15}$");
}

public class VerifyPhoneOtpRequestValidator : AbstractValidator<VerifyPhoneOtpRequestDto>
{
    public VerifyPhoneOtpRequestValidator()
    {
        RuleFor(x => x.Phone).NotEmpty();
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

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequestDto>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PreferredLanguage).Must(l => l is "ar" or "en");
        RuleFor(x => x.Timezone).NotEmpty();
    }
}

public class AdminVerifyEmailRequestValidator : AbstractValidator<AdminVerifyEmailRequestDto>
{
    public AdminVerifyEmailRequestValidator() => RuleFor(x => x.UserId).NotEmpty();
}
