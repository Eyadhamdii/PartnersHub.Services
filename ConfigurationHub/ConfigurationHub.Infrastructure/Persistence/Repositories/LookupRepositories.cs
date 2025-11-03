using Microsoft.EntityFrameworkCore;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence.Repositories;

public class SectorRepository : ISectorRepository {
    private readonly ConfigurationHubDbContext _context;

    public SectorRepository(ConfigurationHubDbContext context) {
        _context = context;
    }

    public async Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _context.Sectors
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sector?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) {
        return await _context.Sectors
            .FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Sector>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _context.Sectors
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sector>> GetActiveAsync(CancellationToken cancellationToken = default) {
        return await _context.Sectors
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) {
        var query = _context.Sectors.Where(s => s.Code == code);

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Sector sector, CancellationToken cancellationToken = default) {
        await _context.Sectors.AddAsync(sector, cancellationToken);
    }

    public void Update(Sector sector) {
        _context.Sectors.Update(sector);
    }

    public void Delete(Sector sector) {
        _context.Sectors.Remove(sector);
    }
}

public class SubSectorRepository : ISubSectorRepository {
    private readonly ConfigurationHubDbContext _context;

    public SubSectorRepository(ConfigurationHubDbContext context) {
        _context = context;
    }

    public async Task<SubSector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _context.SubSectors
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<SubSector?> GetByCodeAsync(string code, Guid sectorId, CancellationToken cancellationToken = default) {
        return await _context.SubSectors
            .FirstOrDefaultAsync(s => s.Code == code && s.SectorId == sectorId, cancellationToken);
    }

    public async Task<IEnumerable<SubSector>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _context.SubSectors
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SubSector>> GetActiveAsync(CancellationToken cancellationToken = default) {
        return await _context.SubSectors
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SubSector>> GetBySectorIdAsync(Guid sectorId, CancellationToken cancellationToken = default) {
        return await _context.SubSectors
            .Where(s => s.SectorId == sectorId && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid sectorId, Guid? excludeId = null, CancellationToken cancellationToken = default) {
        var query = _context.SubSectors.Where(s => s.Code == code && s.SectorId == sectorId);

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(SubSector subSector, CancellationToken cancellationToken = default) {
        await _context.SubSectors.AddAsync(subSector, cancellationToken);
    }

    public void Update(SubSector subSector) {
        _context.SubSectors.Update(subSector);
    }

    public void Delete(SubSector subSector) {
        _context.SubSectors.Remove(subSector);
    }
}

public class AssetTypeRepository : IAssetTypeRepository {
    private readonly ConfigurationHubDbContext _context;

    public AssetTypeRepository(ConfigurationHubDbContext context) {
        _context = context;
    }

    public async Task<AssetType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _context.AssetTypes
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<AssetType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) {
        return await _context.AssetTypes
            .FirstOrDefaultAsync(a => a.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<AssetType>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _context.AssetTypes
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AssetType>> GetActiveAsync(CancellationToken cancellationToken = default) {
        return await _context.AssetTypes
            .Where(a => a.IsActive)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) {
        var query = _context.AssetTypes.Where(a => a.Code == code);

        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(AssetType assetType, CancellationToken cancellationToken = default) {
        await _context.AssetTypes.AddAsync(assetType, cancellationToken);
    }

    public void Update(AssetType assetType) {
        _context.AssetTypes.Update(assetType);
    }

    public void Delete(AssetType assetType) {
        _context.AssetTypes.Remove(assetType);
    }
}

public class UnitOfMeasurementRepository : IUnitOfMeasurementRepository {
    private readonly ConfigurationHubDbContext _context;

    public UnitOfMeasurementRepository(ConfigurationHubDbContext context) {
        _context = context;
    }

    public async Task<UnitOfMeasurement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _context.UnitsOfMeasurement
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UnitOfMeasurement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) {
        return await _context.UnitsOfMeasurement
            .FirstOrDefaultAsync(u => u.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<UnitOfMeasurement>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _context.UnitsOfMeasurement
            .OrderBy(u => u.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UnitOfMeasurement>> GetActiveAsync(CancellationToken cancellationToken = default) {
        return await _context.UnitsOfMeasurement
            .Where(u => u.IsActive)
            .OrderBy(u => u.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) {
        var query = _context.UnitsOfMeasurement.Where(u => u.Code == code);

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(UnitOfMeasurement unitOfMeasurement, CancellationToken cancellationToken = default) {
        await _context.UnitsOfMeasurement.AddAsync(unitOfMeasurement, cancellationToken);
    }

    public void Update(UnitOfMeasurement unitOfMeasurement) {
        _context.UnitsOfMeasurement.Update(unitOfMeasurement);
    }

    public void Delete(UnitOfMeasurement unitOfMeasurement) {
        _context.UnitsOfMeasurement.Remove(unitOfMeasurement);
    }
}