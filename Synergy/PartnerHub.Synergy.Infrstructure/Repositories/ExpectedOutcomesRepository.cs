using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Infrastructure.Persistence;

public class ExpectedOutcomesRepository : IExpectedOutcomesRepository
{
    private readonly SynergyDbContext _context;

    public ExpectedOutcomesRepository(SynergyDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpectedOutcome>> GetAllAsync()
    {
        return await _context.ExpectedOutcomes.ToListAsync();
    }
    public async Task<List<ExpectedOutcome>> GetByIdsAsync(List<int> Ids)
    {
        return await _context.ExpectedOutcomes.Where(eo => Ids.Contains(eo.Id)).ToListAsync();
    }
}
