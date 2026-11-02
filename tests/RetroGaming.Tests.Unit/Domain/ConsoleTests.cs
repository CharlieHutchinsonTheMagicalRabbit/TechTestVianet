using FluentAssertions;
using RetroGaming.Domain.Entities;
using RetroGaming.Domain.Exceptions;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.Tests.Unit.Domain;

[TestFixture]
public class ConsoleTests
{
    [Test]
    public void CreateConsole_WithValidData_ShouldInstantiateSuccessfully()
    {
        // Arrange
        var manufacturerId = Guid.NewGuid();

        // Act
        var console = new Console(manufacturerId, "Sega Genesis", 1988, 4, 30.75m, true);

        // Assert
        console.Id.Should().NotBeEmpty();
        console.ManufacturerId.Should().Be(manufacturerId);
        console.Name.Should().Be("Sega Genesis");
        console.ReleaseYear.Should().Be(1988);
        console.Generation.Should().Be(4);
        console.UnitsSoldInMillions.Should().Be(30.75m);
        console.IsDiscontinued.Should().BeTrue();
        console.Manufacturer.Should().BeNull();
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void CreateConsole_WithInvalidName_ShouldThrowDomainValidationException(string? invalidName)
    {
        // Arrange
        var manufacturerId = Guid.NewGuid();

        // Act
        var act = () => new Console(manufacturerId, invalidName!, 1988, 4, 30.75m, true);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*name is required*");
    }

    [Test]
    public void CreateConsole_WithNegativeUnitsSold_ShouldThrowDomainValidationException()
    {
        // Arrange
        var manufacturerId = Guid.NewGuid();

        // Act
        var act = () => new Console(manufacturerId, "Dreamcast", 1998, 6, -1.5m, true);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Units sold must be non-negative*");
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void CreateConsole_WithInvalidGeneration_ShouldThrowDomainValidationException(int invalidGeneration)
    {
        // Arrange
        var manufacturerId = Guid.NewGuid();

        // Act
        var act = () => new Console(manufacturerId, "Dreamcast", 1998, invalidGeneration, 9.13m, true);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Generation must be greater than or equal to 1*");
    }

    [Test]
    public void UpdateDetails_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var console = new Console(Guid.NewGuid(), "PlayStation 2", 2000, 6, 155.0m, false);

        // Act
        console.UpdateDetails("PlayStation 2 Slim", 2000, 6, 158.7m, true);

        // Assert
        console.Name.Should().Be("PlayStation 2 Slim");
        console.UnitsSoldInMillions.Should().Be(158.7m);
        console.IsDiscontinued.Should().BeTrue();
    }

    [Test]
    public void UpdateDetails_WhenAssignedToManufacturerAndReleaseYearPredatesFoundedYear_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        var manufacturer = new Manufacturer("Sony", "Tokyo", "Japan", 1993, 37.5485, -122.3131);
        var console = new Console(manufacturer.Id, "PlayStation", 1994, 5, 102.49m, true);
        manufacturer.AddConsole(console);

        // Act
        var act = () => console.UpdateDetails("PlayStation Prototype", 1990, 5, 102.49m, true);

        // Assert
        act.Should().Throw<InvalidDomainOperationException>()
            .WithMessage("*cannot predate manufacturer founded year*");
    }
}
