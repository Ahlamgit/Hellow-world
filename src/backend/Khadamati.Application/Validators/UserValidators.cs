using FluentValidation;
using Khadamati.Application.DTOs.Identity;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Features.Identity.Commands;
using Khadamati.Application.Features.Users.Queries;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;

namespace Khadamati.Application.Validators;

public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressDtoValidator(ILocationCatalogService locations)
    {
        RuleFor(x => x.Label).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Country)
            .NotEmpty()
            .Must(PlatformConstants.IsSupportedCountry)
            .WithMessage($"Country must be {PlatformConstants.DefaultCountryCode}.");
        RuleFor(x => x.City)
            .NotEmpty()
            .MustAsync(async (city, ct) => await locations.IsActiveCityAsync(city, null, ct))
            .WithMessage("City must be selected from the supported Saudi cities catalog.");
        RuleFor(x => x.PostalCode).MaximumLength(20);
        RuleFor(x => x.District).MaximumLength(150);
    }
}

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator(ILocationCatalogService locations) =>
        RuleFor(x => x.Request).SetValidator(new CreateAddressDtoValidator(locations));
}

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator(ILocationCatalogService locations) =>
        RuleFor(x => x.Request).SetValidator(new CreateAddressDtoValidator(locations));
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator(ILocationCatalogService locations) =>
        RuleFor(x => x.Request).SetValidator(new UpdateProfileRequestValidator(locations));
}
