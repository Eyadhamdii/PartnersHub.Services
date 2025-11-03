using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

public record GetRequestByIdQuery : IRequest<RequestDto?> {
    public Guid RequestId { get; init; }
}