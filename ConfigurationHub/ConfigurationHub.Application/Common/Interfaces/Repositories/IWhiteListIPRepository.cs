using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;

namespace PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for WhiteListIP aggregate
/// </summary>
public interface IWhiteListIPRepository {
    Task<WhiteListIP?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WhiteListIP>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WhiteListIP>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<WhiteListIP?> GetByIPAddressAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task<bool> IsIPWhitelistedAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task AddAsync(WhiteListIP whiteListIP, CancellationToken cancellationToken = default);
    void Update(WhiteListIP whiteListIP);
    void Delete(WhiteListIP whiteListIP);
}