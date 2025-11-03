using MediatR;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;

public record ChallengeDetailsQuery : IRequest<ChallengeDetailsDTO>
{
    public Guid ChallengeId { get; set; }
}


