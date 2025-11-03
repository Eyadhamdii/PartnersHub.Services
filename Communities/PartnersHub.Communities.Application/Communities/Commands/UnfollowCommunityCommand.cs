
using MediatR;

namespace PartnersHub.Communities.Application.Communities.Commands;

public record UnfollowCommunityCommand : IRequest<bool>
{
    public Guid CommunityId { get; init; }
    public Guid UserId { get; init; }
}
