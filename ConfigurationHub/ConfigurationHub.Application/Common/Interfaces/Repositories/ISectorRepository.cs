using PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

namespace PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for Sector aggregate
/// </summary>
public interface ISectorRepository {
    Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sector?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sector>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sector>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Sector sector, CancellationToken cancellationToken = default);
    void Update(Sector sector);
    void Delete(Sector sector);
}