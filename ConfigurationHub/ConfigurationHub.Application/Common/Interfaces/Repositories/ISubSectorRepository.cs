using PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

namespace PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for SubSector aggregate
/// </summary>
public interface ISubSectorRepository {
    Task<SubSector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SubSector?> GetByCodeAsync(string code, Guid sectorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SubSector>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SubSector>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SubSector>> GetBySectorIdAsync(Guid sectorId, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid sectorId, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(SubSector subSector, CancellationToken cancellationToken = default);
    void Update(SubSector subSector);
    void Delete(SubSector subSector);
}