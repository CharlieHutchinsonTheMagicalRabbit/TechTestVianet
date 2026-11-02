using Microsoft.EntityFrameworkCore;
using RetroGaming.Domain.Entities;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.UseCases.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Manufacturer> Manufacturers { get; }
    DbSet<Console> Consoles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
