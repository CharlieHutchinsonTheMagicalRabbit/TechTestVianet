using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.UseCases.DTOs;
using RetroGaming.Domain.Entities;
using RetroGaming.Domain.Exceptions;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.UseCases.Services;

public class ConsoleService : IConsoleService
{
    private readonly IAppDbContext _dbContext;
    private readonly IValidator<CreateConsoleRequest> _createValidator;
    private readonly IValidator<UpdateConsoleRequest> _updateValidator;

    public ConsoleService(
        IAppDbContext dbContext,
        IValidator<CreateConsoleRequest> createValidator,
        IValidator<UpdateConsoleRequest> updateValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ConsoleDto>> GetAllAsync(ConsoleSearchQuery? query = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Console> consolesQuery = _dbContext.Consoles
            .AsNoTracking()
            .Include(c => c.Manufacturer);

        if (query != null)
        {
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim().ToLower();
                consolesQuery = consolesQuery.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    (c.Manufacturer != null && c.Manufacturer.Name.ToLower().Contains(term)));
            }

            if (query.ManufacturerId.HasValue)
            {
                consolesQuery = consolesQuery.Where(c => c.ManufacturerId == query.ManufacturerId.Value);
            }

            if (query.Generation.HasValue)
            {
                consolesQuery = consolesQuery.Where(c => c.Generation == query.Generation.Value);
            }

            if (query.IsDiscontinued.HasValue)
            {
                consolesQuery = consolesQuery.Where(c => c.IsDiscontinued == query.IsDiscontinued.Value);
            }

            // Sorting
            consolesQuery = (query.SortBy?.ToLower()) switch
            {
                "name" => query.SortDescending
                    ? consolesQuery.OrderByDescending(c => c.Name)
                    : consolesQuery.OrderBy(c => c.Name),
                "releaseyear" => query.SortDescending
                    ? consolesQuery.OrderByDescending(c => c.ReleaseYear).ThenByDescending(c => c.Name)
                    : consolesQuery.OrderBy(c => c.ReleaseYear).ThenBy(c => c.Name),
                "generation" => query.SortDescending
                    ? consolesQuery.OrderByDescending(c => c.Generation).ThenByDescending(c => c.ReleaseYear)
                    : consolesQuery.OrderBy(c => c.Generation).ThenBy(c => c.ReleaseYear),
                "unitssold" or "unitssoldinmillions" => query.SortDescending
                    ? consolesQuery.OrderByDescending(c => c.UnitsSoldInMillions).ThenByDescending(c => c.Name)
                    : consolesQuery.OrderBy(c => c.UnitsSoldInMillions).ThenBy(c => c.Name),
                "manufacturer" => query.SortDescending
                    ? consolesQuery.OrderByDescending(c => c.Manufacturer != null ? c.Manufacturer.Name : string.Empty).ThenByDescending(c => c.Name)
                    : consolesQuery.OrderBy(c => c.Manufacturer != null ? c.Manufacturer.Name : string.Empty).ThenBy(c => c.Name),
                _ => query.SortDescending
                    ? consolesQuery.OrderByDescending(c => c.ReleaseYear).ThenByDescending(c => c.Name)
                    : consolesQuery.OrderBy(c => c.ReleaseYear).ThenBy(c => c.Name)
            };
        }
        else
        {
            consolesQuery = consolesQuery.OrderBy(c => c.ReleaseYear).ThenBy(c => c.Name);
        }

        return await consolesQuery
            .Select(c => new ConsoleDto(
                c.Id,
                c.ManufacturerId,
                c.Manufacturer != null ? c.Manufacturer.Name : string.Empty,
                c.Name,
                c.ReleaseYear,
                c.Generation,
                c.UnitsSoldInMillions,
                c.IsDiscontinued
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<ConsoleDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var console = await _dbContext.Consoles
            .AsNoTracking()
            .Include(c => c.Manufacturer)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (console == null)
        {
            throw new EntityNotFoundException(nameof(Console), id);
        }

        return new ConsoleDto(
            console.Id,
            console.ManufacturerId,
            console.Manufacturer != null ? console.Manufacturer.Name : string.Empty,
            console.Name,
            console.ReleaseYear,
            console.Generation,
            console.UnitsSoldInMillions,
            console.IsDiscontinued
        );
    }

    public async Task<ConsoleDto> CreateAsync(CreateConsoleRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var manufacturer = await _dbContext.Manufacturers
            .Include(m => m.Consoles)
            .FirstOrDefaultAsync(m => m.Id == request.ManufacturerId, cancellationToken);

        if (manufacturer == null)
        {
            throw new EntityNotFoundException(nameof(Manufacturer), request.ManufacturerId);
        }

        var normalizedName = request.Name.Trim();
        var consoleExists = await _dbContext.Consoles
            .AnyAsync(c => c.ManufacturerId == request.ManufacturerId && c.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (consoleExists)
        {
            throw new InvalidDomainOperationException(
                $"A console named '{normalizedName}' already exists for manufacturer '{manufacturer.Name}'.");
        }

        var console = new Console(
            Guid.NewGuid(),
            request.ManufacturerId,
            request.Name,
            request.ReleaseYear,
            request.Generation,
            request.UnitsSoldInMillions,
            request.IsDiscontinued
        );

        // Add console via manufacturer domain method to validate release year vs founded year & name uniqueness
        manufacturer.AddConsole(console);

        _dbContext.Consoles.Add(console);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ConsoleDto(
            console.Id,
            console.ManufacturerId,
            manufacturer.Name,
            console.Name,
            console.ReleaseYear,
            console.Generation,
            console.UnitsSoldInMillions,
            console.IsDiscontinued
        );
    }

    public async Task<ConsoleDto> UpdateAsync(Guid id, UpdateConsoleRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var console = await _dbContext.Consoles
            .Include(c => c.Manufacturer)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (console == null)
        {
            throw new EntityNotFoundException(nameof(Console), id);
        }

        var targetManufacturer = await _dbContext.Manufacturers
            .Include(m => m.Consoles)
            .FirstOrDefaultAsync(m => m.Id == console.ManufacturerId, cancellationToken);

        if (targetManufacturer == null)
        {
            throw new EntityNotFoundException(nameof(Manufacturer), console.ManufacturerId);
        }

        var normalizedName = request.Name.Trim();
        var duplicateName = await _dbContext.Consoles
            .AnyAsync(c => c.Id != id && c.ManufacturerId == console.ManufacturerId && c.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (duplicateName)
        {
            throw new InvalidDomainOperationException(
                $"A console named '{normalizedName}' already exists for manufacturer '{targetManufacturer.Name}'.");
        }

        if (request.ReleaseYear < targetManufacturer.FoundedYear)
        {
            throw new InvalidDomainOperationException(
                $"Console release year ({request.ReleaseYear}) cannot predate manufacturer founded year ({targetManufacturer.FoundedYear}).");
        }

        console.UpdateDetails(
            request.Name,
            request.ReleaseYear,
            request.Generation,
            request.UnitsSoldInMillions,
            request.IsDiscontinued
        );

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ConsoleDto(
            console.Id,
            console.ManufacturerId,
            targetManufacturer.Name,
            console.Name,
            console.ReleaseYear,
            console.Generation,
            console.UnitsSoldInMillions,
            console.IsDiscontinued
        );
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var console = await _dbContext.Consoles
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (console == null)
        {
            throw new EntityNotFoundException(nameof(Console), id);
        }

        _dbContext.Consoles.Remove(console);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
