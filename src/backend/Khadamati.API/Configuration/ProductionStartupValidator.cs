using Khadamati.Application.Interfaces;

namespace Khadamati.API.Configuration;

public static class ProductionStartupValidator
{
    public static void ValidateJwtSecret(IConfiguration configuration, IHostEnvironment environment)
    {
        if (!environment.IsProduction())
            return;

        var secret = configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret)
            || secret.Length < 32
            || secret.Contains("${", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Jwt:Secret must be set to at least 32 characters in Production. " +
                "Use the Jwt__Secret environment variable.");
        }
    }

    public static void EnsureIntegrationsReadyIfRequired(
        IConfiguration configuration,
        IHostEnvironment environment,
        IIntegrationReadinessService readiness)
    {
        if (!environment.IsProduction())
            return;

        if (!configuration.GetValue("Integrations:RequireProductionReady", false))
            return;

        var report = readiness.GetReport();
        if (report.ProductionReady)
            return;

        var blockers = report.Providers
            .Where(p => !p.IsProductionReady)
            .Select(p => $"{p.Category} ({p.Status})")
            .ToList();

        throw new InvalidOperationException(
            "Integrations:RequireProductionReady is enabled but providers are not ready: "
            + string.Join(", ", blockers));
    }
}
