using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;


namespace PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;

public interface IAssociatedProviderRepository
{
    Task<ChallengeRequestAssociatedProvider?> GetById(Guid id, CancellationToken cancellationToken);
}
