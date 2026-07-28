using Khadamati.Application.DTOs.Payments;

namespace Khadamati.Application.Interfaces;

public interface IAreebaWebhookSignatureValidator
{
    void Validate(AreebaWebhookContext context);
}
