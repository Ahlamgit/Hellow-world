using Khadamati.Application.DTOs.Integrations;

namespace Khadamati.Application.Interfaces;

public interface IIntegrationReadinessService
{
    IntegrationReadinessReportDto GetReport();
}
