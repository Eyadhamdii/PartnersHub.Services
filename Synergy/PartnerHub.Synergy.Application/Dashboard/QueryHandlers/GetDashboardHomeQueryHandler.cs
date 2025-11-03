using MediatR;
using PartnersHub.Synergy.Application.Dashboard.DTOs;
using PartnersHub.Synergy.Application.SynergyCompany.Queries;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Common;
using PartnersHub.Synergy.Application.Dashboard.Queries;

namespace PartnersHub.Synergy.Application.Dashboard.QueryHandlers;

/// <summary>
/// Single handler for complete dashboard home page - fetches all data in one go
/// </summary>
public class GetDashboardHomeQueryHandler : IRequestHandler<GetDashboardHomeQuery, Result<DashboardHomeDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;
    private readonly ISuccessStoryRepository _successStoryRepository;
    private readonly ISynergyCompanyRepository _companyRepository;

    public GetDashboardHomeQueryHandler(
        IOpportunityRepository opportunityRepository,
        ISuccessStoryRepository successStoryRepository,
        ISynergyCompanyRepository companyRepository)
    {
        _opportunityRepository = opportunityRepository;
        _successStoryRepository = successStoryRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Result<DashboardHomeDto>> Handle(GetDashboardHomeQuery request, CancellationToken cancellationToken)
    {
        var year = request.Year ?? DateTime.UtcNow.Year;
        var startOfYear = new DateTime(year, 1, 1);

        // Fetch data sequentially to avoid DbContext concurrency issues
        var kpis = await FetchKPIsAsync(request.CompanyId, startOfYear);
        var opportunities = await FetchRecentOpportunitiesAsync();
        var stories = await FetchRecentSuccessStoriesAsync();
        var companies = await FetchRecentCompaniesAsync();

        var dashboard = new DashboardHomeDto
        {
            KPIs = kpis,
            RecentOpportunities = opportunities,
            RecentSuccessStories = stories,
            RecentCompanies = companies
        };

        return Result<DashboardHomeDto>.Success(dashboard);
    }

    private async Task<DashboardKPIsDto> FetchKPIsAsync(Guid companyId, DateTime fromDate)
    {
        var totalCompanies = await _companyRepository.GetTotalCountAsync();
        var collaborationsCount = await _opportunityRepository.GetDistinctCollaboratedCompaniesCountAsync(companyId, fromDate);

        var totalPublishedOpp = await _opportunityRepository.GetTotalCountByStatusAsync(OpportunityStatus.Published, fromDate);
        var totalApprovedOpp = await _opportunityRepository.GetTotalCountByStatusAsync(OpportunityStatus.AssetManagerApproved, fromDate);
        var companyPublishedOpp = await _opportunityRepository.GetCountByCompanyAndStatusAsync(companyId, OpportunityStatus.Published, fromDate);
        var companyApprovedOpp = await _opportunityRepository.GetCountByCompanyAndStatusAsync(companyId, OpportunityStatus.AssetManagerApproved, fromDate);

        var totalStories = await _successStoryRepository.GetTotalCountByStatusAsync(SuccessStoryStatus.Published, fromDate);
        var companyStories = await _successStoryRepository.GetCountByCompanyAndStatusAsync(companyId, SuccessStoryStatus.Published, fromDate);

        return new DashboardKPIsDto
        {
            PortfolioCompanies = new PortfolioCompaniesKPI
            {
                TotalRegistered = totalCompanies,
                YourCollaborations = collaborationsCount
            },
            ActiveOpportunities = new ActiveOpportunitiesKPI
            {
                TotalAcrossSynergy = totalPublishedOpp + totalApprovedOpp,
                YourCompanyOpportunities = companyPublishedOpp + companyApprovedOpp
            },
            SuccessStories = new SuccessStoriesKPI
            {
                TotalPublished = totalStories,
                YourCompanyStories = companyStories
            }
        };
    }

    private async Task<List<RecentOpportunityCardDto>> FetchRecentOpportunitiesAsync()
    {
        var (opportunities, _) = await _opportunityRepository.GetPaginatedAsync(
            pageNumber: 1,
            pageSize: 4,
            status: OpportunityStatus.Published,
            sortBy: "CreatedAt",
            sortDescending: true,
            asNoTracking: true);

        return opportunities.Select(o => new RecentOpportunityCardDto
        {
            Id = o.Id,
            Title = o.Title.Value,
            PostedByCompany = new CompanyInfoDto
            {
                Id = o.CompanyId,
                Name = "Company Name", // TODO: Load from company
                LogoUrl = null
            },
            Description = o.Description.Value,
            CollaborationType = o.OpportunityType?.Name ?? "N/A",
            Sector = o.Sector?.Value ?? "N/A",
            StartDate = o.StartDate,
            EndDate = o.EndDate
        }).ToList();
    }

    private async Task<List<RecentSuccessStoryCardDto>> FetchRecentSuccessStoriesAsync()
    {
        var (stories, _) = await _successStoryRepository.GetPaginatedAsync(
            pageNumber: 1,
            pageSize: 4,
            status: SuccessStoryStatus.Published,
            sortBy: "CreatedAt",
            sortDescending: true,
            asNoTracking: true);

        return stories.Select(s => new RecentSuccessStoryCardDto
        {
            Id = s.Id,
            Title = s.Title.Value,
            SourceCompany = new CompanyInfoDto
            {
                Id = s.CompanyId,
                Name = "Source Company", // TODO: Load from company
                LogoUrl = null
            },
            PartnerCompanies = new List<CompanyInfoDto>(), // TODO: Load collaboration partners
            Description = s.Description.Value,
            Type = MapSuccessStoryType(s.SuccessStoryTypeId),
            StartDate = s.StartDate,
            EndDate = s.EndDate
        }).ToList();
    }

    private async Task<List<RecentCompanyCardDto>> FetchRecentCompaniesAsync()
    {
        var companies = await _companyRepository.GetRecentAsync(4);

        return companies.Select(c => new RecentCompanyCardDto
        {
            Id = c.Id,
            Name = c.Name.Value,
            LogoUrl = c.LogoUrl,
            Description = c.Description?.Value ?? string.Empty,
            Sectors = c.Sectors.Select(s => new CompanySectorDto
            {
                SectorId = s.SectorId,
                SectorName = s.SectorName
            }).ToList(),
            HeadquarterCountry = c.HeadquarterCountry,
            HeadquarterCity = c.HeadquarterCity,
            RegisteredDate = c.CreatedAt
        }).ToList();
    }

    private static string MapSuccessStoryType(int typeId)
    {
        // TODO: Load from lookup table
        return typeId switch
        {
            1 => "Partnership",
            2 => "Collaboration",
            3 => "Joint Venture",
            _ => "Unknown"
        };
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength - 3) + "...";
    }
}
