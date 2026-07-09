using Khadamati.Application.DTOs.Integrations;
using Khadamati.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Infrastructure.Services.Integrations;

public class IntegrationReadinessService : IIntegrationReadinessService
{
    private readonly IConfiguration _configuration;

    public IntegrationReadinessService(IConfiguration configuration) => _configuration = configuration;

    public IntegrationReadinessReportDto GetReport()
    {
        var providers = new[]
        {
            EvaluatePayment(),
            EvaluatePush(),
            EvaluateEmail(),
            EvaluateSms(),
        };

        return new IntegrationReadinessReportDto
        {
            Timestamp = DateTime.UtcNow,
            Providers = providers,
            ProductionReady = providers.All(p => p.IsProductionReady),
        };
    }

    private ProviderReadinessDto EvaluatePayment()
    {
        var provider = (_configuration["Payment:Provider"] ?? "Development").Trim();
        if (provider.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            return Development("Payment", provider);
        }

        if (!provider.Equals("Moyasar", StringComparison.OrdinalIgnoreCase))
        {
            return Misconfigured("Payment", provider, ["Payment:Provider"]);
        }

        var missing = MissingWhenEmpty(
            ("Payment:Moyasar:SecretKey", "Payment:Moyasar:SecretKey"),
            ("Payment:Moyasar:PublishableKey", "Payment:Moyasar:PublishableKey"));

        var warnings = new List<string>();
        if (string.IsNullOrWhiteSpace(_configuration["Payment:Moyasar:WebhookSecret"]))
            warnings.Add("Payment:Moyasar:WebhookSecret is not set — webhooks will not be signature-validated.");

        if (missing.Count > 0)
            return Misconfigured("Payment", provider, missing, warnings);

        return Ready("Payment", provider, warnings);
    }

    private ProviderReadinessDto EvaluatePush()
    {
        var provider = (_configuration["Push:Provider"] ?? "Development").Trim();
        if (provider.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            return Development("Push", provider);
        }

        if (!provider.Equals("firebase", StringComparison.OrdinalIgnoreCase))
        {
            return Misconfigured("Push", provider, ["Push:Provider"]);
        }

        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(_configuration["Push:Firebase:ServerKey"]))
            missing.Add("Push:Firebase:ServerKey");

        var apnsMissing = MissingWhenEmpty(
            ("Push:Apns:TeamId", "Push:Apns:TeamId"),
            ("Push:Apns:KeyId", "Push:Apns:KeyId"),
            ("Push:Apns:BundleId", "Push:Apns:BundleId"),
            ("Push:Apns:PrivateKey", "Push:Apns:PrivateKey"));

        var warnings = new List<string>();
        if (apnsMissing.Count > 0)
            warnings.Add("APNs is not fully configured — iOS push delivery will be skipped.");

        if (missing.Count > 0)
            return Misconfigured("Push", provider, missing, warnings);

        return Ready("Push", provider, warnings, apnsMissing.Count == 0);
    }

    private ProviderReadinessDto EvaluateEmail()
    {
        var provider = (_configuration["Email:Provider"] ?? "Development").Trim();
        if (provider.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            return Development("Email", provider);
        }

        if (provider.Equals("smtp", StringComparison.OrdinalIgnoreCase))
        {
            var missing = MissingWhenEmpty(
                ("Email:Smtp:Host", "Email:Smtp:Host"),
                ("Email:Smtp:Username", "Email:Smtp:Username"),
                ("Email:Smtp:Password", "Email:Smtp:Password"),
                ("Email:Smtp:FromAddress", "Email:Smtp:FromAddress"));

            return missing.Count > 0
                ? Misconfigured("Email", provider, missing)
                : Ready("Email", provider);
        }

        if (provider.Equals("sendgrid", StringComparison.OrdinalIgnoreCase))
        {
            var missing = MissingWhenEmpty(
                ("Email:SendGrid:ApiKey", "Email:SendGrid:ApiKey"),
                ("Email:SendGrid:FromAddress", "Email:SendGrid:FromAddress"));

            return missing.Count > 0
                ? Misconfigured("Email", provider, missing)
                : Ready("Email", provider);
        }

        return Misconfigured("Email", provider, ["Email:Provider"]);
    }

    private ProviderReadinessDto EvaluateSms()
    {
        var provider = (_configuration["Sms:Provider"] ?? "Development").Trim();
        if (provider.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            return Development("Sms", provider);
        }

        if (!provider.Equals("twilio", StringComparison.OrdinalIgnoreCase))
        {
            return Misconfigured("Sms", provider, ["Sms:Provider"]);
        }

        var missing = MissingWhenEmpty(
            ("Sms:Twilio:AccountSid", "Sms:Twilio:AccountSid"),
            ("Sms:Twilio:AuthToken", "Sms:Twilio:AuthToken"),
            ("Sms:Twilio:FromNumber", "Sms:Twilio:FromNumber"));

        return missing.Count > 0
            ? Misconfigured("Sms", provider, missing)
            : Ready("Sms", provider);
    }

    private static ProviderReadinessDto Development(string category, string provider) => new()
    {
        Category = category,
        SelectedProvider = provider,
        Status = "Development",
        IsProductionReady = false,
    };

    private static ProviderReadinessDto Ready(
        string category,
        string provider,
        IReadOnlyList<string>? warnings = null,
        bool productionReady = true) => new()
    {
        Category = category,
        SelectedProvider = provider,
        Status = "Ready",
        IsProductionReady = productionReady,
        Warnings = warnings ?? Array.Empty<string>(),
    };

    private static ProviderReadinessDto Misconfigured(
        string category,
        string provider,
        IReadOnlyList<string> missing,
        IReadOnlyList<string>? warnings = null) => new()
    {
        Category = category,
        SelectedProvider = provider,
        Status = "Misconfigured",
        IsProductionReady = false,
        MissingSettings = missing,
        Warnings = warnings ?? Array.Empty<string>(),
    };

    private List<string> MissingWhenEmpty(params (string key, string label)[] settings)
    {
        var missing = new List<string>();
        foreach (var (key, label) in settings)
        {
            if (string.IsNullOrWhiteSpace(_configuration[key]))
                missing.Add(label);
        }

        return missing;
    }
}
