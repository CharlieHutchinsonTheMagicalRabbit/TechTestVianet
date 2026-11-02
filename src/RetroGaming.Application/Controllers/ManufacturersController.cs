using Microsoft.AspNetCore.Mvc;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManufacturersController : ControllerBase
{
    private readonly IManufacturerService _manufacturerService;

    public ManufacturersController(IManufacturerService manufacturerService)
    {
        _manufacturerService = manufacturerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ManufacturerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ManufacturerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var manufacturers = await _manufacturerService.GetAllAsync(cancellationToken);
        return Ok(manufacturers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ManufacturerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ManufacturerDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var manufacturer = await _manufacturerService.GetByIdAsync(id, cancellationToken);
        return Ok(manufacturer);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ManufacturerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ManufacturerDto>> Create([FromBody] CreateManufacturerRequest request, CancellationToken cancellationToken)
    {
        var created = await _manufacturerService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ManufacturerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ManufacturerDto>> Update(Guid id, [FromBody] UpdateManufacturerRequest request, CancellationToken cancellationToken)
    {
        var updated = await _manufacturerService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _manufacturerService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
