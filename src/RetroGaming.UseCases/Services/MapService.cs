using Microsoft.EntityFrameworkCore;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.UseCases.DTOs;

namespace RetroGaming.UseCases.Services;

public class MapService : IMapService
{
    private readonly IAppDbContext _dbContext;

    public MapService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ManufacturerMapLocationDto>> GetLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Manufacturers
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .Select(m => new ManufacturerMapLocationDto(
                m.Id,
                m.Name,
                $"{m.HqCity}, {m.HqCountry}",
                m.Latitude,
                m.Longitude,
                m.Consoles.Count
            ))
            .ToListAsync(cancellationToken);
    }
}
