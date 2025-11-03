using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence
{
    public interface IChallengeTechnologiesRequestRepository
    {
        Task AddAsync(ChallengeTechnologiesRequest challenge, CancellationToken cancellationToken);
        Task<ChallengeTechnologiesRequest?> GetById(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<ChallengeTechnologiesRequest>> GetByChallengeId(Guid challengeId, CancellationToken cancellationToken);
        Task<bool> CheckExistedTechnologies(Guid challengeId, CancellationToken cancellationToken);
        Task<bool> CheckDuplicateTechnologies(Guid challengeId, Guid technologyId, CancellationToken cancellationToken);
        void Update(ChallengeTechnologiesRequest challenge, CancellationToken cancellationToken);

    }
}
