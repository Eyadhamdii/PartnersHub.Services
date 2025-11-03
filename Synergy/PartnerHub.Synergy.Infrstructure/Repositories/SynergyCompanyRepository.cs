using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Infrastructure.Persistence;
using System.Linq.Expressions;

public class SynergyCompanyRepository : ISynergyCompanyRepository
{
    private readonly SynergyDbContext _context;

    public SynergyCompanyRepository(SynergyDbContext context)
    {
        _context = context;
    }

    public async Task<SynergyCompany?> GetByIdAsync(Guid id, bool asNoTracking = false, params Expression<Func<SynergyCompany, object>>[] includes)
    {
        IQueryable<SynergyCompany> query = _context.SynergyCompanies.Where(c => c.Id == id);

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SynergyCompany>> GetAllAsync(bool asNoTracking = false)
    {
        var query = _context.SynergyCompanies.AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public async Task<List<SynergyCompany>> GetByIdsAsync(List<Guid> ids, bool asNoTracking = false)
    {
        var query = _context.SynergyCompanies.Where(sc => ids.Contains(sc.Id));

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    #region Dashboard Methods

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.SynergyCompanies.CountAsync();
    }

    public async Task<List<SynergyCompany>> GetRecentAsync(int count, bool asNoTracking = true)
    {
        var query = _context.SynergyCompanies
            .OrderByDescending(c => c.CreatedAt)
            .Take(count);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    #endregion
}