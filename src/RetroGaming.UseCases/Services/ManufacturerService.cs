using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RetroGaming.UseCases.Common.Interfaces;
using RetroGaming.UseCases.DTOs;
using RetroGaming.Domain.Entities;
using RetroGaming.Domain.Exceptions;

namespace RetroGaming.UseCases.Services;

public class ManufacturerService : IManufacturerService
{
    private readonly IAppDbContext _dbContext;
    private readonly IValidator<CreateManufacturerRequest> _createValidator;
    private readonly IValidator<UpdateManufacturerRequest> _updateValidator;

    public ManufacturerService(
        IAppDbContext dbContext,
        IValidator<CreateManufacturerRequest> createValidator,
        IValidator<UpdateManufacturerRequest> updateValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ManufacturerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Manufacturers
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .Select(m => new ManufacturerDto(
                m.Id,
                m.Name,
                m.HqCity,
                m.HqCountry,
                m.FoundedYear,
                m.Latitude,
                m.Longitude,
                m.Consoles.Count
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<ManufacturerDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var manufacturer = await _dbContext.Manufacturers
            .AsNoTracking()
            .Include(m => m.Consoles)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (manufacturer == null)
        {
            throw new EntityNotFoundException(nameof(Manufacturer), id);
        }

        var consoles = manufacturer.Consoles
            .OrderBy(c => c.ReleaseYear)
            .ThenBy(c => c.Name)
            .Select(c => new ConsoleDto(
                c.Id,
                c.ManufacturerId,
                manufacturer.Name,
                c.Name,
                c.ReleaseYear,
                c.Generation,
                c.UnitsSoldInMillions,
                c.IsDiscontinued
            ))
            .ToList();

        return new ManufacturerDetailDto(
            manufacturer.Id,
            manufacturer.Name,
            manufacturer.HqCity,
            manufacturer.HqCountry,
            manufacturer.FoundedYear,
            manufacturer.Latitude,
            manufacturer.Longitude,
            consoles
        );
    }

    public async Task<ManufacturerDto> CreateAsync(CreateManufacturerRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var normalizedName = request.Name.Trim();
        var nameExists = await _dbContext.Manufacturers
            .AnyAsync(m => m.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (nameExists)
        {
            throw new InvalidDomainOperationException($"A manufacturer named '{normalizedName}' already exists.");
        }

        var manufacturer = new Manufacturer(
            Guid.NewGuid(),
            request.Name,
            request.HqCity,
            request.HqCountry,
            request.FoundedYear,
            request.Latitude,
            request.Longitude
        );

        _dbContext.Manufacturers.Add(manufacturer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ManufacturerDto(
            manufacturer.Id,
            manufacturer.Name,
            manufacturer.HqCity,
            manufacturer.HqCountry,
            manufacturer.FoundedYear,
            manufacturer.Latitude,
            manufacturer.Longitude,
            0
        );
    }

    public async Task<ManufacturerDto> UpdateAsync(Guid id, UpdateManufacturerRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var manufacturer = await _dbContext.Manufacturers
            .Include(m => m.Consoles)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (manufacturer == null)
        {
            throw new EntityNotFoundException(nameof(Manufacturer), id);
        }

        var normalizedName = request.Name.Trim();
        var nameExists = await _dbContext.Manufacturers
            .AnyAsync(m => m.Id != id && m.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (nameExists)
        {
            throw new InvalidDomainOperationException($"A manufacturer named '{normalizedName}' already exists.");
        }

        manufacturer.UpdateDetails(
            request.Name,
            request.HqCity,
            request.HqCountry,
            request.FoundedYear,
            request.Latitude,
            request.Longitude
        );

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ManufacturerDto(
            manufacturer.Id,
            manufacturer.Name,
            manufacturer.HqCity,
            manufacturer.HqCountry,
            manufacturer.FoundedYear,
            manufacturer.Latitude,
            manufacturer.Longitude,
            manufacturer.Consoles.Count
        );
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var manufacturer = await _dbContext.Manufacturers
            .Include(m => m.Consoles)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (manufacturer == null)
        {
            throw new EntityNotFoundException(nameof(Manufacturer), id);
        }

        if (manufacturer.Consoles.Count > 0)
        {
            throw new InvalidDomainOperationException(
                $"Cannot delete manufacturer '{manufacturer.Name}' because it has {manufacturer.Consoles.Count} associated console(s). Delete or reassign the consoles first.");
        }

        _dbContext.Manufacturers.Remove(manufacturer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
