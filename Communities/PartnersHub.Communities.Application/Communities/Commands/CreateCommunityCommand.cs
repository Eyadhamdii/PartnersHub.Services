using MediatR;
using PartnersHub.Communities.Application.Common;

namespace PartnersHub.Communities.Application.Communities.Commands;

public record CreateCommunityCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
}