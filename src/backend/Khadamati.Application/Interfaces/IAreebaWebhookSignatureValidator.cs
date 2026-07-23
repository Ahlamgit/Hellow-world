namespace Khadamati.Application.Interfaces;

public interface IAreebaWebhookSignatureValidator
{
    void Validate(string? signature, string rawBody);
}
