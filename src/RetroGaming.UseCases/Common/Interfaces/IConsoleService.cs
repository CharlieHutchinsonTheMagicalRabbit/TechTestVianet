using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Common.Interfaces;

public interface IConsoleService
{
    Task<IReadOnlyList<ConsoleDto>> GetAllAsync(ConsoleSearchQuery? query = null, CancellationToken cancellationToken = default);
    Task<ConsoleDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ConsoleDto> CreateAsync(CreateConsoleRequest request, CancellationToken cancellationToken = default);
    Task<ConsoleDto> UpdateAsync(Guid id, UpdateConsoleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
