using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;


namespace PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;

public interface IAssociatedSectorRepository
{
    Task<ChallengeRequestAssociatedSector?> GetById(Guid id, CancellationToken cancellationToken);
}
