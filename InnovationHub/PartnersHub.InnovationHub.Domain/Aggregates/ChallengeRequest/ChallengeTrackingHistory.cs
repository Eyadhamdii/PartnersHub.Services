using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;


namespace PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest
{
    public class ChallengeTrackingHistory : Entity
    {
        public Guid ChallengeId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Description { get; private set; }
        public ChallengeStatus Status { get; private set; }
        public string? ChangedBy { get; private set; }

        private ChallengeTrackingHistory() { }

        public ChallengeTrackingHistory(string description, ChallengeStatus status, string? changedBy = null)
        {
            Timestamp = DateTime.UtcNow;
            Description = description;
            Status = status;
            ChangedBy = changedBy;
        }
    }
}
