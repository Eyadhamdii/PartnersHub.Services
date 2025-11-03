using PartnersHub.InnovationHub.Domain.Common;

namespace PartnersHub.InnovationHub.Domain.Events
{
    public class TechnologyLinkedToChallengeEvent : DomainEvent
    {
        public TechnologyLinkedToChallengeEvent(Guid challengeRequestId, string technologyId, string approvedBy)
        {
            ChallengeRequestId = challengeRequestId;
            TechnologyId = technologyId;
            ApprovedBy = approvedBy;
        }

        public Guid ChallengeRequestId { get; set; }
        public string TechnologyId { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class AssociatedProviderLinkedToChallengeEvent : DomainEvent
    {
        public Guid ChallengeRequestId { get; }
        public string DevCompanyId { get; }
        public string ApprovedBy { get; }
        public AssociatedProviderLinkedToChallengeEvent(Guid challengeRequestId, string devCompanyId, string approvedBy)
        {
            ChallengeRequestId = challengeRequestId;
            DevCompanyId = devCompanyId;
            ApprovedBy = approvedBy;
        }

    }
    public class AssociatedSectorLinkedToChallengeEvent : DomainEvent
    {
        public Guid ChallengeRequestId { get; }
        public string SectorId { get; }
        public string ApprovedBy { get; }
        public AssociatedSectorLinkedToChallengeEvent(Guid challengeRequestId, string sectorId, string approvedBy)
        {
            ChallengeRequestId = challengeRequestId;
            SectorId = sectorId;
            ApprovedBy = approvedBy;
        }

    }
}
