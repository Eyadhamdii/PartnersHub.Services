using PartnersHub.InnovationHub.Domain.Enums;

namespace PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;

public class ChallengeCardDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DevCoName { get; init; } = string.Empty;
    public string DevCoLogoUrl { get; init; } = string.Empty;
    public string SectorName { get; init; } = string.Empty;
    public PriorityLevel PriorityLevel { get; init; }
    public DateTime CreatedAt { get; init; }
}


