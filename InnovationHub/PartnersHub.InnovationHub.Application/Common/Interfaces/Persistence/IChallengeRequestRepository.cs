using PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using PartnersHub.InnovationHub.Application.Common.Paging;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Enums;


namespace PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence
{
    public interface IChallengeRequestRepository
    {
        Task AddAsync(ChallengeRequest challenge, CancellationToken cancellationToken);
        Task<ChallengeRequest?> GetById(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<ChallengeRequest>> GetByUserId(Guid userId, CancellationToken cancellationToken);
        Task Update(ChallengeRequest challenge, CancellationToken cancellationToken);
        Task Delete(ChallengeRequest challenge, CancellationToken cancellationToken);
        Task<PagingResult<ChallengeCardDTO>> ListAsync(ChallengeRequestListQuery filter, CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);



    }
}
