using PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

namespace PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for AssetType aggregate
/// </summary>
public interface IAssetTypeRepository {
    Task<AssetType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AssetType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<AssetType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AssetType>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(AssetType assetType, CancellationToken cancellationToken = default);
    void Update(AssetType assetType);
    void Delete(AssetType assetType);
}