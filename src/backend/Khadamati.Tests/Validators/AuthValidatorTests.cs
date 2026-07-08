using FluentAssertions;
using Khadamati.Application.DTOs.Auth;
using Khadamati.Application.Validators;

namespace Khadamati.Tests.Validators;

public class AuthValidatorTests
{
    private readonly RegisterRequestValidator _registerValidator = new();
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly ChangePasswordRequestValidator _changePasswordValidator = new();
    private readonly VerifyPhoneOtpRequestValidator _otpValidator = new();

    [Fact]
    public async Task Register_WithValidData_ShouldPass()
    {
        var dto = new RegisterRequestDto("user@test.com", "+966501234567", "Password1!", "John", "Doe", "Customer");
        var result = await _registerValidator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldFail()
    {
        var dto = new RegisterRequestDto("user@test.com", "+966501234567", "weak", "John", "Doe", "Customer");
        var result = await _registerValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Register_WithAdminRole_ShouldFail()
    {
        var dto = new RegisterRequestDto("admin@test.com", "+966501234567", "Password1!", "Admin", "User", "Administrator");
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
    public async Task ChangePassword_WithMismatchedConfirm_ShouldFail()
    {
        var dto = new ChangePasswordRequestDto("OldPass1!", "NewPass1!", "Different1!");
        var result = await _changePasswordValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyOtp_WithInvalidFormat_ShouldFail()
    {
        var dto = new VerifyPhoneOtpRequestDto("+966501234567", "abc");
        var result = await _otpValidator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyOtp_WithValidFormat_ShouldPass()
    {
        var dto = new VerifyPhoneOtpRequestDto("+966501234567", "123456");
        var result = await _otpValidator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }
}
