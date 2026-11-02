using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.Tests.Integration.Controllers;

[TestFixture]
public class ManufacturersControllerTests
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
    public async Task GetAll_ReturnsSuccessAndManufacturersList()
    {
        var response = await _client.GetAsync("/api/manufacturers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var manufacturers = await response.Content.ReadFromJsonAsync<IReadOnlyList<ManufacturerDto>>();
        manufacturers.Should().NotBeNull();
        manufacturers!.Count.Should().BeGreaterThanOrEqualTo(5); // Seeded manufacturers
    }

    [Test]
    public async Task GetById_WhenExists_ReturnsManufacturerDetailDto()
    {
        var getAllResponse = await _client.GetAsync("/api/manufacturers");
        var manufacturers = await getAllResponse.Content.ReadFromJsonAsync<IReadOnlyList<ManufacturerDto>>();
        var first = manufacturers!.First();

        var response = await _client.GetAsync($"/api/manufacturers/{first.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await response.Content.ReadFromJsonAsync<ManufacturerDetailDto>();
        detail.Should().NotBeNull();
        detail!.Id.Should().Be(first.Id);
        detail.Name.Should().Be(first.Name);
    }

    [Test]
    public async Task GetById_WhenNotFound_ReturnsProblemDetails404()
    {
        var response = await _client.GetAsync($"/api/manufacturers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
    }

    [Test]
    public async Task Create_WithValidData_Returns201CreatedAndLocationHeader()
    {
        var request = new CreateManufacturerRequest(
            "SNK Corporation",
            "Suita",
            "Japan",
            1978,
            34.7570,
            135.5186
        );

        var response = await _client.PostAsJsonAsync("/api/manufacturers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ManufacturerDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("SNK Corporation");
        response.Headers.Location.Should().NotBeNull();
    }

    [Test]
    public async Task Create_WithInvalidData_Returns400BadRequest()
    {
        var request = new CreateManufacturerRequest(
            "", // empty name
            "Suita",
            "Japan",
            1700, // invalid year
            200.0, // invalid lat
            135.5186
        );

        var response = await _client.PostAsJsonAsync("/api/manufacturers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(400);
    }

    [Test]
    public async Task Create_WithDuplicateName_Returns409Conflict()
    {
        var request = new CreateManufacturerRequest(
            "Nintendo", // Already seeded
            "Kyoto",
            "Japan",
            1889,
            34.9699,
            135.7563
        );

        var response = await _client.PostAsJsonAsync("/api/manufacturers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(409);
    }

    [Test]
    public async Task Update_WithValidData_Returns200Ok()
    {
        var getAllResponse = await _client.GetAsync("/api/manufacturers");
        var manufacturers = await getAllResponse.Content.ReadFromJsonAsync<IReadOnlyList<ManufacturerDto>>();
        var first = manufacturers!.First();

        var updateRequest = new UpdateManufacturerRequest(
            first.Name + " Updated",
            first.HqCity,
            first.HqCountry,
            first.FoundedYear,
            first.Latitude,
            first.Longitude
        );

        var response = await _client.PutAsJsonAsync($"/api/manufacturers/{first.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ManufacturerDto>();
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(first.Name + " Updated");
    }

    [Test]
    public async Task Delete_WhenManufacturerHasConsoles_Returns409Conflict()
    {
        var getAllResponse = await _client.GetAsync("/api/manufacturers");
        var manufacturers = await getAllResponse.Content.ReadFromJsonAsync<IReadOnlyList<ManufacturerDto>>();
        var nintendo = manufacturers!.First(m => m.Name == "Nintendo");

        var response = await _client.DeleteAsync($"/api/manufacturers/{nintendo.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(409);
    }

    [Test]
    public async Task Delete_WhenManufacturerHasNoConsoles_Returns204NoContent()
    {
        // Create manufacturer with no consoles
        var createRequest = new CreateManufacturerRequest(
            "Bandai",
            "Tokyo",
            "Japan",
            1950,
            35.6895,
            139.6917
        );
        var createResponse = await _client.PostAsJsonAsync("/api/manufacturers", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ManufacturerDto>();

        var response = await _client.DeleteAsync($"/api/manufacturers/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/manufacturers/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
