using Microsoft.EntityFrameworkCore;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;


namespace PartnersHub.InnovationHub.Infrastructure.Presistence.Repositories
{
    public class ChallengeRequestRevisionCommentRepository(InnovationHubDbContext dbContext)
    {
        public async Task AddAsync(ChallengeRequestRevisionComment comment, CancellationToken cancellationToken)
        {
            await dbContext.challengeRequestRevisionComments.AddAsync(comment, cancellationToken);
        }

        public async Task<ChallengeRequestRevisionComment?> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await dbContext.challengeRequestRevisionComments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ChallengeRequestRevisionComment>> GetByChallengeId(Guid challengeId, CancellationToken cancellationToken)
        {
            return dbContext.challengeRequestRevisionComments.Where(c => c.ChallengeRequestId == challengeId);
        }

        public void Update(ChallengeRequestRevisionComment challengeComment, CancellationToken cancellationToken)
        {
            dbContext.challengeRequestRevisionComments.Update(challengeComment);
        }
    }
}
