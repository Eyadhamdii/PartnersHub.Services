using MediatR;
using PartnersHub.InfraBase.Application.Common.Models;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Gets all requests with optional status filter and pagination
/// </summary>
public record GetAllRequestsQuery : IRequest<PaginatedList<RequestDto>> {
    public RequestStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// Gets requests by user ID with pagination
/// </summary>
public record GetRequestsByUserQuery : IRequest<PaginatedList<RequestDto>> {
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
