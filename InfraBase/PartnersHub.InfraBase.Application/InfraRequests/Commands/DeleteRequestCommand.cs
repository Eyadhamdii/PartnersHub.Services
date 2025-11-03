using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to delete a request
/// Only Draft requests can be deleted by the contributor who created them
/// </summary>
public record DeleteRequestCommand : IRequest<bool> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
}
