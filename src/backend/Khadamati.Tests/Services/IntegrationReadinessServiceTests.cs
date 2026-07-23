using FluentAssertions;
using Khadamati.Infrastructure.Services.Integrations;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Tests.Services;

public class IntegrationReadinessServiceTests
{
    [Fact]
    public void GetReport_AllDevelopmentProviders_IsNotProductionReady()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var service = new IntegrationReadinessService(config);

        var report = service.GetReport();

        report.ProductionReady.Should().BeFalse();
        report.Providers.Should().HaveCount(4);
        report.Providers.Should().OnlyContain(p => p.Status == "Development");
    }

    [Fact]
    public void GetReport_MoyasarFullyConfigured_IsProductionReadyForPayment()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Provider"] = "Moyasar",
            ["Payment:Moyasar:SecretKey"] = "sk_test_123",
            ["Payment:Moyasar:PublishableKey"] = "pk_test_123",
            ["Push:Provider"] = "firebase",
            ["Push:Firebase:ServerKey"] = "fcm-key",
            ["Push:Apns:TeamId"] = "TEAM",
            ["Push:Apns:KeyId"] = "KEY",
            ["Push:Apns:BundleId"] = "com.khadamati.app",
            ["Push:Apns:PrivateKey"] = "-----BEGIN PRIVATE KEY-----\nabc\n-----END PRIVATE KEY-----",
            ["Email:Provider"] = "sendgrid",
            ["Email:SendGrid:ApiKey"] = "sg-key",
            ["Email:SendGrid:FromAddress"] = "noreply@khadamati.com",
            ["Sms:Provider"] = "twilio",
            ["Sms:Twilio:AccountSid"] = "sid",
            ["Sms:Twilio:AuthToken"] = "token",
            ["Sms:Twilio:FromNumber"] = "+10000000000",
        }).Build();

        var report = new IntegrationReadinessService(config).GetReport();

        report.ProductionReady.Should().BeTrue();
        report.Providers.Should().OnlyContain(p => p.Status == "Ready");
    }

    [Fact]
    public void GetReport_AreebaFullyConfigured_IsReadyForPayment()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Provider"] = "Areeba",
            ["Payment:Areeba:MerchantId"] = "MERCHANT",
            ["Payment:Areeba:ApiPassword"] = "secret",
            ["Push:Provider"] = "Development",
            ["Email:Provider"] = "Development",
            ["Sms:Provider"] = "Development",
        }).Build();

        var payment = new IntegrationReadinessService(config).GetReport().Providers
            .Single(p => p.Category == "Payment");

        payment.Status.Should().Be("Ready");
    }

    [Fact]
    public void GetReport_AreebaMissingMerchant_IsMisconfigured()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Payment:Provider"] = "Areeba",
        }).Build();

        var payment = new IntegrationReadinessService(config).GetReport().Providers
            .Single(p => p.Category == "Payment");

        payment.Status.Should().Be("Misconfigured");
        payment.MissingSettings.Should().Contain("Payment:Areeba:MerchantId");
    }
}
