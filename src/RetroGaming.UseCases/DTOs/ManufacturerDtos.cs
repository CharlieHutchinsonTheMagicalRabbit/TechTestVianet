namespace RetroGaming.UseCases.DTOs;

public record ManufacturerDto(
    Guid Id,
    string Name,
    string HqCity,
    string HqCountry,
    int FoundedYear,
    double Latitude,
    double Longitude,
    int ConsoleCount
);

public record ManufacturerDetailDto(
    Guid Id,
    string Name,
    string HqCity,
    string HqCountry,
    int FoundedYear,
    double Latitude,
    double Longitude,
    IReadOnlyList<ConsoleDto> Consoles
);

public record CreateManufacturerRequest(
    string Name,
    string HqCity,
    string HqCountry,
    int FoundedYear,
    double Latitude,
    double Longitude
);

public record UpdateManufacturerRequest(
    string Name,
    string HqCity,
    string HqCountry,
    int FoundedYear,
    double Latitude,
    double Longitude
);
