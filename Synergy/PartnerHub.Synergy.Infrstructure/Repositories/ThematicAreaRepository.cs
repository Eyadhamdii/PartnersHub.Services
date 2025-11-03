using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Infrastructure.Persistence;

public class ThematicAreaRepository : IThematicAreaRepository
{
    private readonly SynergyDbContext _context;

    public ThematicAreaRepository(SynergyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ThematicArea>> GetAllAsync()
    {
        return await _context.ThematicAreas.ToListAsync();
    }
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ThematicAreas.AnyAsync(o => o.Id == id);
    }
    public async Task<ThematicArea> GetById(int id)
    {
        return await _context.ThematicAreas.FirstAsync(ta => ta.Id == id);
    }
}