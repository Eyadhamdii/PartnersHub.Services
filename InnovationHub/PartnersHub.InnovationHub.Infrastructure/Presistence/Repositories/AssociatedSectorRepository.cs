using Microsoft.EntityFrameworkCore;
using PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Application.Common.Paging;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Enums;



namespace PartnersHub.InnovationHub.Infrastructure.Presistence.Repositories;

public class AssociatedSectorRepository(InnovationHubDbContext dbContext) : IAssociatedSectorRepository
{


    public async Task<ChallengeRequestAssociatedSector?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.associatedSectors.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }


}
