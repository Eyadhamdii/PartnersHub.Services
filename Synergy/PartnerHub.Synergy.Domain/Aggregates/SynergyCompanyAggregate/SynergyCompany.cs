using PartnersHub.Synergy.Domain.Common;
using PartnersHub.Synergy.Domain.Events;
using PartnersHub.Synergy.Domain.ValueObjects;

namespace PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;

/// <summary>
/// Represents a company participating in the synergy program (Aggregate Root)
/// </summary>
public class SynergyCompany : AggregateRoot
{
    private readonly List<SynergyCompanySector> _sectors = new();

    public CompanyName Name { get; private set; } = null!;
    public string HeadquarterCountry { get; private set; } = null!;
    public string HeadquarterCity { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public string? LogoUrl { get; private set; }
    public RepresentativeInformation RepresentativeInformation { get; private set; } = null!;
    public IReadOnlyCollection<SynergyCompanySector> Sectors => _sectors.AsReadOnly();

    private SynergyCompany() { }

    private SynergyCompany(Guid companyId, CompanyName name, string headquarterCountry, string headquarterCity,
        Description description, string? logoUrl, RepresentativeInformation representativeInformation, Guid createdBy)
    {
        Id = companyId;
        Name = name;
        HeadquarterCountry = headquarterCountry;
        HeadquarterCity = headquarterCity;
        Description = description;
        LogoUrl = logoUrl;
        RepresentativeInformation = representativeInformation;

        MarkAsCreated(createdBy);
        AddDomainEvent(new CompanyCreatedEvent(Id, name.Value, createdBy));
    }

    public static Result<SynergyCompany> Create(Guid companyId, string name, string headquarterCountry,
        string headquarterCity, string description, string repName, string repPosition, string repEmail,
        string repPhone, Guid createdBy, string? logoUrl = null)
    {
        var nameResult = CompanyName.Create(name);
        if (nameResult.IsFailure)
            return Result<SynergyCompany>.Failure(nameResult.Error!);

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure)
            return Result<SynergyCompany>.Failure(descriptionResult.Error!);

        var repInfoResult = RepresentativeInformation.Create(repName, repPosition, repEmail, repPhone);
        if (repInfoResult.IsFailure)
            return Result<SynergyCompany>.Failure(repInfoResult.Error!);

        if (string.IsNullOrWhiteSpace(headquarterCountry))
            return Result<SynergyCompany>.Failure("Headquarter country is required");

        if (string.IsNullOrWhiteSpace(headquarterCity))
            return Result<SynergyCompany>.Failure("Headquarter city is required");

        var company = new SynergyCompany(companyId, nameResult.Value!, headquarterCountry.Trim(),
            headquarterCity.Trim(), descriptionResult.Value!, logoUrl?.Trim(), repInfoResult.Value!, createdBy);

        return Result<SynergyCompany>.Success(company);
    }

    public Result<bool> AddSector(Guid sectorId, string sectorName)
    {
        if (sectorId == Guid.Empty)
            return Result<bool>.Failure("Sector ID is required");

        if (string.IsNullOrWhiteSpace(sectorName))
            return Result<bool>.Failure("Sector name is required");

        if (_sectors.Any(s => s.SectorId == sectorId))
            return Result<bool>.Failure("Sector already assigned to this company");

        try
        {
            var companySector = new SynergyCompanySector(Id, sectorId, sectorName);
            _sectors.Add(companySector);
            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }

    public Result<bool> AddSectors(Dictionary<Guid, string> sectors)
    {
        if (sectors == null || sectors.Count == 0)
            return Result<bool>.Failure("At least one sector is required");

        try
        {
            foreach (var sector in sectors.Where(s => !_sectors.Any(cs => cs.SectorId == s.Key)))
            {
                var companySector = new SynergyCompanySector(Id, sector.Key, sector.Value);
                _sectors.Add(companySector);
            }
            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }

    public Result<bool> RemoveSector(Guid sectorId)
    {
        var sector = _sectors.FirstOrDefault(s => s.SectorId == sectorId);
        if (sector == null)
            return Result<bool>.Failure("Sector not assigned to this company");

        _sectors.Remove(sector);
        return Result<bool>.Success(true);
    }

    public Result<bool> UpdateInformation(string? name, string? headquarterCountry, string? headquarterCity,
        string? description, Guid userId)
    {
        bool hasChanges = false;

        if (!string.IsNullOrWhiteSpace(name) && name != Name.Value)
        {
            var nameResult = CompanyName.Create(name);
            if (nameResult.IsFailure)
                return Result<bool>.Failure(nameResult.Error!);

            Name = nameResult.Value!;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            var descResult = Description.Create(description);
            if (descResult.IsFailure)
                return Result<bool>.Failure(descResult.Error!);

            Description = descResult.Value!;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(headquarterCountry) && headquarterCountry != HeadquarterCountry)
        {
            HeadquarterCountry = headquarterCountry.Trim();
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(headquarterCity) && headquarterCity != HeadquarterCity)
        {
            HeadquarterCity = headquarterCity.Trim();
            hasChanges = true;
        }

        if (hasChanges)
        {
            MarkAsUpdated(userId);
            AddDomainEvent(new CompanyUpdatedEvent(Id, userId));
        }

        return Result<bool>.Success(true);
    }

    public Result<bool> SetLogoUrl(string logoUrl, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
            return Result<bool>.Failure("Logo URL cannot be empty");

        if (!Uri.IsWellFormedUriString(logoUrl, UriKind.Absolute))
            return Result<bool>.Failure("Logo URL must be a valid URL");

        LogoUrl = logoUrl.Trim();
        MarkAsUpdated(updatedBy);
        AddDomainEvent(new CompanyUpdatedEvent(Id, updatedBy));

        return Result<bool>.Success(true);
    }

    public Result<bool> RemoveLogoUrl(Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(LogoUrl))
            return Result<bool>.Failure("No logo to remove");

        LogoUrl = null;
        MarkAsUpdated(updatedBy);
        AddDomainEvent(new CompanyUpdatedEvent(Id, updatedBy));

        return Result<bool>.Success(true);
    }
}
