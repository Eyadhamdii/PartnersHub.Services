using MediatR;
using PartnersHub.InnovationHub.Domain.Common;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public record DeleteChallengeRequestDraftCommand : IRequest<Result<bool>>
{
    public Guid RequestId { get; set; } = Guid.Empty;
}
