using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Common.Interfaces;

public interface IMapService
{
    Task<IReadOnlyList<ManufacturerMapLocationDto>> GetLocationsAsync(CancellationToken cancellationToken = default);
}
