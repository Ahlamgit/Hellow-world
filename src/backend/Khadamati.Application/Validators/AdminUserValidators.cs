using FluentValidation;
using Khadamati.Application.DTOs.Users;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Enums;

namespace Khadamati.Application.Validators;

public class AdminUserListQueryValidator : AbstractValidator<AdminUserListQueryDto>
{
    public AdminUserListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortDirection).Must(d => d is "asc" or "desc").WithMessage("SortDirection must be asc or desc.");
        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrWhiteSpace(s) || Enum.TryParse<UserStatus>(s, true, out _))
            .WithMessage("Invalid status.");
        RuleFor(x => x.Role)
            .Must(r => string.IsNullOrWhiteSpace(r) || RoleNames.All.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid role.");
    }
}

public class CreateAdminUserValidator : AbstractValidator<CreateAdminUserDto>
{
    public CreateAdminUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => RoleNames.All.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid role.");
        RuleFor(x => x.PreferredLanguage).Must(l => l is "ar" or "en");
        RuleFor(x => x.Status)
            .Must(s => Enum.TryParse<UserStatus>(s, true, out _))
            .WithMessage("Invalid status.");
    }
}

public class UpdateAdminUserValidator : AbstractValidator<UpdateAdminUserDto>
{
    public UpdateAdminUserValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PreferredLanguage).Must(l => l is "ar" or "en");
        RuleFor(x => x.Status)
            .Must(s => Enum.TryParse<UserStatus>(s, true, out _))
            .WithMessage("Invalid status.");
    }
}

public class AssignUserRolesValidator : AbstractValidator<AssignUserRolesDto>
{
    public AssignUserRolesValidator()
    {
        RuleFor(x => x.PrimaryRole)
            .NotEmpty()
            .Must(r => RoleNames.All.Contains(r, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.Roles)
            .NotEmpty()
            .Must(roles => roles.All(r => RoleNames.All.Contains(r, StringComparer.OrdinalIgnoreCase)))
            .WithMessage("All roles must be valid.");
        RuleFor(x => x)
            .Must(d => d.Roles.Contains(d.PrimaryRole, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Primary role must be included in roles list.");
    }
}

public class SuspendUserValidator : AbstractValidator<SuspendUserDto>
{
    public SuspendUserValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
