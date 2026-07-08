using FluentValidation;
using Khadamati.Application.DTOs.Admin;

namespace Khadamati.Application.Validators;

public class AdminListQueryValidator : AbstractValidator<AdminListQueryDto>
{
  public AdminListQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
    RuleFor(x => x.PageSize).InclusiveBetween(1, 500);
    RuleFor(x => x.SortDirection).Must(d => d.Equals("asc", StringComparison.OrdinalIgnoreCase) || d.Equals("desc", StringComparison.OrdinalIgnoreCase));
  }
}

public class AdminBulkActionValidator : AbstractValidator<AdminBulkActionDto>
{
  private static readonly string[] ValidActions = ["delete", "activate", "deactivate", "suspend", "archive", "markread", "resolve", "close"];

  public AdminBulkActionValidator()
  {
    RuleFor(x => x.Action).NotEmpty().Must(a => ValidActions.Contains(a, StringComparer.OrdinalIgnoreCase));
    RuleFor(x => x.Ids).NotEmpty();
  }
}
