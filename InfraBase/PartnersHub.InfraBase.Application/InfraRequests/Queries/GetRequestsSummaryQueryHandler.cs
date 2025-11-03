using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Handler for getting lightweight list of requests
/// Returns summary DTOs without navigation collections for performance
/// </summary>
public class GetRequestsSummaryQueryHandler : IRequestHandler<GetRequestsSummaryQuery, (IEnumerable<RequestSummaryDto> Items, int TotalCount)> {
    private readonly IInfrabaseRequestRepository _repository;

    public GetRequestsSummaryQueryHandler(IInfrabaseRequestRepository repository) {
        _repository = repository;
    }

    public async Task<(IEnumerable<RequestSummaryDto> Items, int TotalCount)> Handle(
        GetRequestsSummaryQuery query,
        CancellationToken cancellationToken) {
        
        // Convert pageIndex (0-based) to pageNumber (1-based)
        var pageNumber = query.PageIndex + 1;

        // Determine which statuses to filter by
        List<RequestStatus>? statusesToFilter = null;
        if (query.Statuses != null && query.Statuses.Any()) {
            statusesToFilter = query.Statuses;
        } else if (query.Status.HasValue) {
            statusesToFilter = new List<RequestStatus> { query.Status.Value };
        }

        // Get lightweight data from repository
        var (requests, totalCount) = await _repository.GetSummaryPaginatedAsync(
            query.CompanyId,
            null, // Don't use single status parameter
            pageNumber,
            query.PageSize,
            cancellationToken);

        // Apply status filtering if provided
        var filteredRequests = requests;
        if (statusesToFilter != null && statusesToFilter.Any()) {
            filteredRequests = requests.Where(r => statusesToFilter.Contains(r.Status));
            totalCount = filteredRequests.Count(); // Recalculate total after filtering
            
            // Re-apply pagination after filtering
            filteredRequests = filteredRequests
                .Skip((pageNumber - 1) * query.PageSize)
                .Take(query.PageSize);
        }

        // Map to summary DTOs
        var summaryDtos = filteredRequests.Select(r => new RequestSummaryDto {
            Id = r.Id,
            RequestCode = r.RequestCode ?? string.Empty,
            ProjectName = r.ProjectName.Value,
            AssetTypeId = r.AssetTypeId,
            AssetTypeName = string.Empty, // Frontend will fetch from ConfigurationHub
            Status = r.Status,
            SubmittedAt = r.SubmittedAt,
            SubmittedBy = r.SubmittedBy ?? r.CreatedBy, // Fallback to CreatedBy if SubmittedBy is null
            TotalAmount = r.TotalRequestAmount,
            CreatedBy = r.CreatedBy,
            RequesterName = null,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            CompanyId = r.CompanyId,
            CompanyName = r.CompanyName
        }).ToList();

        return (summaryDtos, totalCount);
    }
}
