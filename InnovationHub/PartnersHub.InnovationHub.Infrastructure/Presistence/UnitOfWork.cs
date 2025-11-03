using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Infrastructure.Presistence;



namespace PartnersHub.InnovationHub.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly InnovationHubDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UnitOfWork(InnovationHubDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public  async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var item in _context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (item.State)
            {
                case EntityState.Added:
                    item.Entity.CreatedAt = DateTime.UtcNow;
                    item.Entity.CreatedBy = string.IsNullOrEmpty(_currentUser?.UserId) ? "Na" : _currentUser.UserId;
                    break;

                case EntityState.Modified:
                    item.Entity.UpdatedAt = DateTime.UtcNow;
                    item.Entity.UpdatedBy = string.IsNullOrEmpty(_currentUser?.UserId) ? "Na" : _currentUser.UserId;
                    break;
            }
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }
}
