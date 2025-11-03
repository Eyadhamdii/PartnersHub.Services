namespace PartnersHub.Synergy.Application.SynergyCompany.Queries;

public class RegisteredCompanyCardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public List<CompanySectorDto> Sectors { get; set; } = new();
    public string HeadquarterCountry { get; set; } = null!;
    public string HeadquarterCity { get; set; } = null!;
    public int CollaborationsCount { get; set; }
    public string Description { get; set; } = null!;
}

public class CompanyDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public List<CompanySectorDto> Sectors { get; set; } = new();
    public string HeadquarterCountry { get; set; } = null!;
    public string HeadquarterCity { get; set; } = null!;
    public int CollaborationsCount { get; set; }
    public string Description { get; set; } = null!;
    public List<string> Services { get; set; } = new();
    public List<string> CollaborationFocus { get; set; } = new();
    public RepresentativeInfoDto Representative { get; set; } = null!;
    public List<OpportunityCollaborationDto> Collaborations { get; set; } = new();
    public List<SuccessStoryPreviewDto> SuccessStories { get; set; } = new();
}

public class CompanySectorDto
{
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = null!;
}

public class RepresentativeInfoDto
{
    public string Name { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}

public class OpportunityCollaborationDto
{
    public Guid OpportunityId { get; set; }
    public string Title { get; set; } = null!;
    public string CollaborationType { get; set; } = null!;
    public string Sector { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string PostedByCompany { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class SuccessStoryPreviewDto
{
    public Guid StoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;
    public List<CompanyNameLogoDto> PartnerCompanies { get; set; } = new();
    public string PostedBy { get; set; } = null!;
    public DateTime PostedDate { get; set; }
    public string Sector { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; } = null!;
}

public class CompanyNameLogoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}
