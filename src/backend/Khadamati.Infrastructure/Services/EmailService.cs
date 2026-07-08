using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task SendEmailVerificationAsync(string email, string firstName, string verificationToken, string language, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["App:WebBaseUrl"] ?? "http://localhost:3000";
        var link = $"{baseUrl}/verify-email?token={verificationToken}";
        var subject = language == "ar" ? "تأكيد البريد الإلكتروني - خدماتي" : "Verify your email - KHADAMATI";

        _logger.LogInformation(
            "EMAIL [Verification] To: {Email}, Subject: {Subject}, Link: {Link}, User: {FirstName}",
            email, subject, link, firstName);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(string email, string firstName, string resetToken, string language, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["App:WebBaseUrl"] ?? "http://localhost:3000";
        var link = $"{baseUrl}/reset-password?token={resetToken}";
        var subject = language == "ar" ? "إعادة تعيين كلمة المرور - خدماتي" : "Reset your password - KHADAMATI";

        _logger.LogInformation(
            "EMAIL [PasswordReset] To: {Email}, Subject: {Subject}, Link: {Link}, User: {FirstName}",
            email, subject, link, firstName);

        return Task.CompletedTask;
    }
}

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger) => _logger = logger;

    public Task SendOtpAsync(string phone, string otp, string language, CancellationToken cancellationToken = default)
    {
        var message = language == "ar"
            ? $"رمز التحقق الخاص بك في خدماتي: {otp}"
            : $"Your KHADAMATI verification code: {otp}";

        _logger.LogInformation("SMS [OTP] To: {Phone}, Message: {Message}", phone, message);
        return Task.CompletedTask;
    }
}
