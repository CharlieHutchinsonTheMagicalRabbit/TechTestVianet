using FluentAssertions;
using RetroGaming.UseCases.DTOs;
using RetroGaming.UseCases.Validators;

namespace RetroGaming.Tests.Unit.Application;

[TestFixture]
public class ValidatorTests
{
    private CreateManufacturerRequestValidator _createManufacturerValidator = null!;
    private UpdateManufacturerRequestValidator _updateManufacturerValidator = null!;
    private CreateConsoleRequestValidator _createConsoleValidator = null!;
    private UpdateConsoleRequestValidator _updateConsoleValidator = null!;

    [SetUp]
    public void Setup()
    {
        _createManufacturerValidator = new CreateManufacturerRequestValidator();
        _updateManufacturerValidator = new UpdateManufacturerRequestValidator();
        _createConsoleValidator = new CreateConsoleRequestValidator();
        _updateConsoleValidator = new UpdateConsoleRequestValidator();
    }

    [Test]
    public async Task CreateManufacturerRequestValidator_WithValidData_ShouldBeValid()
    {
        // Arrange
        var request = new CreateManufacturerRequest("Nintendo", "Kyoto", "Japan", 1889, 34.9699, 135.7563);

        // Act
        var result = await _createManufacturerValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestCase("", "City", "Country", 1990, 0, 0)]
    [TestCase("Name", "", "Country", 1990, 0, 0)]
    [TestCase("Name", "City", "", 1990, 0, 0)]
    [TestCase("Name", "City", "Country", 1799, 0, 0)]
    [TestCase("Name", "City", "Country", 2999, 0, 0)]
    [TestCase("Name", "City", "Country", 1990, -90.1, 0)]
    [TestCase("Name", "City", "Country", 1990, 90.1, 0)]
    [TestCase("Name", "City", "Country", 1990, 0, -180.1)]
    [TestCase("Name", "City", "Country", 1990, 0, 180.1)]
    public async Task CreateManufacturerRequestValidator_WithInvalidData_ShouldBeInvalid(
        string name, string city, string country, int year, double lat, double lon)
    {
        // Arrange
        var request = new CreateManufacturerRequest(name, city, country, year, lat, lon);

        // Act
        var result = await _createManufacturerValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task CreateConsoleRequestValidator_WithValidData_ShouldBeValid()
    {
        // Arrange
        var request = new CreateConsoleRequest(Guid.NewGuid(), "PlayStation 2", 2000, 6, 155.0m, true);

        // Act
        var result = await _createConsoleValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task CreateConsoleRequestValidator_WithEmptyManufacturerId_ShouldBeInvalid()
    {
        // Arrange
        var request = new CreateConsoleRequest(Guid.Empty, "Console", 2000, 6, 10.0m, false);

        // Act
        var result = await _createConsoleValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateConsoleRequest.ManufacturerId));
    }

    [TestCase("", 2000, 1, 10)]
    [TestCase("Name", 1969, 1, 10)]
    [TestCase("Name", 2000, 0, 10)]
    [TestCase("Name", 2000, 1, -1)]
    public async Task CreateConsoleRequestValidator_WithInvalidFields_ShouldBeInvalid(
        string name, int releaseYear, int generation, decimal unitsSold)
    {
        // Arrange
        var request = new CreateConsoleRequest(Guid.NewGuid(), name, releaseYear, generation, unitsSold, false);

        // Act
        var result = await _createConsoleValidator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
