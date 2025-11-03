using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Infrastructure.Persistence.Repositories
{
    public class SuccessStroyTypeRepository : ISuccessStroyTypeRepository
    {
        private readonly SynergyDbContext _context;

        public SuccessStroyTypeRepository(SynergyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SuccessStoryType>> GetAllAsync()
        {
            return await _context.SuccessStoryTypes.ToListAsync();
        }
    }
}
