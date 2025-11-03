using Microsoft.EntityFrameworkCore;
using PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Application.Common.Paging;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Enums;



namespace PartnersHub.InnovationHub.Infrastructure.Presistence.Repositories;

public class ChallengeRequestRepository(InnovationHubDbContext dbContext) : IChallengeRequestRepository
{

    public async Task AddAsync(ChallengeRequest challenge, CancellationToken cancellationToken)
    {
        await dbContext.challengeRequests.AddAsync(challenge, cancellationToken);
    }

    public async Task<ChallengeRequest?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.challengeRequests.Include(c=>c.SourceCompany).Include(c => c.AssociatedSector).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ChallengeRequest>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.challengeRequests.Where(c => c.UserId == userId);
    }

    public async Task Update(ChallengeRequest challenge, CancellationToken cancellationToken)
    {
        dbContext.challengeRequests.Update(challenge);
    }

    public async Task Delete(ChallengeRequest challenge, CancellationToken cancellationToken)
    {
            dbContext.challengeRequests.Remove(challenge);
    }


    public async Task<PagingResult<ChallengeCardDTO>> ListAsync(ChallengeRequestListQuery filter, CancellationToken cancellationToken)
    {
   
        var challenges = dbContext.challengeRequests
            .Include(c => c.SourceCompany)
            .Include(c => c.AssociatedSector).AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            challenges = challenges
                .Where(c => c.Name.Contains(filter.Search) ||
                           c.SubmitterName.Contains(filter.Search) ||
                          (c.SourceCompany != null && c.SourceCompany.Name.Contains(filter.Search)) ||
                          (c.AssociatedSector != null && c.AssociatedSector.Name.Contains(filter.Search)));
        }

        if (filter.DevCoId != null && filter.DevCoId.Count > 0)
        {
            challenges = challenges
                .Where(c => filter.DevCoId.Contains(c.SourceCompanyId));
        }

        if (filter.SectorId != null && filter.SectorId.Count > 0)
        {
            challenges = challenges
                .Where(c => filter.SectorId.Contains(c.AssociatedSectorId));
        }

        if (filter.PriorityLevel != null && filter.PriorityLevel.Count > 0)
        {
            var ids = filter.PriorityLevel
                            .Select(pl => (PriorityLevel)Enum.Parse(typeof(PriorityLevel), pl))
                            .Select(pl => (int)pl)
                            .ToArray();

            challenges = challenges
                .Where(c => ids.Contains(c.PriorityLevelId));
        }
        if(filter.IsMyChallenge == true)
        {
            challenges = challenges.Where(c =>  c.IsArchived != true && (filter.UserId == null || c.UserId == filter.UserId) &&(c.ChallengeStatus == ChallengeStatus.Approved || c.ChallengeStatus == ChallengeStatus.Draft));

        }
        else
        {
            challenges = challenges.Where(c => c.ChallengeStatus == ChallengeStatus.Approved  && c.IsArchived != true);

        }

        var orderedChallenges = challenges.OrderByDescending(c => c.CreatedAt);

        var totalCount = await orderedChallenges.CountAsync(cancellationToken);
        var items = await orderedChallenges
            .Skip((filter.PageNumber -1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        var challengeCardDtos = items.Select(c => new ChallengeCardDTO
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            DevCoName = c.SourceCompany?.Name,
            SectorName = c.AssociatedSector?.Name,
            DevCoLogoUrl = "",
            PriorityLevel = (PriorityLevel)c.PriorityLevelId,
            CreatedAt = c.CreatedAt,
        }).ToList();

        return new PagingResult<ChallengeCardDTO>
        {
            Items = challengeCardDtos,
            TotalCount = totalCount,
            Page = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }


    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await dbContext.challengeRequests.AnyAsync(c => EF.Functions.Like(c.Name, $"%{name}%", "\\"), cancellationToken);
    }
}
