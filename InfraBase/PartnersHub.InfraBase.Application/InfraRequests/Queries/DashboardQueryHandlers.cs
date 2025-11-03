using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Handler for getting My Dashboard statistics
/// </summary>
public class GetMyDashboardStatsQueryHandler : IRequestHandler<GetMyDashboardStatsQuery, DashboardStatsDto> {
    private readonly IInfrabaseRequestRepository _requestRepository;

    public GetMyDashboardStatsQueryHandler(IInfrabaseRequestRepository requestRepository) {
        _requestRepository = requestRepository;
    }

    public async Task<DashboardStatsDto> Handle(GetMyDashboardStatsQuery query, CancellationToken cancellationToken) {
        var userRequests = await _requestRepository.GetByCreatedByAsync(query.UserId, cancellationToken);

        var stats = new DashboardStatsDto {
            TotalRequests = userRequests.Count(),
            DraftRequests = userRequests.Count(r => r.Status == RequestStatus.Draft),
            PendingRequests = userRequests.Count(r => 
                r.Status == RequestStatus.PendingPcAdminApproval || 
                r.Status == RequestStatus.PendingPcInfrabaseApproval),
            ApprovedRequests = userRequests.Count(r => r.Status == RequestStatus.InfrabaseApproved),
            RejectedRequests = userRequests.Count(r => 
                r.Status == RequestStatus.RejectedByPcAdmin || 
                r.Status == RequestStatus.InfrabaseRejected),
            
            // ChangeRequested is a status within InfraRequest, not a separate entity
            TotalChangeRequests = userRequests.Count(r => r.Status == RequestStatus.ChangeRequested),
            PendingChangeRequests = userRequests.Count(r => r.Status == RequestStatus.ChangeRequested)
        };

        return stats;
    }
}

/// <summary>
/// Handler for getting Team Dashboard statistics
/// </summary>
public class GetTeamDashboardStatsQueryHandler : IRequestHandler<GetTeamDashboardStatsQuery, DashboardStatsDto> {
    private readonly IInfrabaseRequestRepository _requestRepository;

    public GetTeamDashboardStatsQueryHandler(IInfrabaseRequestRepository requestRepository) {
        _requestRepository = requestRepository;
    }

    public async Task<DashboardStatsDto> Handle(GetTeamDashboardStatsQuery query, CancellationToken cancellationToken) {
        // Get all requests from the company (team requests)
        var allRequests = await _requestRepository.GetAllAsync(cancellationToken);
        var teamRequests = allRequests.Where(r => r.CompanyId == query.CompanyId).ToList();

        var stats = new DashboardStatsDto {
            TotalRequests = teamRequests.Count,
            DraftRequests = teamRequests.Count(r => r.Status == RequestStatus.Draft),
            PendingRequests = teamRequests.Count(r => 
                r.Status == RequestStatus.PendingPcAdminApproval || 
                r.Status == RequestStatus.PendingPcInfrabaseApproval),
            ApprovedRequests = teamRequests.Count(r => 
                r.Status == RequestStatus.InfrabaseApproved),
            RejectedRequests = teamRequests.Count(r => 
                r.Status == RequestStatus.RejectedByPcAdmin || 
                r.Status == RequestStatus.InfrabaseRejected),
            
            // ChangeRequested is a status within InfraRequest
            TotalChangeRequests = teamRequests.Count(r => r.Status == RequestStatus.ChangeRequested),
            PendingChangeRequests = teamRequests.Count(r => r.Status == RequestStatus.ChangeRequested)
        };

        return stats;
    }
}

/// <summary>
/// Handler for getting My Requests with pagination and filtering
/// </summary>
public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, PaginatedResult<RequestSummaryDto>> {
    private readonly IInfrabaseRequestRepository _requestRepository;

    public GetMyRequestsQueryHandler(IInfrabaseRequestRepository requestRepository) {
        _requestRepository = requestRepository;
    }

    public async Task<PaginatedResult<RequestSummaryDto>> Handle(GetMyRequestsQuery query, CancellationToken cancellationToken) {
        // Get user's requests
        var userRequests = await _requestRepository.GetByCreatedByAsync(query.UserId, cancellationToken);

        // Apply status filter
        if (query.Status.HasValue) {
            userRequests = userRequests.Where(r => r.Status == query.Status.Value);
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(query.SearchTerm)) {
            var searchTerm = query.SearchTerm.ToLower();
            userRequests = userRequests.Where(r =>
                (r.RequestCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                r.ProjectName.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Convert to DTOs
        var requestDtos = userRequests.Select(r => new RequestSummaryDto {
            Id = r.Id,
            RequestCode = r.RequestCode ?? string.Empty,
            ProjectName = r.ProjectName.Value,
            AssetTypeId = r.AssetTypeId,
            AssetTypeName = string.Empty, // Frontend will fetch from ConfigurationHub
            Status = r.Status,
            SubmittedAt = r.SubmittedAt,
            SubmittedBy = r.SubmittedBy,
            TotalAmount = r.TotalRequestAmount,
            CreatedBy = r.CreatedBy,
            RequesterName = null,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            CompanyId = r.CompanyId,
            CompanyName = r.CompanyName
        }).ToList();

        // Apply sorting
        requestDtos = ApplySorting(requestDtos, query.SortBy, query.SortDescending);

        // Get total count before pagination
        var totalCount = requestDtos.Count;

        // Apply pagination
        var paginatedItems = requestDtos
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResult<RequestSummaryDto> {
            Items = paginatedItems,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    private List<RequestSummaryDto> ApplySorting(List<RequestSummaryDto> items, string? sortBy, bool descending) {
        if (string.IsNullOrWhiteSpace(sortBy)) {
            // Default sort by creation date descending
            return descending 
                ? items.OrderByDescending(r => r.CreatedAt).ToList()
                : items.OrderBy(r => r.CreatedAt).ToList();
        }

        return sortBy.ToLower() switch {
            "requestcode" => descending 
                ? items.OrderByDescending(r => r.RequestCode).ToList()
                : items.OrderBy(r => r.RequestCode).ToList(),
            "projectname" => descending 
                ? items.OrderByDescending(r => r.ProjectName).ToList()
                : items.OrderBy(r => r.ProjectName).ToList(),
            "status" => descending 
                ? items.OrderByDescending(r => r.Status).ToList()
                : items.OrderBy(r => r.Status).ToList(),
            "submittedat" => descending 
                ? items.OrderByDescending(r => r.SubmittedAt).ToList()
                : items.OrderBy(r => r.SubmittedAt).ToList(),
            "totalamount" => descending 
                ? items.OrderByDescending(r => r.TotalAmount).ToList()
                : items.OrderBy(r => r.TotalAmount).ToList(),
            "createdat" => descending 
                ? items.OrderByDescending(r => r.CreatedAt).ToList()
                : items.OrderBy(r => r.CreatedAt).ToList(),
            _ => descending 
                ? items.OrderByDescending(r => r.CreatedAt).ToList()
                : items.OrderBy(r => r.CreatedAt).ToList()
        };
    }
}

/// <summary>
/// Handler for getting Team Requests with pagination and filtering
/// </summary>
public class GetTeamRequestsQueryHandler : IRequestHandler<GetTeamRequestsQuery, PaginatedResult<RequestSummaryDto>> {
    private readonly IInfrabaseRequestRepository _requestRepository;

    public GetTeamRequestsQueryHandler(IInfrabaseRequestRepository requestRepository) {
        _requestRepository = requestRepository;
    }

    public async Task<PaginatedResult<RequestSummaryDto>> Handle(GetTeamRequestsQuery query, CancellationToken cancellationToken) {
        // Get all requests
        var allRequests = await _requestRepository.GetAllAsync(cancellationToken);
        
        // Filter by company (team requests)
        var teamRequests = allRequests.Where(r => r.CompanyId == query.CompanyId);

        // Team view includes all requests (including drafts) for full visibility
        // If you want to exclude drafts, uncomment the line below:
        // teamRequests = teamRequests.Where(r => r.Status != RequestStatus.Draft);

        // Apply status filter
        if (query.Status.HasValue) {
            teamRequests = teamRequests.Where(r => r.Status == query.Status.Value);
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(query.SearchTerm)) {
            var searchTerm = query.SearchTerm.ToLower();
            teamRequests = teamRequests.Where(r =>
                (r.RequestCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                r.ProjectName.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Convert to DTOs
        var requestDtos = teamRequests.Select(r => new RequestSummaryDto {
            Id = r.Id,
            RequestCode = r.RequestCode ?? string.Empty,
            ProjectName = r.ProjectName.Value,
            AssetTypeId = r.AssetTypeId,
            AssetTypeName = string.Empty, // Frontend will fetch from ConfigurationHub
            Status = r.Status,
            SubmittedAt = r.SubmittedAt,
            SubmittedBy = r.SubmittedBy,
            TotalAmount = r.TotalRequestAmount,
            CreatedBy = r.CreatedBy,
            RequesterName = null, // TODO: Fetch from user service or include in request
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            CompanyId = r.CompanyId,
            CompanyName = r.CompanyName
        }).ToList();

        // Apply sorting
        requestDtos = ApplySorting(requestDtos, query.SortBy, query.SortDescending);

        // Get total count before pagination
        var totalCount = requestDtos.Count;

        // Apply pagination
        var paginatedItems = requestDtos
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new PaginatedResult<RequestSummaryDto> {
            Items = paginatedItems,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    private List<RequestSummaryDto> ApplySorting(List<RequestSummaryDto> items, string? sortBy, bool descending) {
        if (string.IsNullOrWhiteSpace(sortBy)) {
            return descending 
                ? items.OrderByDescending(r => r.CreatedAt).ToList()
                : items.OrderBy(r => r.CreatedAt).ToList();
        }

        return sortBy.ToLower() switch {
            "requestcode" => descending 
                ? items.OrderByDescending(r => r.RequestCode).ToList()
                : items.OrderBy(r => r.RequestCode).ToList(),
            "projectname" => descending 
                ? items.OrderByDescending(r => r.ProjectName).ToList()
                : items.OrderBy(r => r.ProjectName).ToList(),
            "status" => descending 
                ? items.OrderByDescending(r => r.Status).ToList()
                : items.OrderBy(r => r.Status).ToList(),
            "submittedat" => descending 
                ? items.OrderByDescending(r => r.SubmittedAt).ToList()
                : items.OrderBy(r => r.SubmittedAt).ToList(),
            "totalamount" => descending 
                ? items.OrderByDescending(r => r.TotalAmount).ToList()
                : items.OrderBy(r => r.TotalAmount).ToList(),
            "requestername" => descending 
                ? items.OrderByDescending(r => r.RequesterName).ToList()
                : items.OrderBy(r => r.RequesterName).ToList(),
            "createdat" => descending 
                ? items.OrderByDescending(r => r.CreatedAt).ToList()
                : items.OrderBy(r => r.CreatedAt).ToList(),
            _ => descending 
                ? items.OrderByDescending(r => r.CreatedAt).ToList()
                : items.OrderBy(r => r.CreatedAt).ToList()
        };
    }
}
