using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Get dashboard statistics for PC Admin - My Requests
/// </summary>
public record GetMyDashboardStatsQuery : IRequest<DashboardStatsDto> {
    public Guid UserId { get; init; }
}

/// <summary>
/// Get dashboard statistics for PC Admin - Team Requests
/// </summary>
public record GetTeamDashboardStatsQuery : IRequest<DashboardStatsDto> {
    public Guid CompanyId { get; init; }
}

/// <summary>
/// Get paginated list of user's own requests with filters
/// </summary>
public record GetMyRequestsQuery : IRequest<PaginatedResult<RequestSummaryDto>> {
    public Guid UserId { get; init; }
    public RequestStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}

/// <summary>
/// Get paginated list of team requests with filters (for PC Admin)
/// </summary>
public record GetTeamRequestsQuery : IRequest<PaginatedResult<RequestSummaryDto>> {
    public Guid CompanyId { get; init; }
    public RequestStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}

/// <summary>
/// Dashboard statistics DTO
/// </summary>
public record DashboardStatsDto {
    public int TotalRequests { get; init; }
    public int DraftRequests { get; init; }
    public int PendingRequests { get; init; }
    public int ApprovedRequests { get; init; }
    public int RejectedRequests { get; init; }
    
    public int TotalChangeRequests { get; init; }
    public int PendingChangeRequests { get; init; }
}

/// <summary>
/// Request summary DTO for list views
/// </summary>
public record RequestSummaryDto {
    public Guid Id { get; init; }
    public string RequestCode { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
    public Guid AssetTypeId { get; init; }
    public string AssetTypeName { get; init; } = string.Empty;  // Will be fetched from ConfigurationHub by frontend
    public RequestStatus Status { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public Guid? SubmittedBy { get; init; }  // Who submitted the request
    public decimal TotalAmount { get; init; }
    public Guid CreatedBy { get; init; }
    public string? RequesterName { get; init; }  // For team requests
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Guid? CompanyId { get; init; }  // For company filtering
    public string? CompanyName { get; init; }  // For display
}

/// <summary>
/// Paginated result wrapper
/// </summary>
public record PaginatedResult<T> {
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
