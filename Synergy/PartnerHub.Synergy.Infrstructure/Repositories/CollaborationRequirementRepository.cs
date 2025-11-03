using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Infrastructure.Persistence;

public class CollaborationRequirementRepository : ICollaborationRequirementRepository
{
    private readonly SynergyDbContext _context;

    public CollaborationRequirementRepository(SynergyDbContext context)
    {
        _context = context;
    }

    public async Task<List<CollaborationRequirement>> GetAllAsync()
    {
        return await _context.CollaborationRequirements.ToListAsync();
    }
    public async Task<List<CollaborationRequirement>> GetByIdsAsync(List<int> Ids)
    {
        return await _context.CollaborationRequirements.Where(eo => Ids.Contains(eo.Id)).ToListAsync();
    }
}