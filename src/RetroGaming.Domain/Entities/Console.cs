using RetroGaming.Domain.Exceptions;

namespace RetroGaming.Domain.Entities;

public class Console
{
    // Parameterless constructor for EF Core
    private Console()
    {
    }

    public Console(
        Guid id,
        Guid manufacturerId,
        string name,
        int releaseYear,
        int generation,
        decimal unitsSoldInMillions,
        bool isDiscontinued)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        ManufacturerId = manufacturerId;
        ValidateAndSetDetails(name, releaseYear, generation, unitsSoldInMillions, isDiscontinued);
    }

    public Console(
        Guid manufacturerId,
        string name,
        int releaseYear,
        int generation,
        decimal unitsSoldInMillions,
        bool isDiscontinued)
        : this(Guid.NewGuid(), manufacturerId, name, releaseYear, generation, unitsSoldInMillions, isDiscontinued)
    {
    }

    public Guid Id { get; private set; }
    public Guid ManufacturerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int ReleaseYear { get; private set; }
    public int Generation { get; private set; }
    public decimal UnitsSoldInMillions { get; private set; }
    public bool IsDiscontinued { get; private set; }

    public Manufacturer? Manufacturer { get; private set; }

    public void UpdateDetails(
        string name,
        int releaseYear,
        int generation,
        decimal unitsSoldInMillions,
        bool isDiscontinued)
    {
        ValidateAndSetDetails(name, releaseYear, generation, unitsSoldInMillions, isDiscontinued);
    }

    public void AssignToManufacturer(Manufacturer manufacturer)
    {
        ArgumentNullException.ThrowIfNull(manufacturer);

        if (ReleaseYear < manufacturer.FoundedYear)
        {
            throw new InvalidDomainOperationException(
                $"Console release year ({ReleaseYear}) cannot predate manufacturer founded year ({manufacturer.FoundedYear}).");
        }

        Manufacturer = manufacturer;
        ManufacturerId = manufacturer.Id;
    }

    private void ValidateAndSetDetails(
        string name,
        int releaseYear,
        int generation,
        decimal unitsSoldInMillions,
        bool isDiscontinued)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Console name is required.");
        }

        if (unitsSoldInMillions < 0)
        {
            throw new DomainValidationException("Units sold must be non-negative.");
        }

        if (generation < 1)
        {
            throw new DomainValidationException("Generation must be greater than or equal to 1.");
        }

        if (Manufacturer != null && releaseYear < Manufacturer.FoundedYear)
        {
            throw new InvalidDomainOperationException(
                $"Console release year ({releaseYear}) cannot predate manufacturer founded year ({Manufacturer.FoundedYear}).");
        }

        Name = name.Trim();
        ReleaseYear = releaseYear;
        Generation = generation;
        UnitsSoldInMillions = unitsSoldInMillions;
        IsDiscontinued = isDiscontinued;
    }
}
