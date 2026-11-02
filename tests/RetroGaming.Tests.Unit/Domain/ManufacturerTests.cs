using FluentAssertions;
using RetroGaming.Domain.Entities;
using RetroGaming.Domain.Exceptions;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.Tests.Unit.Domain;

[TestFixture]
public class ManufacturerTests
{
    [Test]
    public void CreateManufacturer_WithValidData_ShouldInstantiateSuccessfully()
    {
        // Arrange & Act
        var manufacturer = new Manufacturer("Nintendo", "Kyoto", "Japan", 1889, 34.9699, 135.7563);

        // Assert
        manufacturer.Id.Should().NotBeEmpty();
        manufacturer.Name.Should().Be("Nintendo");
        manufacturer.HqCity.Should().Be("Kyoto");
        manufacturer.HqCountry.Should().Be("Japan");
        manufacturer.FoundedYear.Should().Be(1889);
        manufacturer.Latitude.Should().Be(34.9699);
        manufacturer.Longitude.Should().Be(135.7563);
        manufacturer.Consoles.Should().BeEmpty();
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void CreateManufacturer_WithInvalidName_ShouldThrowDomainValidationException(string? invalidName)
    {
        // Act
        var act = () => new Manufacturer(invalidName!, "Kyoto", "Japan", 1889, 34.9699, 135.7563);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*name is required*");
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void CreateManufacturer_WithInvalidHqCity_ShouldThrowDomainValidationException(string? invalidCity)
    {
        // Act
        var act = () => new Manufacturer("Nintendo", invalidCity!, "Japan", 1889, 34.9699, 135.7563);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*City is required*");
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void CreateManufacturer_WithInvalidHqCountry_ShouldThrowDomainValidationException(string? invalidCountry)
    {
        // Act
        var act = () => new Manufacturer("Nintendo", "Kyoto", invalidCountry!, 1889, 34.9699, 135.7563);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Country is required*");
    }

    [TestCase(1799)]
    [TestCase(2999)]
    public void CreateManufacturer_WithUnreasonableFoundedYear_ShouldThrowDomainValidationException(int invalidYear)
    {
        // Act
        var act = () => new Manufacturer("Nintendo", "Kyoto", "Japan", invalidYear, 34.9699, 135.7563);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Founded year must be between 1800 and*");
    }

    [TestCase(-90.01)]
    [TestCase(90.01)]
    public void CreateManufacturer_WithInvalidLatitude_ShouldThrowDomainValidationException(double invalidLatitude)
    {
        // Act
        var act = () => new Manufacturer("Nintendo", "Kyoto", "Japan", 1889, invalidLatitude, 135.7563);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Latitude must be between -90 and 90 degrees*");
    }

    [TestCase(-180.01)]
    [TestCase(180.01)]
    public void CreateManufacturer_WithInvalidLongitude_ShouldThrowDomainValidationException(double invalidLongitude)
    {
        // Act
        var act = () => new Manufacturer("Nintendo", "Kyoto", "Japan", 1889, 34.9699, invalidLongitude);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Longitude must be between -180 and 180 degrees*");
    }

    [Test]
    public void UpdateDetails_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var manufacturer = new Manufacturer("Sega", "Tokyo", "Japan", 1960, 35.6264, 139.7402);

        // Act
        manufacturer.UpdateDetails("Sega Games", "Tokyo", "Japan", 1960, 35.6265, 139.7405);

        // Assert
        manufacturer.Name.Should().Be("Sega Games");
        manufacturer.Latitude.Should().Be(35.6265);
        manufacturer.Longitude.Should().Be(139.7405);
    }

    [Test]
    public void AddConsole_WithValidConsole_ShouldAddToCollectionAndAssignManufacturer()
    {
        // Arrange
        var manufacturer = new Manufacturer("Nintendo", "Kyoto", "Japan", 1889, 34.9699, 135.7563);
        var console = new Console(manufacturer.Id, "NES", 1983, 3, 61.91m, true);

        // Act
        manufacturer.AddConsole(console);

        // Assert
        manufacturer.Consoles.Should().ContainSingle().Which.Should().Be(console);
        console.Manufacturer.Should().Be(manufacturer);
        console.ManufacturerId.Should().Be(manufacturer.Id);
    }

    [Test]
    public void AddConsole_WithReleaseYearEarlierThanFoundedYear_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        var manufacturer = new Manufacturer("Sony", "Tokyo", "Japan", 1993, 37.5485, -122.3131);
        var console = new Console(manufacturer.Id, "Prototype Console", 1990, 4, 0m, true);

        // Act
        var act = () => manufacturer.AddConsole(console);

        // Assert
        act.Should().Throw<InvalidDomainOperationException>()
            .WithMessage("*cannot predate manufacturer founded year*");
    }

    [Test]
    public void AddConsole_WithDuplicateConsoleName_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        var manufacturer = new Manufacturer("Nintendo", "Kyoto", "Japan", 1889, 34.9699, 135.7563);
        var console1 = new Console(manufacturer.Id, "Super Nintendo", 1990, 4, 49.10m, true);
        var console2 = new Console(manufacturer.Id, "super nintendo", 1992, 4, 10.0m, true);

        manufacturer.AddConsole(console1);

        // Act
        var act = () => manufacturer.AddConsole(console2);

        // Assert
        act.Should().Throw<InvalidDomainOperationException>()
            .WithMessage("*already exists for manufacturer*");
    }

    [Test]
    public void UpdateDetails_WhenFoundedYearSetAfterExistingConsoleReleaseYear_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        var manufacturer = new Manufacturer("Nintendo", "Kyoto", "Japan", 1889, 34.9699, 135.7563);
        var console = new Console(manufacturer.Id, "NES", 1983, 3, 61.91m, true);
        manufacturer.AddConsole(console);

        // Act
        var act = () => manufacturer.UpdateDetails("Nintendo", "Kyoto", "Japan", 1990, 34.9699, 135.7563);

        // Assert
        act.Should().Throw<InvalidDomainOperationException>()
            .WithMessage("*Cannot change founded year to 1990 because console 'NES' was released in 1983*");
    }

    [Test]
    public void RemoveConsole_ShouldRemoveConsoleFromCollection()
    {
        // Arrange
        var manufacturer = new Manufacturer("Nintendo", "Kyoto", "Japan", 1889, 34.9699, 135.7563);
        var console = new Console(manufacturer.Id, "NES", 1983, 3, 61.91m, true);
        manufacturer.AddConsole(console);

        // Act
        manufacturer.RemoveConsole(console);

        // Assert
        manufacturer.Consoles.Should().BeEmpty();
    }
}
