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
    public class ChallengeTechnologiesRequestRepository(InnovationHubDbContext dbContext) : IChallengeTechnologiesRequestRepository
    {
        public async Task AddAsync(ChallengeTechnologiesRequest technologyRequest, CancellationToken cancellationToken)
        {
            await dbContext.challengeTechnologiesRequests.AddAsync(technologyRequest, cancellationToken);
        }

        public async Task<ChallengeTechnologiesRequest?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.challengeTechnologiesRequests.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ChallengeTechnologiesRequest>> GetByChallengeId(Guid challengeId, CancellationToken cancellationToken)
        {
            return dbContext.challengeTechnologiesRequests.Where(c => c.ChallengeRequestId == challengeId);
        }

        public void Update(ChallengeTechnologiesRequest technologyRequest, CancellationToken cancellationToken)
        {
            dbContext.challengeTechnologiesRequests.Update(technologyRequest);
        }

        public async Task<bool> CheckExistedTechnologies(Guid challengeId, CancellationToken cancellationToken)
        {
            return await dbContext.challengeTechnologiesRequests.AnyAsync(c => c.ChallengeRequestId == challengeId, cancellationToken);
        }

        public async Task<bool> CheckDuplicateTechnologies(Guid challengeId, Guid technologyId, CancellationToken cancellationToken)
        {
            return await dbContext.challengeTechnologiesRequests.AnyAsync(c => c.ChallengeRequestId == challengeId && c.LinkedTechnology.Id == technologyId, cancellationToken);
        }
    }
}
