using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence
{
    public interface ITechnologyRepository
    {
        Task AddAsync(Technology challenge, CancellationToken cancellationToken);
        Task<Technology?> GetByIdAsync(string id, CancellationToken cancellationToken);
        void Update(Technology challenge, CancellationToken cancellationToken);
    }
}
