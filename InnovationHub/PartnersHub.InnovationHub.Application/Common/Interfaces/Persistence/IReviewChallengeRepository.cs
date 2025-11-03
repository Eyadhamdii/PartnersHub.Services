using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence
{
    public interface IReviewChallengeRepository
    {

        Task AddAsync(ChallengeRequestRevisionComment comment, CancellationToken cancellationToken);
    }
}
