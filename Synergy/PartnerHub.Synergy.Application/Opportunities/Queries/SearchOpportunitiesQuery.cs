using MediatR;
using PartnersHub.Synergy.Application.Opportunities.DTOs;
using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Application.Opportunities.Queries;

/// <summary>
/// Query for searching opportunities with pagination, search, and filtering
/// </summary>
public record SearchOpportunitiesQuery : IRequest<Result<SearchOpportunitiesResultDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? SearchTerm { get; init; }
    public List<Guid>? SectorIds { get; init; }
    public List<int>? OpportunityTypeIds { get; init; }
    public List<int>? ThematicAreaIds { get; init; }
    public List<int>? CollaborationRequirementIds { get; init; }
    public List<int>? ExpectedOutcomeIds { get; init; }
    public string? Status { get; init; }
    public string? SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
}
