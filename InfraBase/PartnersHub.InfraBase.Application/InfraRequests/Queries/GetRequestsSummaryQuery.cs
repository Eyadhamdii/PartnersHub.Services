using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Query to get lightweight list of requests without navigation collections
/// Optimized for list views with pagination
/// </summary>
public record GetRequestsSummaryQuery : IRequest<(IEnumerable<RequestSummaryDto> Items, int TotalCount)> {
    public Guid? CompanyId { get; init; }
    public RequestStatus? Status { get; init; }
    public List<RequestStatus>? Statuses { get; init; }  // Support multiple statuses
    public int PageIndex { get; init; } = 0;
    public int PageSize { get; init; } = 10;
}
