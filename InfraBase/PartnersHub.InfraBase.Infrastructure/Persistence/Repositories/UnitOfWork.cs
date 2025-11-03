using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Infrastructure.Persistence;

namespace PartnersHub.InfraBase.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of Unit of Work pattern
/// Manages database transactions through EF Core DbContext
/// </summary>
public class UnitOfWork : IUnitOfWork {
    private readonly InfrabaseDbContext _context;

    public UnitOfWork(InfrabaseDbContext context) {
        _context = context;
    }

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
