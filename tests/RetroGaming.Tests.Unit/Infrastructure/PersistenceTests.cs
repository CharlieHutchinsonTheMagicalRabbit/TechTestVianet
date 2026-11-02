using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetroGaming.Domain.Entities;
using RetroGaming.Infrastructure.Persistence;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.Tests.Unit.Infrastructure;

[TestFixture]
public class PersistenceTests
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
    public void AppDbContext_ModelConfiguration_HasCorrectTableNamesAndKeys()
    {
        using var context = CreateInMemoryDbContext();
        var model = context.Model;

        var manufacturerEntity = model.FindEntityType(typeof(Manufacturer));
        manufacturerEntity.Should().NotBeNull();
        manufacturerEntity!.GetTableName().Should().Be("Manufacturers");
        manufacturerEntity.FindPrimaryKey()!.Properties.Single().Name.Should().Be(nameof(Manufacturer.Id));

        var consoleEntity = model.FindEntityType(typeof(Console));
        consoleEntity.Should().NotBeNull();
        consoleEntity!.GetTableName().Should().Be("Consoles");
        consoleEntity.FindPrimaryKey()!.Properties.Single().Name.Should().Be(nameof(Console.Id));
    }

    [Test]
    public void AppDbContext_ModelConfiguration_HasRestrictDeleteBehaviorForConsoles()
    {
        using var context = CreateInMemoryDbContext();
        var consoleEntity = context.Model.FindEntityType(typeof(Console));
        var foreignKey = consoleEntity!.GetForeignKeys().Single(fk => fk.PrincipalEntityType.ClrType == typeof(Manufacturer));

        foreignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Test]
    public void AppDbContext_ModelConfiguration_HasUniqueIndexOnManufacturerIdAndConsoleName()
    {
        using var context = CreateInMemoryDbContext();
        var consoleEntity = context.Model.FindEntityType(typeof(Console));
        var indexes = consoleEntity!.GetIndexes();

        var compositeIndex = indexes.FirstOrDefault(i =>
            i.Properties.Count == 2 &&
            i.Properties.Any(p => p.Name == nameof(Console.ManufacturerId)) &&
            i.Properties.Any(p => p.Name == nameof(Console.Name)));

        compositeIndex.Should().NotBeNull();
        compositeIndex!.IsUnique.Should().BeTrue();
    }

    [Test]
    public void AppDbContext_ModelConfiguration_HasIndexOnManufacturerName()
    {
        using var context = CreateInMemoryDbContext();
        var manufacturerEntity = context.Model.FindEntityType(typeof(Manufacturer));
        var indexes = manufacturerEntity!.GetIndexes();

        var nameIndex = indexes.FirstOrDefault(i =>
            i.Properties.Count == 1 &&
            i.Properties.Any(p => p.Name == nameof(Manufacturer.Name)));

        nameIndex.Should().NotBeNull();
    }

    [Test]
    public async Task AppDbContext_SeedData_ContainsAllFiveClassicManufacturers()
    {
        using var context = CreateInMemoryDbContext();

        var manufacturers = await context.Manufacturers.ToListAsync();

        manufacturers.Should().HaveCount(5);
        manufacturers.Select(m => m.Name).Should().Contain([
            "Nintendo",
            "Sega",
            "Sony Interactive Entertainment",
            "Atari",
            "Microsoft"
        ]);

        var nintendo = manufacturers.Single(m => m.Name == "Nintendo");
        nintendo.HqCity.Should().Be("Kyoto");
        nintendo.HqCountry.Should().Be("Japan");
        nintendo.FoundedYear.Should().Be(1889);
        nintendo.Latitude.Should().BeApproximately(34.9699, 0.0001);
        nintendo.Longitude.Should().BeApproximately(135.7563, 0.0001);

        var sega = manufacturers.Single(m => m.Name == "Sega");
        sega.HqCity.Should().Be("Tokyo");
        sega.HqCountry.Should().Be("Japan");
        sega.FoundedYear.Should().Be(1960);
        sega.Latitude.Should().BeApproximately(35.6264, 0.0001);
        sega.Longitude.Should().BeApproximately(139.7402, 0.0001);

        var sony = manufacturers.Single(m => m.Name == "Sony Interactive Entertainment");
        sony.HqCity.Should().Be("San Mateo");
        sony.HqCountry.Should().Be("USA");
        sony.FoundedYear.Should().Be(1993);
        sony.Latitude.Should().BeApproximately(37.5485, 0.0001);
        sony.Longitude.Should().BeApproximately(-122.3131, 0.0001);

        var atari = manufacturers.Single(m => m.Name == "Atari");
        atari.HqCity.Should().Be("Sunnyvale");
        atari.HqCountry.Should().Be("USA");
        atari.FoundedYear.Should().Be(1972);
        atari.Latitude.Should().BeApproximately(37.3688, 0.0001);
        atari.Longitude.Should().BeApproximately(-122.0363, 0.0001);

        var ms = manufacturers.Single(m => m.Name == "Microsoft");
        ms.HqCity.Should().Be("Redmond");
        ms.HqCountry.Should().Be("USA");
        ms.FoundedYear.Should().Be(1975);
        ms.Latitude.Should().BeApproximately(47.6423, 0.0001);
        ms.Longitude.Should().BeApproximately(-122.1391, 0.0001);
    }

    [Test]
    public async Task AppDbContext_SeedData_ContainsAllClassicConsolesWithCorrectManufacturers()
    {
        using var context = CreateInMemoryDbContext();

        var consoles = await context.Consoles.Include(c => c.Manufacturer).ToListAsync();

        consoles.Should().HaveCount(10);

        var nintendoConsoles = consoles.Where(c => c.Manufacturer?.Name == "Nintendo").ToList();
        nintendoConsoles.Should().HaveCount(3);
        nintendoConsoles.Select(c => c.Name).Should().BeEquivalentTo(["NES / Famicom", "Super Nintendo", "N64"]);

        var segaConsoles = consoles.Where(c => c.Manufacturer?.Name == "Sega").ToList();
        segaConsoles.Should().HaveCount(3);
        segaConsoles.Select(c => c.Name).Should().BeEquivalentTo(["Master System", "Mega Drive / Genesis", "Dreamcast"]);

        var sonyConsoles = consoles.Where(c => c.Manufacturer?.Name == "Sony Interactive Entertainment").ToList();
        sonyConsoles.Should().HaveCount(2);
        sonyConsoles.Select(c => c.Name).Should().BeEquivalentTo(["PlayStation", "PlayStation 2"]);

        var atariConsoles = consoles.Where(c => c.Manufacturer?.Name == "Atari").ToList();
        atariConsoles.Should().HaveCount(1);
        atariConsoles.Single().Name.Should().Be("Atari 2600");

        var msConsoles = consoles.Where(c => c.Manufacturer?.Name == "Microsoft").ToList();
        msConsoles.Should().HaveCount(1);
        msConsoles.Single().Name.Should().Be("Xbox");
    }
}
