using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public record ArchiveChallengeRequestCommand : IRequest<bool>
{
    public Guid RequestId { get; set; } = Guid.Empty;
}
