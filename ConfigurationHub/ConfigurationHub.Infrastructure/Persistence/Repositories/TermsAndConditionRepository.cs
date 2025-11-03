using Microsoft.EntityFrameworkCore;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;
using PartnersHub.ConfigurationHub.Domain.Enums;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence.Repositories;

public class TermsAndConditionRepository : ITermsAndConditionRepository {
    private readonly ConfigurationHubDbContext _context;

    public TermsAndConditionRepository(ConfigurationHubDbContext context) {
        _context = context;
    }

    public async Task<TermsAndCondition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _context.TermsAndConditions
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TermsAndCondition>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _context.TermsAndConditions
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TermsAndCondition?> GetActiveByTypeAsync(TermsType type, CancellationToken cancellationToken = default) {
        return await _context.TermsAndConditions
            .Where(t => t.Type == type
                       && t.Status == TermsStatus.Active
                       && t.EffectiveDate <= DateTime.UtcNow
                       && (!t.ExpiryDate.HasValue || t.ExpiryDate.Value > DateTime.UtcNow))
            .OrderByDescending(t => t.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TermsAndCondition>> GetByTypeAsync(TermsType type, CancellationToken cancellationToken = default) {
        return await _context.TermsAndConditions
            .Where(t => t.Type == type)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TermsAndCondition?> GetByVersionAsync(string version, TermsType type, CancellationToken cancellationToken = default) {
        return await _context.TermsAndConditions
            .FirstOrDefaultAsync(t => t.Version == version && t.Type == type, cancellationToken);
    }

    public async Task<bool> VersionExistsAsync(string version, TermsType type, CancellationToken cancellationToken = default) {
        return await _context.TermsAndConditions
            .AnyAsync(t => t.Version == version && t.Type == type, cancellationToken);
    }

    public async Task AddAsync(TermsAndCondition termsAndCondition, CancellationToken cancellationToken = default) {
        await _context.TermsAndConditions.AddAsync(termsAndCondition, cancellationToken);
    }

    public void Update(TermsAndCondition termsAndCondition) {
        _context.TermsAndConditions.Update(termsAndCondition);
    }

    public void Delete(TermsAndCondition termsAndCondition) {
        _context.TermsAndConditions.Remove(termsAndCondition);
    }
}