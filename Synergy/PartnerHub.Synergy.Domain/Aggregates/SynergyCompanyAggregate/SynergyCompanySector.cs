using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;

/// <summary>
/// Junction entity for Company-Sector many-to-many relationship
/// SectorId and SectorName reference sectors from ConfigurationHub microservice
/// </summary>
public class SynergyCompanySector : Entity
{
    public Guid CompanyId { get; private set; }
    public Guid SectorId { get; private set; }
    public string SectorName { get; private set; } = null!;
    public DateTime AssignedDate { get; private set; }

    private SynergyCompanySector() { }

    public SynergyCompanySector(Guid companyId, Guid sectorId, string sectorName)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required", nameof(companyId));

        if (sectorId == Guid.Empty)
            throw new ArgumentException("Sector ID is required", nameof(sectorId));

        if (string.IsNullOrWhiteSpace(sectorName))
            throw new ArgumentException("Sector name is required", nameof(sectorName));

        CompanyId = companyId;
        SectorId = sectorId;
        SectorName = sectorName.Trim();
        AssignedDate = DateTime.UtcNow;
    }
}
