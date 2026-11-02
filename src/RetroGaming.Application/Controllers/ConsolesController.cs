using Microsoft.AspNetCore.Mvc;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsolesController : ControllerBase
{
    private readonly IConsoleService _consoleService;

    public ConsolesController(IConsoleService consoleService)
    {
        _consoleService = consoleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConsoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ConsoleDto>>> GetAll([FromQuery] ConsoleSearchQuery query, CancellationToken cancellationToken)
    {
        var consoles = await _consoleService.GetAllAsync(query, cancellationToken);
        return Ok(consoles);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConsoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsoleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var console = await _consoleService.GetByIdAsync(id, cancellationToken);
        return Ok(console);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConsoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConsoleDto>> Create([FromBody] CreateConsoleRequest request, CancellationToken cancellationToken)
    {
        var created = await _consoleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ConsoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConsoleDto>> Update(Guid id, [FromBody] UpdateConsoleRequest request, CancellationToken cancellationToken)
    {
        var updated = await _consoleService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _consoleService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
