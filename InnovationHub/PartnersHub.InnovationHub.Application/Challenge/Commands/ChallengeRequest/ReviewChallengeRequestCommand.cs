using MediatR;
using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;


namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public record ReviewChallengeRequestCommand : IRequest<Result<bool>>
{

    public Guid ChallengeRequestId { get; init; }

    public string Comment { get; init; }

    public ChallengeStatus Status { get; init; }
}
