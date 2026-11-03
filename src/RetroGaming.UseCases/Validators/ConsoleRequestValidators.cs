using FluentValidation;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Validators;

public class CreateConsoleRequestValidator : AbstractValidator<CreateConsoleRequest>
{
    public CreateConsoleRequestValidator()
    {
        RuleFor(x => x.ManufacturerId)
            .NotEmpty().WithMessage("Manufacturer ID is required.");

        RuleFor(x => x.Name).ValidName("Console name");
        RuleFor(x => x.ReleaseYear).ValidReleaseYear();
        RuleFor(x => x.Generation).ValidGeneration();
        RuleFor(x => x.UnitsSoldInMillions).ValidUnitsSold();
    }
}

public class UpdateConsoleRequestValidator : AbstractValidator<UpdateConsoleRequest>
{
    public UpdateConsoleRequestValidator()
    {
        RuleFor(x => x.Name).ValidName("Console name");
        RuleFor(x => x.ReleaseYear).ValidReleaseYear();
        RuleFor(x => x.Generation).ValidGeneration();
        RuleFor(x => x.UnitsSoldInMillions).ValidUnitsSold();
    }
}
