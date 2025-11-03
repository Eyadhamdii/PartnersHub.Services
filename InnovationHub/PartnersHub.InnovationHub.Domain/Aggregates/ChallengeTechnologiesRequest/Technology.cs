using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;

namespace PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;

public class Technology : Entity
{
    public string Name { get; private set; }
    public TechnologyStage TechnologyStage { get; private set; }
    public string Sector { get; private set; }

    private Technology() { }

    public Technology(string id, string name, TechnologyStage technologystage, string sector)
    {
        Id = new Guid(id);
        Name = name;
        TechnologyStage = technologystage;
        Sector = sector;
    }

    public void UpdateDetails(string name, TechnologyStage stage, string sector)
    {
        Name = name;
        TechnologyStage = stage;
        Sector = sector;
    }
}
