using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.UseCases.Services;

namespace RetroGaming.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IManufacturerService, ManufacturerService>();
        services.AddScoped<IConsoleService, ConsoleService>();
        services.AddScoped<IMapService, MapService>();

        return services;
    }
}
