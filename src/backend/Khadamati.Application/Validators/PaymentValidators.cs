using FluentValidation;
using Khadamati.Application.DTOs.Payments;
using Khadamati.Application.Features.Payments.Commands;

namespace Khadamati.Application.Validators;

public class MoyasarWebhookValidator : AbstractValidator<MoyasarWebhookDto>
{
    public MoyasarWebhookValidator()
    {
        RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
    }
}

public class ProcessMoyasarWebhookCommandValidator : AbstractValidator<ProcessMoyasarWebhookCommand>
{
    public ProcessMoyasarWebhookCommandValidator()
    {
        RuleFor(x => x.RawBody).NotEmpty();
        RuleFor(x => x.Payload).NotNull();
    }
}
