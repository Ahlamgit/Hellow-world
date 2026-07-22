using FluentAssertions;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.Interfaces;
using Khadamati.Application.Validators;
using Moq;

namespace Khadamati.Tests.Validators;

public class AuthValidatorTests
{
    private readonly RegisterRequestValidator _registerValidator;
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly ChangePasswordRequestValidator _changePasswordValidator;

    public AuthValidatorTests()
    {
        var policyMock = new Mock<IPasswordPolicyService>();
        policyMock.Setup(p => p.ValidatePassword(It.IsAny<string>()))
            .Callback<string>(p =>
            {
                if (p.Length < 8) throw new Exception("Too short");
            });
        _registerValidator = new RegisterRequestValidator(policyMock.Object);
        _changePasswordValidator = new ChangePasswordRequestValidator(policyMock.Object);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldPass()
    {
        var dto = new RegisterRequestDto("user@test.com", "+966501234567", "Password1!", "Password1!", "John", "Doe", "Customer");
        var result = await _registerValidator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldFail()
    {
        var dto = new RegisterRequestDto("user@test.com", "+966501234567", "weak", "weak", "John", "Doe", "Customer");
        var result = await _registerValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Register_WithAdminRole_ShouldFail()
    {
        var dto = new RegisterRequestDto("admin@test.com", "+966501234567", "Password1!", "Password1!", "Admin", "User", "Administrator");
        var result = await _registerValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WithValidData_ShouldPass()
    {
        var dto = new LoginRequestDto("user@test.com", "Password1!", true);
        var result = await _loginValidator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Register_WithMismatchedConfirmPassword_ShouldFail()
    {
        var dto = new RegisterRequestDto("user@test.com", "+966501234567", "Password1!", "Different1!", "John", "Doe", "Customer");
        var result = await _registerValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ChangePassword_WithMismatchedConfirm_ShouldFail()
    {
        var dto = new ChangePasswordRequestDto("OldPass1!", "NewPass1!", "Different1!");
        var result = await _changePasswordValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyOtp_WithInvalidFormat_ShouldFail()
    {
        var validator = new VerifyPhoneOtpRequestValidator();
        var dto = new VerifyPhoneOtpRequestDto("+966501234567", "abc");
        var result = await validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyOtp_WithValidFormat_ShouldPass()
    {
        var validator = new VerifyPhoneOtpRequestValidator();
        var dto = new VerifyPhoneOtpRequestDto("+966501234567", "123456");
        var result = await validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }
}
