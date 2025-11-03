using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to remove an item from a request
/// </summary>
public record RemoveRequestItemCommand : IRequest<bool>
{
    public Guid RequestId { get; init; }
    public Guid ItemId { get; init; }
    public Guid UserId { get; init; }  // Added for history tracking
}
