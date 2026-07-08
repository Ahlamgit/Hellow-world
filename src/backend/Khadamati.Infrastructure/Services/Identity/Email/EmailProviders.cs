using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Identity.Email;

public class DevelopmentEmailProvider : IEmailProvider
{
    private readonly ILogger<DevelopmentEmailProvider> _logger;

    public DevelopmentEmailProvider(ILogger<DevelopmentEmailProvider> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DEV EMAIL to {To} | {Subject} | {Body}", to, subject, htmlBody);
        return Task.CompletedTask;
    }
}

public class SmtpEmailProvider : IEmailProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailProvider> _logger;

    public SmtpEmailProvider(IConfiguration configuration, ILogger<SmtpEmailProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var client = new System.Net.Mail.SmtpClient(
            _configuration["Email:Smtp:Host"],
            _configuration.GetValue("Email:Smtp:Port", 587))
        {
            EnableSsl = _configuration.GetValue("Email:Smtp:EnableSsl", true),
            Credentials = new System.Net.NetworkCredential(
                _configuration["Email:Smtp:Username"],
                _configuration["Email:Smtp:Password"])
        };

        var from = _configuration["Email:Smtp:FromAddress"] ?? "noreply@khadamati.com";
        var message = new System.Net.Mail.MailMessage(from, to, subject, htmlBody) { IsBodyHtml = true };
        await client.SendMailAsync(message, cancellationToken);
        _logger.LogInformation("SMTP email sent to {To}", to);
    }
}

public class SendGridEmailProvider : IEmailProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailProvider> _logger;
    private readonly HttpClient _httpClient;

    public SendGridEmailProvider(IConfiguration configuration, ILogger<SendGridEmailProvider> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Email:SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("Email:SendGrid:ApiKey is not configured.");
        var from = _configuration["Email:SendGrid:FromAddress"] ?? "noreply@khadamati.com";
        var fromName = _configuration["Email:SendGrid:FromName"] ?? "Khadamati";

        var payload = System.Text.Json.JsonSerializer.Serialize(new
        {
            personalizations = new[] { new { to = new[] { new { email = to } } } },
            from = new { email = from, name = fromName },
            subject,
            content = new[] { new { type = "text/html", value = htmlBody } }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("SendGrid email sent to {To}", to);
    }
}

public class EmailService : IEmailService
{
    private readonly IEmailProvider _provider;
    private readonly IConfiguration _configuration;

    public EmailService(IEmailProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    public Task SendEmailVerificationAsync(string email, string firstName, string verificationToken, string language, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["App:WebBaseUrl"] ?? "http://localhost:3000";
        var link = $"{baseUrl}/verify-email?token={verificationToken}";
        var subject = language == "ar" ? "تأكيد البريد الإلكتروني - خدماتي" : "Verify your email - Khadamati";
        var body = language == "ar"
            ? $"<p>مرحباً {firstName}،</p><p>يرجى تأكيد بريدك الإلكتروني: <a href=\"{link}\">{link}</a></p>"
            : $"<p>Hello {firstName},</p><p>Please verify your email: <a href=\"{link}\">{link}</a></p>";
        return _provider.SendAsync(email, subject, body, cancellationToken);
    }

    public Task SendPasswordResetAsync(string email, string firstName, string resetToken, string language, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["App:WebBaseUrl"] ?? "http://localhost:3000";
        var link = $"{baseUrl}/reset-password?token={resetToken}";
        var subject = language == "ar" ? "إعادة تعيين كلمة المرور - خدماتي" : "Reset your password - Khadamati";
        var body = language == "ar"
            ? $"<p>مرحباً {firstName}،</p><p>أعد تعيين كلمة المرور: <a href=\"{link}\">{link}</a></p>"
            : $"<p>Hello {firstName},</p><p>Reset your password: <a href=\"{link}\">{link}</a></p>";
        return _provider.SendAsync(email, subject, body, cancellationToken);
    }
}
