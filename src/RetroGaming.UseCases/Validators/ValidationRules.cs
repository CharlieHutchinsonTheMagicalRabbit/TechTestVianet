using FluentValidation;

namespace RetroGaming.UseCases.Validators;

/// <summary>
/// Shared validation constants and reusable FluentValidation rules so that the
/// Create/Update validators (and consoles vs. manufacturers) never drift apart.
/// </summary>
public static class ValidationRules
{
    /// <summary>Earliest release year for a (retro) console.</summary>
    public const int MinReleaseYear = 1970;

    /// <summary>
    /// Earliest founded year for a manufacturer. Kept lenient because several real
    /// manufacturers (e.g. Nintendo, 1889) predate the console era.
    /// </summary>
    public const int MinFoundedYear = 1800;

    /// <summary>Allow release years a little into the future for upcoming hardware.</summary>
    public static int MaxReleaseYear => DateTime.UtcNow.Year + 5;

    /// <summary>Founded years cannot be in the future.</summary>
    public static int MaxFoundedYear => DateTime.UtcNow.Year;

    public static IRuleBuilderOptions<T, string> ValidName<T>(
        this IRuleBuilder<T, string> rule, string label) =>
        rule
            .NotEmpty().WithMessage($"{label} is required.")
            .MaximumLength(100).WithMessage($"{label} must not exceed 100 characters.");

    public static IRuleBuilderOptions<T, int> ValidReleaseYear<T>(
        this IRuleBuilder<T, int> rule) =>
        rule
            .InclusiveBetween(MinReleaseYear, MaxReleaseYear)
            .WithMessage($"Release year must be between {MinReleaseYear} and {MaxReleaseYear}.");

    public static IRuleBuilderOptions<T, int> ValidFoundedYear<T>(
        this IRuleBuilder<T, int> rule) =>
        rule
            .InclusiveBetween(MinFoundedYear, MaxFoundedYear)
            .WithMessage($"Founded year must be between {MinFoundedYear} and {MaxFoundedYear}.");

    public static IRuleBuilderOptions<T, int> ValidGeneration<T>(
        this IRuleBuilder<T, int> rule) =>
        rule
            .GreaterThanOrEqualTo(1)
            .WithMessage("Generation must be greater than or equal to 1.");

    public static IRuleBuilderOptions<T, decimal> ValidUnitsSold<T>(
        this IRuleBuilder<T, decimal> rule) =>
        rule
            .GreaterThanOrEqualTo(0)
            .WithMessage("Units sold must be non-negative.");

    public static IRuleBuilderOptions<T, double> ValidLatitude<T>(
        this IRuleBuilder<T, double> rule) =>
        rule
            .InclusiveBetween(-90.0, 90.0)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

    public static IRuleBuilderOptions<T, double> ValidLongitude<T>(
        this IRuleBuilder<T, double> rule) =>
        rule
            .InclusiveBetween(-180.0, 180.0)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
}
