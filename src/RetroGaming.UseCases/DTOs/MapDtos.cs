namespace RetroGaming.UseCases.DTOs;

public record ManufacturerMapLocationDto(
    Guid Id,
    string Name,
    string LocationString,
    double Latitude,
    double Longitude,
    int ConsoleCount
);
