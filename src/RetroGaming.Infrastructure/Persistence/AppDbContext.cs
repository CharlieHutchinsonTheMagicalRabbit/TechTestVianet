using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.Domain.Entities;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Console> Consoles => Set<Console>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Seed();
    }
}
