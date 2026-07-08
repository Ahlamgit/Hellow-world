using FluentAssertions;
using FluentValidation.TestHelper;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Validators;

namespace Khadamati.Tests.Validators;

public class AdminValidatorTests
{
    private readonly AdminListQueryValidator _listValidator = new();
    private readonly AdminBulkActionValidator _bulkValidator = new();

    [Fact]
    public void ListQuery_InvalidPage_Fails()
    {
        var result = _listValidator.TestValidate(new AdminListQueryDto { Page = 0, PageSize = 10 });
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void ListQuery_InvalidPageSize_Fails()
    {
        var result = _listValidator.TestValidate(new AdminListQueryDto { Page = 1, PageSize = 1000 });
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void ListQuery_ValidInput_Passes()
    {
        var result = _listValidator.TestValidate(new AdminListQueryDto { Page = 1, PageSize = 25, SortDirection = "asc" });
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void BulkAction_InvalidAction_Fails()
    {
        var result = _bulkValidator.TestValidate(new AdminBulkActionDto { Action = "invalid", Ids = ["id1"] });
        result.ShouldHaveValidationErrorFor(x => x.Action);
    }

    [Fact]
    public void BulkAction_EmptyIds_Fails()
    {
        var result = _bulkValidator.TestValidate(new AdminBulkActionDto { Action = "activate", Ids = [] });
        result.ShouldHaveValidationErrorFor(x => x.Ids);
    }

    [Fact]
    public void BulkAction_ValidInput_Passes()
    {
        var result = _bulkValidator.TestValidate(new AdminBulkActionDto { Action = "activate", Ids = [Guid.NewGuid().ToString()] });
        result.ShouldNotHaveAnyValidationErrors();
    }
}
