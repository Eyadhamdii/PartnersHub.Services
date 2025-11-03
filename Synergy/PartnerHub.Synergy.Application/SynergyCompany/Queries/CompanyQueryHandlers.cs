using MediatR;
using PartnersHub.Synergy.Application.Interfaces.Repository;

namespace PartnersHub.Synergy.Application.SynergyCompany.Queries;

public class GetRegisteredCompaniesQueryHandler : IRequestHandler<GetRegisteredCompaniesQuery, RegisteredCompaniesResultDto>
{
    private readonly ISynergyCompanyRepository _companyRepository;
    private readonly IOpportunityRepository _opportunityRepository;

    public GetRegisteredCompaniesQueryHandler(
        ISynergyCompanyRepository companyRepository,
        IOpportunityRepository opportunityRepository)
    {
        _companyRepository = companyRepository;
        _opportunityRepository = opportunityRepository;
    }

    public async Task<RegisteredCompaniesResultDto> Handle(GetRegisteredCompaniesQuery request, CancellationToken cancellationToken)
    {
        var allCompanies = await _companyRepository.GetAllAsync(asNoTracking: true);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLowerInvariant();
            allCompanies = allCompanies.Where(c => 
                c.Name.Value.ToLowerInvariant().Contains(searchLower) ||
                c.Description.Value.ToLowerInvariant().Contains(searchLower))
                .ToList();
        }

        if (request.SectorIds?.Any() == true)
            allCompanies = allCompanies.Where(c => c.Sectors.Any(s => request.SectorIds.Contains(s.SectorId))).ToList();

        if (request.Countries?.Any() == true)
            allCompanies = allCompanies.Where(c => request.Countries.Contains(c.HeadquarterCountry)).ToList();

        if (request.Cities?.Any() == true)
            allCompanies = allCompanies.Where(c => request.Cities.Contains(c.HeadquarterCity)).ToList();

        var totalCount = allCompanies.Count();

        var paginatedCompanies = allCompanies
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var companyDtos = new List<RegisteredCompanyCardDto>();
        foreach (var company in paginatedCompanies)
        {
            var collaborationsCount = await _opportunityRepository.GetDistinctCollaboratedCompaniesCountAsync(company.Id);
            
            companyDtos.Add(new RegisteredCompanyCardDto
            {
                Id = company.Id,
                Name = company.Name.Value,
                LogoUrl = company.LogoUrl,
                Sectors = company.Sectors.Select(s => new CompanySectorDto 
                { 
                    SectorId = s.SectorId, 
                    SectorName = s.SectorName 
                }).ToList(),
                HeadquarterCountry = company.HeadquarterCountry,
                HeadquarterCity = company.HeadquarterCity,
                CollaborationsCount = collaborationsCount,
                Description = company.Description.Value
            });
        }

        var allCompaniesForFilters = await _companyRepository.GetAllAsync(asNoTracking: true);

        return new RegisteredCompaniesResultDto
        {
            Companies = companyDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            AvailableCountries = allCompaniesForFilters.Select(c => c.HeadquarterCountry).Distinct().OrderBy(c => c).ToList(),
            AvailableCities = allCompaniesForFilters.Select(c => c.HeadquarterCity).Distinct().OrderBy(c => c).ToList()
        };
    }
}

public class GetCompanyDetailsQueryHandler : IRequestHandler<GetCompanyDetailsQuery, CompanyDetailsDto?>
{
    private readonly ISynergyCompanyRepository _companyRepository;
    private readonly IOpportunityRepository _opportunityRepository;
    private readonly ISuccessStoryRepository _successStoryRepository;

    public GetCompanyDetailsQueryHandler(
        ISynergyCompanyRepository companyRepository,
        IOpportunityRepository opportunityRepository,
        ISuccessStoryRepository successStoryRepository)
    {
        _companyRepository = companyRepository;
        _opportunityRepository = opportunityRepository;
        _successStoryRepository = successStoryRepository;
    }

    public async Task<CompanyDetailsDto?> Handle(GetCompanyDetailsQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId, asNoTracking: true);
        if (company == null)
            return null;

        var collaborationsCount = await _opportunityRepository.GetDistinctCollaboratedCompaniesCountAsync(company.Id);
        var opportunities = await _opportunityRepository.GetByPublishingCompanyId(company.Id);
        
        var collaborationDtos = opportunities.Select(o => new OpportunityCollaborationDto
        {
            OpportunityId = o.Id,
            Title = o.Title.Value,
            CollaborationType = o.OpportunityType?.Name ?? "N/A",
            Sector = o.Sector.Value,
            StartDate = o.StartDate,
            EndDate = o.EndDate,
            PostedByCompany = company.Name.Value,
            Description = o.Description.Value
        }).ToList();

        var successStories = await _successStoryRepository.GetByCompanyIdAsync(company.Id);
        
        var storyDtos = successStories.Select(s => new SuccessStoryPreviewDto
        {
            StoryId = s.Id,
            Title = s.Title.Value,
            Type = "Collaboration",
            PartnerCompanies = new List<CompanyNameLogoDto>(),
            PostedBy = company.Name.Value,
            PostedDate = s.CreatedAt,
            Sector = "N/A",
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            Description = s.Description.Value
        }).ToList();

        return new CompanyDetailsDto
        {
            Id = company.Id,
            Name = company.Name.Value,
            LogoUrl = company.LogoUrl,
            Sectors = company.Sectors.Select(s => new CompanySectorDto 
            { 
                SectorId = s.SectorId, 
                SectorName = s.SectorName 
            }).ToList(),
            HeadquarterCountry = company.HeadquarterCountry,
            HeadquarterCity = company.HeadquarterCity,
            CollaborationsCount = collaborationsCount,
            Description = company.Description.Value,
            Services = new List<string>(),
            CollaborationFocus = new List<string>(),
            Representative = new RepresentativeInfoDto
            {
                Name = company.RepresentativeInformation.Name,
                Position = company.RepresentativeInformation.Position,
                Email = company.RepresentativeInformation.Email,
                Phone = company.RepresentativeInformation.Phone
            },
            Collaborations = collaborationDtos,
            SuccessStories = storyDtos
        };
    }
}
