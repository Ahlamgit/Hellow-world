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
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
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
