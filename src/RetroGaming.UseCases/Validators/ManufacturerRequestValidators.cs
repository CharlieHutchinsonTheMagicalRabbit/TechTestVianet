using FluentValidation;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Validators;

public class CreateManufacturerRequestValidator : AbstractValidator<CreateManufacturerRequest>
{
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Manufacturer name is required.")
            .MaximumLength(100).WithMessage("Manufacturer name must not exceed 100 characters.");

        RuleFor(x => x.HqCity)
            .NotEmpty().WithMessage("HQ City is required.")
            .MaximumLength(100).WithMessage("HQ City must not exceed 100 characters.");

        RuleFor(x => x.HqCountry)
            .NotEmpty().WithMessage("HQ Country is required.")
            .MaximumLength(100).WithMessage("HQ Country must not exceed 100 characters.");

        RuleFor(x => x.FoundedYear)
            .InclusiveBetween(1800, DateTime.UtcNow.Year)
            .WithMessage($"Founded year must be between 1800 and {DateTime.UtcNow.Year}.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90.0, 90.0)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180.0, 180.0)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
    }
}

public class UpdateManufacturerRequestValidator : AbstractValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Manufacturer name is required.")
            .MaximumLength(100).WithMessage("Manufacturer name must not exceed 100 characters.");

        RuleFor(x => x.HqCity)
            .NotEmpty().WithMessage("HQ City is required.")
            .MaximumLength(100).WithMessage("HQ City must not exceed 100 characters.");

        RuleFor(x => x.HqCountry)
            .NotEmpty().WithMessage("HQ Country is required.")
            .MaximumLength(100).WithMessage("HQ Country must not exceed 100 characters.");

        RuleFor(x => x.FoundedYear)
            .InclusiveBetween(1800, DateTime.UtcNow.Year)
            .WithMessage($"Founded year must be between 1800 and {DateTime.UtcNow.Year}.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90.0, 90.0)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180.0, 180.0)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
    }
}
