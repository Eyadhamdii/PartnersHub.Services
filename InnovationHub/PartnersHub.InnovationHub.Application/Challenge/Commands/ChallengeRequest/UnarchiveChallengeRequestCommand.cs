using MediatR;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public record UnarchiveChallengeRequestCommand : IRequest<bool>
{
    public Guid RequestId { get; set; } = Guid.Empty;
}
