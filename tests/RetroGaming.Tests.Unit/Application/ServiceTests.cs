using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetroGaming.UseCases.DTOs;
using RetroGaming.UseCases.Services;
using RetroGaming.UseCases.Validators;
using RetroGaming.Domain.Entities;
using RetroGaming.Domain.Exceptions;
using RetroGaming.Infrastructure.Persistence;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.Tests.Unit.Application;

[TestFixture]
public class ServiceTests
{
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Test]
    public async Task ManufacturerService_GetAllAsync_ShouldReturnAllManufacturersWithConsoleCounts()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new ManufacturerService(
            context,
            new CreateManufacturerRequestValidator(),
            new UpdateManufacturerRequestValidator());

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().HaveCount(5); // Seeded manufacturers
        var nintendo = result.Single(m => m.Name == "Nintendo");
        nintendo.ConsoleCount.Should().Be(3);
    }

    [Test]
    public async Task ManufacturerService_GetByIdAsync_WhenExists_ShouldReturnDetailWithConsoles()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var seeded = await context.Manufacturers.FirstAsync(m => m.Name == "Sony Interactive Entertainment");
        var service = new ManufacturerService(
            context,
            new CreateManufacturerRequestValidator(),
            new UpdateManufacturerRequestValidator());

        // Act
        var result = await service.GetByIdAsync(seeded.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Sony Interactive Entertainment");
        result.Consoles.Should().HaveCount(2);
    }

    [Test]
    public async Task ManufacturerService_GetByIdAsync_WhenDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new ManufacturerService(
            context,
            new CreateManufacturerRequestValidator(),
            new UpdateManufacturerRequestValidator());

        // Act & Assert
        var act = async () => await service.GetByIdAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Test]
    public async Task ManufacturerService_CreateAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new ManufacturerService(
            context,
            new CreateManufacturerRequestValidator(),
            new UpdateManufacturerRequestValidator());

        var request = new CreateManufacturerRequest("SNK", "Osaka", "Japan", 1978, 34.6937, 135.5023);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("SNK");
        var inDb = await context.Manufacturers.FirstOrDefaultAsync(m => m.Id == result.Id);
        inDb.Should().NotBeNull();
    }

    [Test]
    public async Task ManufacturerService_CreateAsync_WithDuplicateName_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new ManufacturerService(
            context,
            new CreateManufacturerRequestValidator(),
            new UpdateManufacturerRequestValidator());

        var request = new CreateManufacturerRequest("Nintendo", "Kyoto", "Japan", 1889, 34.9699, 135.7563);

        // Act & Assert
        var act = async () => await service.CreateAsync(request);
        await act.Should().ThrowAsync<InvalidDomainOperationException>()
            .WithMessage("*already exists*");
    }

    [Test]
    public async Task ManufacturerService_DeleteAsync_WhenHasConsoles_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var nintendo = await context.Manufacturers.FirstAsync(m => m.Name == "Nintendo");
        var service = new ManufacturerService(
            context,
            new CreateManufacturerRequestValidator(),
            new UpdateManufacturerRequestValidator());

        // Act & Assert
        var act = async () => await service.DeleteAsync(nintendo.Id);
        await act.Should().ThrowAsync<InvalidDomainOperationException>()
            .WithMessage("*Cannot delete manufacturer 'Nintendo' because it has 3 associated console(s)*");
    }

    [Test]
    public async Task ManufacturerService_DeleteAsync_WhenNoConsoles_ShouldDeleteSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var emptyManufacturer = new Manufacturer("NEC", "Tokyo", "Japan", 1899, 35.6895, 139.6917);
        context.Manufacturers.Add(emptyManufacturer);
        await context.SaveChangesAsync();

        var service = new ManufacturerService(
            context,
            new CreateManufacturerRequestValidator(),
            new UpdateManufacturerRequestValidator());

        // Act
        await service.DeleteAsync(emptyManufacturer.Id);

        // Assert
        var inDb = await context.Manufacturers.FindAsync(emptyManufacturer.Id);
        inDb.Should().BeNull();
    }

    [Test]
    public async Task ConsoleService_GetAllAsync_WithSearchAndSort_ShouldFilterAndSortCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new ConsoleService(
            context,
            new CreateConsoleRequestValidator(),
            new UpdateConsoleRequestValidator());

        var query = new ConsoleSearchQuery(SearchTerm: "PlayStation", SortBy: "releaseYear", SortDescending: true);

        // Act
        var result = await service.GetAllAsync(query);

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("PlayStation 2");
        result.Last().Name.Should().Be("PlayStation");
    }

    [Test]
    public async Task ConsoleService_CreateAsync_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var nintendo = await context.Manufacturers.FirstAsync(m => m.Name == "Nintendo");
        var service = new ConsoleService(
            context,
            new CreateConsoleRequestValidator(),
            new UpdateConsoleRequestValidator());

        var request = new CreateConsoleRequest(nintendo.Id, "Game Boy", 1989, 4, 118.69m, true);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Game Boy");
        result.ManufacturerName.Should().Be("Nintendo");
    }

    [Test]
    public async Task ConsoleService_CreateAsync_WithDuplicateConsoleNameForSameManufacturer_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var nintendo = await context.Manufacturers.FirstAsync(m => m.Name == "Nintendo");
        var service = new ConsoleService(
            context,
            new CreateConsoleRequestValidator(),
            new UpdateConsoleRequestValidator());

        var request = new CreateConsoleRequest(nintendo.Id, "Super Nintendo", 1990, 4, 49.10m, true);

        // Act & Assert
        var act = async () => await service.CreateAsync(request);
        await act.Should().ThrowAsync<InvalidDomainOperationException>()
            .WithMessage("*already exists*");
    }

    [Test]
    public async Task ConsoleService_CreateAsync_WhenReleaseYearPredatesFoundedYear_ShouldThrowInvalidDomainOperationException()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var sony = await context.Manufacturers.FirstAsync(m => m.Name == "Sony Interactive Entertainment"); // Founded 1993
        var service = new ConsoleService(
            context,
            new CreateConsoleRequestValidator(),
            new UpdateConsoleRequestValidator());

        var request = new CreateConsoleRequest(sony.Id, "Prototype Console", 1985, 3, 0m, true);

        // Act & Assert
        var act = async () => await service.CreateAsync(request);
        await act.Should().ThrowAsync<InvalidDomainOperationException>()
            .WithMessage("*cannot predate manufacturer founded year*");
    }

    [Test]
    public async Task MapService_GetLocationsAsync_ShouldReturnAllLocationsWithFormattedStrings()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new MapService(context);

        // Act
        var result = await service.GetLocationsAsync();

        // Assert
        result.Should().HaveCount(5);
        var nintendo = result.Single(m => m.Name == "Nintendo");
        nintendo.LocationString.Should().Be("Kyoto, Japan");
        nintendo.ConsoleCount.Should().Be(3);
    }
}
