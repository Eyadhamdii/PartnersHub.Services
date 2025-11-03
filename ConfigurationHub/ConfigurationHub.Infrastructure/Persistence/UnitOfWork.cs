using PartnersHub.ConfigurationHub.Application.Common.Interfaces;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork {
    private readonly ConfigurationHubDbContext _context;

    public UnitOfWork(ConfigurationHubDbContext context) {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}