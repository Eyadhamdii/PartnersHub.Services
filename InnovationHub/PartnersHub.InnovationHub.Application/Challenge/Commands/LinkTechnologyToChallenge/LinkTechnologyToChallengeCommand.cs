using MediatR;
using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.LinkTechnologyToChallenge;

public record LinkTechnologyToChallengeCommand : IRequest<Result>
{
    [Required]
    public Guid ChallengeId { get; init; }
    
    [Required]
    public TechnologyDto LinkedTechnology { get; init; }
    
    [Required]
    public string JusificationForLinking { get; init; }
}

public record TechnologyDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public TechnologyStage TechnologyStage { get; init; }
    public string Sector { get; init; }
}
