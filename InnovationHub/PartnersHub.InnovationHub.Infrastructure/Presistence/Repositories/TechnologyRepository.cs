using Microsoft.EntityFrameworkCore;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Infrastructure.Presistence.Repositories
{
    public class TechnologyRepository(InnovationHubDbContext dbContext) : ITechnologyRepository
    {
        public async Task AddAsync(Technology technology, CancellationToken cancellationToken)
        {
            await dbContext.technologies.AddAsync(technology, cancellationToken);
        }

        public async Task<Technology?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await dbContext.technologies.FirstOrDefaultAsync(c => c.Id.ToString() == id, cancellationToken);
        }
        public void Update(Technology technology, CancellationToken cancellationToken)
        {
            dbContext.technologies.Update(technology);
        }
    }
}
