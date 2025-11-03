using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;
using PartnersHub.InnovationHub.Domain.Events;
using System;

namespace PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest
{
    /// <summary>
    /// Represents a request to link technology to a challenge.
    /// </summary>
    public class ChallengeTechnologiesRequest : AggregateRoot
    {
        // Private constructor for EF or other ORM usage
        private ChallengeTechnologiesRequest() { }

        public Guid ChallengeRequestId { get; private set; }
        public Guid TechnologyId { get; private set; }
        public Technology LinkedTechnology { get; private set; }
        public string JustificationForLinking { get; private set; }
        public RequestStatus RequestStatus { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeTechnologiesRequest"/> class.
        /// </summary>
        /// <param name="challengeRequestId">The identifier for the challenge request.</param>
        /// <param name="technology">The technology to link.</param>
        /// <param name="justification">The justification for linking.</param>
        /// <param name="requestedBy">The user requesting the link.</param>
        public ChallengeTechnologiesRequest(
            Guid challengeRequestId,
            Technology technology,
            string justification,
            string requestedBy)
        {
            if (challengeRequestId == Guid.Empty)
                throw new ArgumentException("Challenge request ID cannot be empty.", nameof(challengeRequestId));
            if (technology == null)
                throw new ArgumentNullException(nameof(technology));
            if (string.IsNullOrWhiteSpace(justification))
                throw new ArgumentException("Justification cannot be null or empty.", nameof(justification));
            if (string.IsNullOrWhiteSpace(requestedBy))
                throw new ArgumentException("Requested by cannot be null or empty.", nameof(requestedBy));

            ChallengeRequestId = challengeRequestId;
            LinkedTechnology = technology;
            TechnologyId = technology.Id;
            JustificationForLinking = justification;
            CreatedBy = requestedBy;
            RequestStatus = RequestStatus.PendingReview;
            CreatedAt = DateTime.UtcNow;
        }

        public void Approve(string approvedBy)
        {
            if (RequestStatus == RequestStatus.Approved)
                throw new InvalidOperationException("Request is already approved.");

            RequestStatus = RequestStatus.Approved;
            AddDomainEvent(new TechnologyLinkedToChallengeEvent(ChallengeRequestId, LinkedTechnology.Id.ToString(), approvedBy));
        }

        public void Reject(string rejectedBy, string reason)
        {
            if (RequestStatus == RequestStatus.Rejected)
                throw new InvalidOperationException("Request is already rejected.");

            RequestStatus = RequestStatus.Rejected;
            // Potentially log the reason for rejection if needed
        }

        /// <summary>
        /// Creates a new <see cref="ChallengeTechnologiesRequest"/> instance.
        /// </summary>
        public static ChallengeTechnologiesRequest Create(
            Guid challengeRequestId,
            string technologyId,
            string technologyName,
            TechnologyStage stage,
            string sector,
            string justification,
            string requestedBy,
            Func<string, Technology?> findTechnologyById)
        {
            var existingTech = findTechnologyById(technologyId);

            if (existingTech != null)
            {
                existingTech.UpdateDetails(technologyName, stage, sector);
                return new ChallengeTechnologiesRequest(challengeRequestId, existingTech, justification, requestedBy);
            }

            var newTech = new Technology(technologyId, technologyName, stage, sector);
            return new ChallengeTechnologiesRequest(challengeRequestId, newTech, justification, requestedBy);
        }
    }
}