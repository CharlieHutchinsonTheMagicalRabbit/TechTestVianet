using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Common.Interfaces;

public interface IManufacturerService
{
    Task<IReadOnlyList<ManufacturerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ManufacturerDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ManufacturerDto> CreateAsync(CreateManufacturerRequest request, CancellationToken cancellationToken = default);
    Task<ManufacturerDto> UpdateAsync(Guid id, UpdateManufacturerRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
