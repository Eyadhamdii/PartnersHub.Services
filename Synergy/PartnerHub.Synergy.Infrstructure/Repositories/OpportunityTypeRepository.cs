using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Infrastructure.Persistence;

public class OpportunityTypeRepository : IOpportunityTypeRepository
{
    private readonly SynergyDbContext _context;

    public OpportunityTypeRepository(SynergyDbContext context)
    {
        _context = context;
    }
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.OpportunityTypes.AnyAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<OpportunityType>> GetAllAsync()
    {
        return await _context.OpportunityTypes.ToListAsync();
    }
    public async Task<OpportunityType> GetById(int id)
    {
        return await _context.OpportunityTypes.FirstAsync(ot => ot.Id == id);
    }
}