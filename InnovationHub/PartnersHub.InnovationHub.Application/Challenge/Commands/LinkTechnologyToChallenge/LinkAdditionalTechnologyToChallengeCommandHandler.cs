using MediatR;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using PartnersHub.InnovationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.LinkTechnologyToChallenge
{
    public class LinkAdditionalTechnologyToChallengeCommandHandler(
        ICurrentUserService _userService,
        ITechnologyRepository _technologyRepository,
        IChallengeTechnologiesRequestRepository _requestRepository,
        IUnitOfWork _unitOfWork) : IRequestHandler<LinkAdditionalTechnologyToChallengeCommand, Result>
    {
        public async Task<Result> Handle(LinkAdditionalTechnologyToChallengeCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!await _requestRepository.CheckExistedTechnologies(request.ChallengeId, cancellationToken))
            {
                return Result.Failure("There is no previous Technology Request For this Challenge Request");
            }

            // Parse technology ID from string to Guid
            if (!Guid.TryParse(request.LinkedTechnology.Id, out var technologyId))
            {
                return Result.Failure("Invalid technology ID format");
            }

            if (await _requestRepository.CheckDuplicateTechnologies(request.ChallengeId, technologyId, cancellationToken))
            {
                return Result.Failure("There is on going Link Request to the same Technology");
            }

            var existingTechnology = await _technologyRepository.GetByIdAsync(request.LinkedTechnology.Id, cancellationToken);

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
