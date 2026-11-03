using FluentValidation;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Validators;

public class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturerRequest>
{
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).ValidName("Manufacturer name");
        RuleFor(x => x.HqCity).ValidName("HQ City");
        RuleFor(x => x.HqCountry).ValidName("HQ Country");
        RuleFor(x => x.FoundedYear).ValidFoundedYear();
        RuleFor(x => x.Latitude).ValidLatitude();
        RuleFor(x => x.Longitude).ValidLongitude();
    }
}

public class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).ValidName("Manufacturer name");
        RuleFor(x => x.HqCity).ValidName("HQ City");
        RuleFor(x => x.HqCountry).ValidName("HQ Country");
        RuleFor(x => x.FoundedYear).ValidFoundedYear();
        RuleFor(x => x.Latitude).ValidLatitude();
        RuleFor(x => x.Longitude).ValidLongitude();
    }
}
