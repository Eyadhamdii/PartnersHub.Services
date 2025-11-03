using MediatR;
using PartnersHub.InnovationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.LinkTechnologyToChallenge
{
    public record LinkAdditionalTechnologyToChallengeCommand : IRequest<Result>
    {
        [Required]
        public Guid ChallengeId { get; init; }
        [Required]
        public TechnologyDto LinkedTechnology { get; init; }
        [Required]
        public string JusificationForLinking { get; init; }
    }
}
