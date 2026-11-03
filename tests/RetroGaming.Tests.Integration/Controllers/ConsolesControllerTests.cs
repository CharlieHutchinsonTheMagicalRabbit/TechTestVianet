using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.Tests.Integration.Controllers;

[TestFixture]
public class ConsolesControllerTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAll_ReturnsConsoles()
    {
        var response = await _client.GetAsync("/api/consoles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var consoles = await response.Content.ReadFromJsonAsync<IReadOnlyList<ConsoleDto>>();
        consoles.Should().NotBeNull();
        consoles!.Count.Should().BeGreaterThanOrEqualTo(10);
    }

    [Test]
    public async Task GetAll_WithSearchAndSort_ReturnsFilteredAndSortedConsoles()
    {
        var response = await _client.GetAsync("/api/consoles?searchTerm=playstation&sortBy=releaseYear&sortDescending=true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var consoles = await response.Content.ReadFromJsonAsync<IReadOnlyList<ConsoleDto>>();
        consoles.Should().NotBeNull();
        consoles!.Should().NotBeEmpty();
        consoles!.All(c => c.Name.Contains("PlayStation", StringComparison.OrdinalIgnoreCase) ||
                           c.ManufacturerName.Contains("PlayStation", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Test]
    public async Task GetById_WhenExists_ReturnsConsole()
    {
        var getAllResponse = await _client.GetAsync("/api/consoles");
        var consoles = await getAllResponse.Content.ReadFromJsonAsync<IReadOnlyList<ConsoleDto>>();
        var first = consoles!.First();

        var response = await _client.GetAsync($"/api/consoles/{first.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var console = await response.Content.ReadFromJsonAsync<ConsoleDto>();
        console.Should().NotBeNull();
        console!.Id.Should().Be(first.Id);
        console.Name.Should().Be(first.Name);
    }

    [Test]
    public async Task GetById_WhenNotFound_ReturnsProblemDetails404()
    {
        var response = await _client.GetAsync($"/api/consoles/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
    }

    [Test]
    public async Task Create_WithValidData_Returns201Created()
    {
        var mResponse = await _client.GetAsync("/api/manufacturers");
        var manufacturers = await mResponse.Content.ReadFromJsonAsync<IReadOnlyList<ManufacturerDto>>();
        var nintendo = manufacturers!.First(m => m.Name == "Nintendo");

        var request = new CreateConsoleRequest(
            nintendo.Id,
            "GameCube",
            2001,
            6,
            21.74m,
            true
        );

        var response = await _client.PostAsJsonAsync("/api/consoles", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ConsoleDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("GameCube");
        response.Headers.Location.Should().NotBeNull();
    }

    [Test]
    public async Task Create_WhenReleaseYearPredatesManufacturer_Returns409Conflict()
    {
        var mResponse = await _client.GetAsync("/api/manufacturers");
        var manufacturers = await mResponse.Content.ReadFromJsonAsync<IReadOnlyList<ManufacturerDto>>();
        var sony = manufacturers!.First(m => m.Name.Contains("Sony"));

        var request = new CreateConsoleRequest(
            sony.Id,
            "Sony Retro Prototype",
            1980, // Sony founded in 1993
            1,
            0.5m,
            true
        );

        var response = await _client.PostAsJsonAsync("/api/consoles", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(409);
    }

    [Test]
    public async Task Update_WithValidData_Returns200Ok()
    {
        var getAllResponse = await _client.GetAsync("/api/consoles");
        var consoles = await getAllResponse.Content.ReadFromJsonAsync<IReadOnlyList<ConsoleDto>>();
        var first = consoles!.First();

        var updateRequest = new UpdateConsoleRequest(
            first.Name + " Special Edition",
            first.ReleaseYear,
            first.Generation,
            first.UnitsSoldInMillions + 1.0m,
            first.IsDiscontinued
        );

        var response = await _client.PutAsJsonAsync($"/api/consoles/{first.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ConsoleDto>();
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(first.Name + " Special Edition");
    }

    [Test]
    public async Task Delete_WhenExists_Returns204NoContent()
    {
        var getAllResponse = await _client.GetAsync("/api/consoles");
        var consoles = await getAllResponse.Content.ReadFromJsonAsync<IReadOnlyList<ConsoleDto>>();
        var first = consoles!.First();

        var response = await _client.DeleteAsync($"/api/consoles/{first.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/consoles/{first.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
