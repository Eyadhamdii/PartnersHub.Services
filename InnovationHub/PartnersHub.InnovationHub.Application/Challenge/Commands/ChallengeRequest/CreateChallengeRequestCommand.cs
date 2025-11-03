using MediatR;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;


namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest
{
    public record CreateChallengeRequestCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public SourceCompanyDto SourceCompany { get; set; }
        public AssociatedSectorDto AssociatedSector { get; set; }
        public string SubmitterName { get; init; } = string.Empty;
        public PriorityLevel PriorityLevel { get; init; }
        public bool? IsDraft { get; init; }
        //public List<ChallengeRequestAttachment> Attachments { get; init; } = new List<ChallengeRequestAttachment>();
        //public List<Technology> Technologies { get; init; } = new List<Technology>();
    }

    public record SourceCompanyDto(Guid id, string name);
    public record AssociatedSectorDto(Guid id, string name);
}
