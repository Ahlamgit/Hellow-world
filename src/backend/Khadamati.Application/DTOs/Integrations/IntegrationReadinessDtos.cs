namespace Khadamati.Application.DTOs.Integrations;

public class IntegrationReadinessReportDto
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool ProductionReady { get; set; }
    public IReadOnlyList<ProviderReadinessDto> Providers { get; set; } = Array.Empty<ProviderReadinessDto>();
}

public class ProviderReadinessDto
{
    public string Category { get; set; } = string.Empty;
    public string SelectedProvider { get; set; } = string.Empty;
    /// <summary>Ready | Development | Misconfigured</summary>
    public string Status { get; set; } = string.Empty;
    public bool IsProductionReady { get; set; }
    public IReadOnlyList<string> MissingSettings { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Warnings { get; set; } = Array.Empty<string>();
}
