using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.Tests.Integration.Controllers;

[TestFixture]
public class MapControllerTests
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
    public async Task GetLocations_ReturnsLocationsWithConsoleCounts()
    {
        var response = await _client.GetAsync("/api/map/locations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var locations = await response.Content.ReadFromJsonAsync<IReadOnlyList<ManufacturerMapLocationDto>>();
        locations.Should().NotBeNull();
        locations!.Count.Should().BeGreaterThanOrEqualTo(5);

        var nintendo = locations.FirstOrDefault(l => l.Name == "Nintendo");
        nintendo.Should().NotBeNull();
        nintendo!.LocationString.Should().Be("Kyoto, Japan");
        nintendo.Latitude.Should().BeApproximately(34.9699, 0.001);
        nintendo.Longitude.Should().BeApproximately(135.7563, 0.001);
        nintendo.ConsoleCount.Should().BeGreaterThan(0);
    }
}
