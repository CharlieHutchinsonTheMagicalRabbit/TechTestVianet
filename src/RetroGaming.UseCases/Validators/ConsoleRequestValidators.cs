using FluentValidation;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Validators;

public class CreateConsoleRequestValidator : AbstractValidator<CreateConsoleRequest>
{
    public CreateConsoleRequestValidator()
    {
        RuleFor(x => x.ManufacturerId)
            .NotEmpty().WithMessage("Manufacturer ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Console name is required.")
            .MaximumLength(100).WithMessage("Console name must not exceed 100 characters.");

        RuleFor(x => x.ReleaseYear)
            .InclusiveBetween(1970, DateTime.UtcNow.Year + 5)
            .WithMessage($"Release year must be between 1970 and {DateTime.UtcNow.Year + 5}.");

        RuleFor(x => x.Generation)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Generation must be greater than or equal to 1.");

        RuleFor(x => x.UnitsSoldInMillions)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Units sold must be non-negative.");
    }
}

public class UpdateConsoleRequestValidator : AbstractValidator<UpdateConsoleRequest>
{
    public UpdateConsoleRequestValidator()
    {
        RuleFor(x => x.ManufacturerId)
            .NotEmpty().WithMessage("Manufacturer ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Console name is required.")
            .MaximumLength(100).WithMessage("Console name must not exceed 100 characters.");

        RuleFor(x => x.ReleaseYear)
            .InclusiveBetween(1970, DateTime.UtcNow.Year + 5)
            .WithMessage($"Release year must be between 1970 and {DateTime.UtcNow.Year + 5}.");

        RuleFor(x => x.Generation)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Generation must be greater than or equal to 1.");

        RuleFor(x => x.UnitsSoldInMillions)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Units sold must be non-negative.");
    }
}
