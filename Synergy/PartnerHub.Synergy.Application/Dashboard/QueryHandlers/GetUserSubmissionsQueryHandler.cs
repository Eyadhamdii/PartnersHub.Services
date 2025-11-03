using MediatR;
using PartnersHub.Synergy.Application.Dashboard.DTOs;
using PartnersHub.Synergy.Application.Dashboard.Queries;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Application.Dashboard.QueryHandlers;

public class GetUserSubmissionsQueryHandler : IRequestHandler<GetUserSubmissionsQuery, Result<List<UserOpportunitySubmissionDto>>>
{
    private readonly IOpportunityRepository _opportunityRepository;

    public GetUserSubmissionsQueryHandler(IOpportunityRepository opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<Result<List<UserOpportunitySubmissionDto>>> Handle(GetUserSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var statuses = MapStatusFilter(request.Status);

        // Use unified SearchOpportunitiesAsync method with companyId
        var (opportunities, totalCount) = await _opportunityRepository.SearchOpportunitiesAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            companyId: request.CompanyId,
            searchTerm: request.SearchTerm,
            statuses: statuses,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            asNoTracking: true);

        var dtos = opportunities.Select(o => new UserOpportunitySubmissionDto
        {
            Id = o.Id,
            Title = o.Title.Value,
            SubmissionDate = o.CreatedAt,
            CollaborationType = o.OpportunityType?.Name ?? "N/A",
            Sector = o.Sector?.Value ?? "N/A",
            Status = MapStatusToDisplay(o.Status)
        }).ToList();

        return Result<List<UserOpportunitySubmissionDto>>.Success(dtos);
    }

    private static List<OpportunityStatus>? MapStatusFilter(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        return status.ToLower() switch
        {
            "pending" => new List<OpportunityStatus> 
            { 
                OpportunityStatus.PendingApproval,
                OpportunityStatus.AdminApproved,
                OpportunityStatus.AssetManagerApproved
            },
            "published" => new List<OpportunityStatus> { OpportunityStatus.Published },
            "returned" => new List<OpportunityStatus> 
            { 
                OpportunityStatus.AdminRejected,
                OpportunityStatus.AssetManagerRejected
            },
            _ => null
        };
    }

    private static string MapStatusToDisplay(OpportunityStatus status)
    {
        return status switch
        {
            OpportunityStatus.PendingApproval => "Pending",
            OpportunityStatus.AdminApproved => "Pending",
            OpportunityStatus.AssetManagerApproved => "Pending",
            OpportunityStatus.Published => "Published",
            OpportunityStatus.AdminRejected => "Returned",
            OpportunityStatus.AssetManagerRejected => "Returned",
            _ => "Draft"
        };
    }
}

public class GetUserSuccessStoriesQueryHandler : IRequestHandler<GetUserSuccessStoriesQuery, Result<List<UserSuccessStorySubmissionDto>>>
{
    private readonly ISuccessStoryRepository _successStoryRepository;

    public GetUserSuccessStoriesQueryHandler(ISuccessStoryRepository successStoryRepository)
    {
        _successStoryRepository = successStoryRepository;
    }

    public async Task<Result<List<UserSuccessStorySubmissionDto>>> Handle(GetUserSuccessStoriesQuery request, CancellationToken cancellationToken)
    {
        var status = MapStatusFilter(request.Status);

        var (stories, totalCount) = await _successStoryRepository.GetPaginatedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            companyId: request.CompanyId,
            status: status,
            searchTerm: request.SearchTerm,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            asNoTracking: true);

        var dtos = stories.Select(s => new UserSuccessStorySubmissionDto
        {
            Id = s.Id,
            Title = s.Title.Value,
            SubmissionDate = s.CreatedAt,
            Type = MapSuccessStoryType(s.SuccessStoryTypeId),
            SubmissionStatus = MapStatusToDisplay(s.Status)
        }).ToList();

        return Result<List<UserSuccessStorySubmissionDto>>.Success(dtos);
    }

    private static SuccessStoryStatus? MapStatusFilter(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return null;

        return status.ToLower() switch
        {
            "pending" => SuccessStoryStatus.PendingReview,
            "published" => SuccessStoryStatus.Published,
            "returned" => SuccessStoryStatus.Rejected,
            _ => null
        };
    }

    private static string MapStatusToDisplay(SuccessStoryStatus status)
    {
        return status switch
        {
            SuccessStoryStatus.PendingReview => "Pending",
            SuccessStoryStatus.Approved => "Pending",
            SuccessStoryStatus.Published => "Published",
            SuccessStoryStatus.Rejected => "Returned",
            _ => "Draft"
        };
    }

    private static string MapSuccessStoryType(int typeId)
    {
        // TODO: Load from lookup table
        return typeId switch
        {
            1 => "Partnership",
            2 => "Collaboration",
            3 => "Joint Venture",
            _ => "Unknown"
        };
    }
}
