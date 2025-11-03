using Microsoft.EntityFrameworkCore;
using PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Application.Common.Paging;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Enums;



namespace PartnersHub.InnovationHub.Infrastructure.Presistence.Repositories;

public class AssociatedProviderRepository(InnovationHubDbContext dbContext) : IAssociatedProviderRepository
{


    public async Task<ChallengeRequestAssociatedProvider?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.associatedProviders.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }


}
