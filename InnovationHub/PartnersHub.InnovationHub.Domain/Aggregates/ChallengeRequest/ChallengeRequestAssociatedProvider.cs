using PartnersHub.InnovationHub.Domain.Common;

namespace PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;

public class ChallengeRequestAssociatedProvider : Entity
{
    public string Name { get; private set; }

    private ChallengeRequestAssociatedProvider() { }

    internal ChallengeRequestAssociatedProvider(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Provider ID cannot be empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Provider name is required", nameof(name));

        Id = id;
        Name = name;
    }

    internal void UpdateDetails(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Provider name is required", nameof(name));

        Name = name;
    }
}
