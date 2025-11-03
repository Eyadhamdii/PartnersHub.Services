using PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

namespace PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for UnitOfMeasurement aggregate
/// </summary>
public interface IUnitOfMeasurementRepository {
    Task<UnitOfMeasurement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UnitOfMeasurement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<UnitOfMeasurement>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UnitOfMeasurement>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(UnitOfMeasurement unitOfMeasurement, CancellationToken cancellationToken = default);
    void Update(UnitOfMeasurement unitOfMeasurement);
    void Delete(UnitOfMeasurement unitOfMeasurement);
}