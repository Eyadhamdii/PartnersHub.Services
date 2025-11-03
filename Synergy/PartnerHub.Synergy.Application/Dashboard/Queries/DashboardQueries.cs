using MediatR;
using PartnersHub.Synergy.Application.Dashboard.DTOs;
using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Application.Dashboard.Queries;

/// <summary>
/// Get complete dashboard home page data (KPIs + recent items) in one query
/// </summary>
public class GetDashboardHomeQuery : IRequest<Result<DashboardHomeDto>>
{
    public Guid CompanyId { get; set; }
    public int? Year { get; set; } // Optional: filter by year (default: current year for YTD)
}

/// <summary>
/// Get user's submissions (opportunities and success stories) with filtering and pagination
/// </summary>
public class GetUserSubmissionsQuery : IRequest<Result<List<UserOpportunitySubmissionDto>>>
{
    public Guid CompanyId { get; set; }
    public string? Status { get; set; } // Pending, Published, Returned
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } // Title, SubmissionDate, Status
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Get user's success story submissions with filtering and pagination
/// </summary>
public class GetUserSuccessStoriesQuery : IRequest<Result<List<UserSuccessStorySubmissionDto>>>
{
    public Guid CompanyId { get; set; }
    public string? Status { get; set; } // Pending, Published, Returned
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } // Title, SubmissionDate, Status
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
