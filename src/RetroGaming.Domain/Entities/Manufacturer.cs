using RetroGaming.Domain.Exceptions;

namespace RetroGaming.Domain.Entities;

public class Manufacturer
{
    private readonly List<Console> _consoles = [];

    // Parameterless constructor for EF Core
    private Manufacturer()
    {
    }

    public Manufacturer(
        Guid id,
        string name,
        string hqCity,
        string hqCountry,
        int foundedYear,
        double latitude,
        double longitude)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        ValidateAndSetDetails(name, hqCity, hqCountry, foundedYear, latitude, longitude);
    }

    public Manufacturer(
        string name,
        string hqCity,
        string hqCountry,
        int foundedYear,
        double latitude,
        double longitude)
        : this(Guid.NewGuid(), name, hqCity, hqCountry, foundedYear, latitude, longitude)
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string HqCity { get; private set; } = string.Empty;
    public string HqCountry { get; private set; } = string.Empty;
    public int FoundedYear { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public IReadOnlyCollection<Console> Consoles => _consoles.AsReadOnly();

    public void UpdateDetails(
        string name,
        string hqCity,
        string hqCountry,
        int foundedYear,
        double latitude,
        double longitude)
    {
        ValidateAndSetDetails(name, hqCity, hqCountry, foundedYear, latitude, longitude);
    }

    public void AddConsole(Console console)
    {
        ArgumentNullException.ThrowIfNull(console);

        if (console.ReleaseYear < FoundedYear)
        {
            throw new InvalidDomainOperationException(
                $"Console release year ({console.ReleaseYear}) cannot predate manufacturer founded year ({FoundedYear}).");
        }

        if (_consoles.Any(c => string.Equals(c.Name, console.Name, StringComparison.OrdinalIgnoreCase) && c.Id != console.Id))
        {
            throw new InvalidDomainOperationException(
                $"A console named '{console.Name}' already exists for manufacturer '{Name}'.");
        }

        console.AssignToManufacturer(this);

        if (!_consoles.Contains(console))
        {
            _consoles.Add(console);
        }
    }

    public void RemoveConsole(Console console)
    {
        ArgumentNullException.ThrowIfNull(console);
        _consoles.Remove(console);
    }

    private void ValidateAndSetDetails(
        string name,
        string hqCity,
        string hqCountry,
        int foundedYear,
        double latitude,
        double longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Manufacturer name is required.");
        }

        if (string.IsNullOrWhiteSpace(hqCity))
        {
            throw new DomainValidationException("HQ City is required.");
        }

        if (string.IsNullOrWhiteSpace(hqCountry))
        {
            throw new DomainValidationException("HQ Country is required.");
        }

        var currentYear = DateTime.UtcNow.Year;
        if (foundedYear < 1800 || foundedYear > currentYear)
        {
            throw new DomainValidationException(
                $"Founded year must be between 1800 and {currentYear}.");
        }

        if (latitude < -90.0 || latitude > 90.0)
        {
            throw new DomainValidationException("Latitude must be between -90 and 90 degrees.");
        }

        if (longitude < -180.0 || longitude > 180.0)
        {
            throw new DomainValidationException("Longitude must be between -180 and 180 degrees.");
        }

        // If changing founded year on existing manufacturer, ensure existing consoles are still valid
        if (_consoles.Any(c => c.ReleaseYear < foundedYear))
        {
            var conflictingConsole = _consoles.First(c => c.ReleaseYear < foundedYear);
            throw new InvalidDomainOperationException(
                $"Cannot change founded year to {foundedYear} because console '{conflictingConsole.Name}' was released in {conflictingConsole.ReleaseYear}.");
        }

        Name = name.Trim();
        HqCity = hqCity.Trim();
        HqCountry = hqCountry.Trim();
        FoundedYear = foundedYear;
        Latitude = latitude;
        Longitude = longitude;
    }
}
