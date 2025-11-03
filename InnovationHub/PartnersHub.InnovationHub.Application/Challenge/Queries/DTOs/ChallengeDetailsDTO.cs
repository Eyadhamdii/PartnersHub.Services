using PartnersHub.InnovationHub.Domain.Aggregates;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using PartnersHub.InnovationHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;

public class ChallengeDetailsDTO
{
    public string Name { get; set; }
    public string SubmitterName { get;  set; }
    public string Description { get;  set; }
    public AssociatedProviderModel SourceCompany { get;  set; }
    public AssociatedSectorModel AssociatedSector { get;  set; }
    public PriorityLevel PriorityLevel { get;  set; }
    public DateTime DateAdded { get; set; }

    public ChallengeStatus ChallengeStatus { get;  set; }

    public bool? IsDraft { get;  set; }

    public List<ChallengeRequestAttachment> Attachments { get; set; }

    public List<Technology> Technologies { get; set; }

}


public class AssociatedProviderModel
{
    public Guid Id { get;  set; }
    public string Name { get;  set; }
}


public class AssociatedSectorModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string LogoUrl { get; set; }
}