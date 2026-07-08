using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Identity.Sms;

public class DevelopmentSmsProvider : ISmsProvider
{
    private readonly ILogger<DevelopmentSmsProvider> _logger;

    public DevelopmentSmsProvider(ILogger<DevelopmentSmsProvider> logger) => _logger = logger;

    public Task SendAsync(string phone, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DEV SMS to {Phone}: {Message}", phone, message);
        return Task.CompletedTask;
    }
}

public class TwilioSmsProvider : ISmsProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwilioSmsProvider> _logger;
    private readonly HttpClient _httpClient;

    public TwilioSmsProvider(IConfiguration configuration, ILogger<TwilioSmsProvider> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendAsync(string phone, string message, CancellationToken cancellationToken = default)
    {
        var accountSid = _configuration["Sms:Twilio:AccountSid"]
            ?? throw new InvalidOperationException("Sms:Twilio:AccountSid is not configured.");
        var authToken = _configuration["Sms:Twilio:AuthToken"]
            ?? throw new InvalidOperationException("Sms:Twilio:AuthToken is not configured.");
        var from = _configuration["Sms:Twilio:FromNumber"]
            ?? throw new InvalidOperationException("Sms:Twilio:FromNumber is not configured.");

        var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json";
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["To"] = phone,
            ["From"] = from,
            ["Body"] = message
        });

        var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
        using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Twilio SMS sent to {Phone}", phone);
    }
}

public class SmsService : ISmsService
{
    private readonly ISmsProvider _provider;

    public SmsService(ISmsProvider provider) => _provider = provider;

    public Task SendOtpAsync(string phone, string otp, string language, CancellationToken cancellationToken = default)
    {
        var message = language == "ar"
            ? $"رمز التحقق الخاص بك في خدماتي: {otp}"
            : $"Your Khadamati verification code: {otp}";
        return _provider.SendAsync(phone, message, cancellationToken);
    }
}
