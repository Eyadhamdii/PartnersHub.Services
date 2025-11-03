using PartnersHub.InnovationHub.Domain.Common;

namespace PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;

public class ChallengeRequestAssociatedSector : Entity
{
    public string Name { get; private set; }

    private ChallengeRequestAssociatedSector() { }

    internal ChallengeRequestAssociatedSector(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Sector ID cannot be empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Sector name is required", nameof(name));

        Id = id;
        Name = name;
    }

    internal void UpdateDetails(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Sector name is required", nameof(name));

        Name = name;
    }
}
