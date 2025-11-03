using MediatR;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Domain.Enums;

namespace PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;

public class ChallengeDetailsHandler : IRequestHandler<ChallengeDetailsQuery, ChallengeDetailsDTO?>
{
    private readonly IChallengeRequestRepository _repository;

    public ChallengeDetailsHandler(IChallengeRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChallengeDetailsDTO?> Handle(ChallengeDetailsQuery query, CancellationToken cancellationToken)
    {
        var request = await _repository.GetById(query.ChallengeId, cancellationToken);

        if (request == null)
            return null;

        return new ChallengeDetailsDTO
        {
            Name = request.Name,
            SubmitterName = request.SubmitterName,
            Description = request.Description,
            SourceCompany = request.SourceCompany != null
                ? new AssociatedProviderModel
                {
                    Id = request.SourceCompany.Id,
                    Name = request.SourceCompany.Name,
                }
                : null,
            AssociatedSector = request.AssociatedSector != null
                ? new AssociatedSectorModel
                {
                    Id = request.AssociatedSector.Id,
                    Name = request.AssociatedSector.Name,
                    LogoUrl = ""
                }
                : null,
            PriorityLevel = (PriorityLevel)request.PriorityLevelId,
            DateAdded = request.CreatedAt,
            ChallengeStatus = request.ChallengeStatus,
            IsDraft = request.IsDraft,
            Attachments = request.Attachments?.ToList(),
            Technologies = request.Technologies?.ToList()
        };
    }
}
