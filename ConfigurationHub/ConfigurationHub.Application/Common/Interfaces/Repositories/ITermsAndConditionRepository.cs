using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;
using PartnersHub.ConfigurationHub.Domain.Enums;

namespace PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for TermsAndCondition aggregate
/// </summary>
public interface ITermsAndConditionRepository {
    Task<TermsAndCondition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermsAndCondition>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TermsAndCondition?> GetActiveByTypeAsync(TermsType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermsAndCondition>> GetByTypeAsync(TermsType type, CancellationToken cancellationToken = default);
    Task<TermsAndCondition?> GetByVersionAsync(string version, TermsType type, CancellationToken cancellationToken = default);
    Task<bool> VersionExistsAsync(string version, TermsType type, CancellationToken cancellationToken = default);
    Task AddAsync(TermsAndCondition termsAndCondition, CancellationToken cancellationToken = default);
    void Update(TermsAndCondition termsAndCondition);
    void Delete(TermsAndCondition termsAndCondition);
}