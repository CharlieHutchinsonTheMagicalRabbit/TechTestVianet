using Microsoft.AspNetCore.Mvc;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MapController : ControllerBase
{
    private readonly IMapService _mapService;

    public MapController(IMapService mapService)
    {
        _mapService = mapService;
    }

    [HttpGet("locations")]
    [ProducesResponseType(typeof(IReadOnlyList<ManufacturerMapLocationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ManufacturerMapLocationDto>>> GetLocations(CancellationToken cancellationToken)
    {
        var locations = await _mapService.GetLocationsAsync(cancellationToken);
        return Ok(locations);
    }
}
