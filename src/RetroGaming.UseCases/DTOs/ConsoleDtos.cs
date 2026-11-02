namespace RetroGaming.UseCases.DTOs;

public record ConsoleDto(
    Guid Id,
    Guid ManufacturerId,
    string ManufacturerName,
    string Name,
    int ReleaseYear,
    int Generation,
    decimal UnitsSoldInMillions,
    bool IsDiscontinued
);

public record CreateConsoleRequest(
    Guid ManufacturerId,
    string Name,
    int ReleaseYear,
    int Generation,
    decimal UnitsSoldInMillions,
    bool IsDiscontinued
);

public record UpdateConsoleRequest(
    Guid ManufacturerId,
    string Name,
    int ReleaseYear,
    int Generation,
    decimal UnitsSoldInMillions,
    bool IsDiscontinued
);

public record ConsoleSearchQuery(
    string? SearchTerm = null,
    Guid? ManufacturerId = null,
    int? Generation = null,
    bool? IsDiscontinued = null,
    string? SortBy = null,
    bool SortDescending = false
);
