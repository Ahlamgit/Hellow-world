using FluentAssertions;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Validators;

namespace Khadamati.Tests.Validators;

public class AdminUserValidatorTests
{
    [Fact]
    public void CreateAdminUserValidator_ShouldRejectInvalidRole()
    {
        var validator = new CreateAdminUserValidator();
        var result = validator.Validate(new CreateAdminUserDto
        {
            Email = "a@test.com",
            Phone = "+966500000000",
            Password = "Password1!",
            FirstName = "A",
            LastName = "B",
            Role = "InvalidRole",
            Status = "Active",
        });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void AssignUserRolesValidator_ShouldRequirePrimaryInRoles()
    {
        var validator = new AssignUserRolesValidator();
        var result = validator.Validate(new AssignUserRolesDto
        {
            Roles = ["Customer"],
            PrimaryRole = "Admin",
        });
        result.IsValid.Should().BeFalse();
    }
}
