using MediatR;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using PartnersHub.InnovationHub.Domain.Common;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.LinkTechnologyToChallenge
{
    public class LinkTechnologyToChallengeCommandHandler(
        ICurrentUserService _userService,
        ITechnologyRepository _technologyRepository,
        IChallengeTechnologiesRequestRepository _requestRepository,
        IUnitOfWork _unitOfWork) : IRequestHandler<LinkTechnologyToChallengeCommand, Result>
    {
        public async Task<Result> Handle(LinkTechnologyToChallengeCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var existingTechnology = await _technologyRepository.GetByIdAsync(
                request.LinkedTechnology.Id,
                cancellationToken);

            var challengeRequest = ChallengeTechnologiesRequest.Create(
                request.ChallengeId,
                request.LinkedTechnology.Id,
                request.LinkedTechnology.Name,
                request.LinkedTechnology.TechnologyStage,
                request.LinkedTechnology.Sector,
                request.JusificationForLinking,
                _userService.UserId,
                id => existingTechnology);

            await _requestRepository.AddAsync(challengeRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
