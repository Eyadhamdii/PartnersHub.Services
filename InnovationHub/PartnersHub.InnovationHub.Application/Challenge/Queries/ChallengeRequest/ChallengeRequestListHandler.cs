using MediatR;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Application.Common.Paging;

namespace PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest
{
    public class ChallengeRequestListHandler : IRequestHandler<ChallengeRequestListQuery, PagingResult<ChallengeCardDTO>>
    {
        private readonly IChallengeRequestRepository _challengeRequestReadRepo;

        public ChallengeRequestListHandler(IChallengeRequestRepository challengeRequestReadRepo)
        {
            _challengeRequestReadRepo = challengeRequestReadRepo;
        }

        public async Task<PagingResult<ChallengeCardDTO>> Handle(
            ChallengeRequestListQuery request,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return await Task.FromCanceled<PagingResult<ChallengeCardDTO>>(cancellationToken);

            var data = await _challengeRequestReadRepo.ListAsync(request, cancellationToken);

            if (data == null || !data.Items.Any())
            {
                return new PagingResult<ChallengeCardDTO>
                {
                    Items = new List<ChallengeCardDTO>(),
                    TotalCount = 0,
                    Page = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            return new PagingResult<ChallengeCardDTO>
            {
                Items = data.Items,
                TotalCount = data.TotalCount,
                Page = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
