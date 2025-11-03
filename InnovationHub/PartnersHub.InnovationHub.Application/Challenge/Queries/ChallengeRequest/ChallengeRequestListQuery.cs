using MediatR;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using PartnersHub.InnovationHub.Application.Common.Paging;

namespace PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;

public record ChallengeRequestListQuery(
    string? Search,                 
    List<Guid>? DevCoId,                  
    List<Guid>? SectorId,                 
    List<string>? PriorityLevel,
    bool? IsMyChallenge,
    Guid? UserId ,
int PageSize = 8 ,
    int PageNumber = 1
    ) : IRequest<PagingResult<ChallengeCardDTO>>;
