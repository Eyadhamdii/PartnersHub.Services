using MediatR;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Application.Opportunities.DTOs;
using PartnersHub.Synergy.Application.Opportunities.Queries;
using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Application.Opportunities.QueryHandlers;

public class SearchOpportunitiesQueryHandler : IRequestHandler<SearchOpportunitiesQuery, Result<SearchOpportunitiesResultDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;
    private readonly ISynergyCompanyRepository _companyRepository;
    private readonly IOpportunityTypeRepository _opportunityTypeRepository;
    private readonly IThematicAreaRepository _thematicAreaRepository;
    private readonly ICollaborationRequirementRepository _collaborationRequirementRepository;
    private readonly IExpectedOutcomesRepository _expectedOutcomesRepository;

    public SearchOpportunitiesQueryHandler(
        IOpportunityRepository opportunityRepository,
        ISynergyCompanyRepository companyRepository,
        IOpportunityTypeRepository opportunityTypeRepository,
        IThematicAreaRepository thematicAreaRepository,
        ICollaborationRequirementRepository collaborationRequirementRepository,
        IExpectedOutcomesRepository expectedOutcomesRepository)
    {
        _opportunityRepository = opportunityRepository;
        _companyRepository = companyRepository;
        _opportunityTypeRepository = opportunityTypeRepository;
        _thematicAreaRepository = thematicAreaRepository;
        _collaborationRequirementRepository = collaborationRequirementRepository;
        _expectedOutcomesRepository = expectedOutcomesRepository;
    }

    public async Task<Result<SearchOpportunitiesResultDto>> Handle(SearchOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        // Parse status if provided
        List<OpportunityStatus>? statuses = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<OpportunityStatus>(request.Status, true, out var status))
            {
                statuses = new List<OpportunityStatus> { status };
            }
        }

        // Call repository with all filters - filtering happens at database level
        var (opportunities, totalCount) = await _opportunityRepository.SearchOpportunitiesAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            searchTerm: request.SearchTerm,
            sectorIds: request.SectorIds,
            opportunityTypeIds: request.OpportunityTypeIds,
            thematicAreaIds: request.ThematicAreaIds,
            collaborationRequirementIds: request.CollaborationRequirementIds,
            expectedOutcomeIds: request.ExpectedOutcomeIds,
            statuses: statuses,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            asNoTracking: true);

        // Get company information for the opportunities
        var companyIds = opportunities.Select(o => o.CompanyId).Distinct().ToList();
        var companies = await _companyRepository.GetByIdsAsync(companyIds, asNoTracking: true);
        var companyDict = companies.ToDictionary(c => c.Id);

        // Map to DTOs
        var opportunityDtos = opportunities.Select(o =>
        {
            var company = companyDict.GetValueOrDefault(o.CompanyId);
            
            return new OpportunitySearchCardDto
            {
                Id = o.Id,
                Title = o.Title.Value,
                Description = o.Description?.Value ?? string.Empty,
                Status = o.Status.ToString(),
                CompanyId = o.CompanyId,
                CompanyName = company?.Name.Value ?? "Unknown Company",
                CompanyLogoUrl = company?.LogoUrl,
                OpportunityTypeId = o.OpportunityTypeId,
                OpportunityTypeName = o.OpportunityType?.Name ?? "N/A",
                ThematicAreaId = o.ThematicAreaId,
                ThematicAreaName = o.ThematicArea?.Name ?? "N/A",
                SectorId = o.Sector.Id,
                SectorName = o.Sector.Value,
                CollaborationRequirements = o.CollaborationRequirements.Select(cr => cr.Name).ToList(),
                ExpectedOutcomes = o.ExpectedOutcomes.Select(eo => eo.Name).ToList(),
                CollaboratedCompaniesCount = o.CollaboratedCompanies.Count,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                CreatedAt = o.CreatedAt
            };
        }).ToList();

        // Build available filters (these need separate queries to get all available options)
        var result = new SearchOpportunitiesResultDto
        {
            Opportunities = opportunityDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            AvailableSectors = await BuildSectorFilters(request),
            AvailableOpportunityTypes = await BuildOpportunityTypeFilters(request),
            AvailableThematicAreas = await BuildThematicAreaFilters(request),
            AvailableCollaborationRequirements = await BuildCollaborationRequirementFilters(request),
            AvailableExpectedOutcomes = await BuildExpectedOutcomeFilters(request),
            AvailableStatuses = new List<string> { "Published", "AssetManagerApproved" }
        };

        return Result<SearchOpportunitiesResultDto>.Success(result);
    }

    private async Task<List<FilterOptionDto>> BuildSectorFilters(SearchOpportunitiesQuery request)
    {
        // Get all published opportunities to build filter counts
        var (allOpportunities, _) = await _opportunityRepository.SearchOpportunitiesAsync(
            pageNumber: 1,
            pageSize: int.MaxValue,
            searchTerm: request.SearchTerm,
            opportunityTypeIds: request.OpportunityTypeIds,
            thematicAreaIds: request.ThematicAreaIds,
            collaborationRequirementIds: request.CollaborationRequirementIds,
            expectedOutcomeIds: request.ExpectedOutcomeIds,
            statuses: null, // Get all statuses for filter counts
            asNoTracking: true);

        return allOpportunities
            .GroupBy(o => new { o.Sector.Id, o.Sector.Value })
            .Select(g => new FilterOptionDto
            {
                Id = g.Key.Id.ToString(),
                Name = g.Key.Value,
                Count = g.Count()
            })
            .OrderBy(f => f.Name)
            .ToList();
    }

    private async Task<List<FilterOptionDto>> BuildOpportunityTypeFilters(SearchOpportunitiesQuery request)
    {
        var types = await _opportunityTypeRepository.GetAllAsync();
        
        var (allOpportunities, _) = await _opportunityRepository.SearchOpportunitiesAsync(
            pageNumber: 1,
            pageSize: int.MaxValue,
            searchTerm: request.SearchTerm,
            sectorIds: request.SectorIds,
            thematicAreaIds: request.ThematicAreaIds,
            collaborationRequirementIds: request.CollaborationRequirementIds,
            expectedOutcomeIds: request.ExpectedOutcomeIds,
            asNoTracking: true);

        var typeCounts = allOpportunities
            .GroupBy(o => o.OpportunityTypeId)
            .ToDictionary(g => g.Key, g => g.Count());

        return types
            .Where(t => typeCounts.ContainsKey(t.Id))
            .Select(t => new FilterOptionDto
            {
                Id = t.Id.ToString(),
                Name = t.Name,
                Count = typeCounts[t.Id]
            })
            .OrderBy(f => f.Name)
            .ToList();
    }

    private async Task<List<FilterOptionDto>> BuildThematicAreaFilters(SearchOpportunitiesQuery request)
    {
        var areas = await _thematicAreaRepository.GetAllAsync();
        
        var (allOpportunities, _) = await _opportunityRepository.SearchOpportunitiesAsync(
            pageNumber: 1,
            pageSize: int.MaxValue,
            searchTerm: request.SearchTerm,
            sectorIds: request.SectorIds,
            opportunityTypeIds: request.OpportunityTypeIds,
            collaborationRequirementIds: request.CollaborationRequirementIds,
            expectedOutcomeIds: request.ExpectedOutcomeIds,
            asNoTracking: true);

        var areaCounts = allOpportunities
            .GroupBy(o => o.ThematicAreaId)
            .ToDictionary(g => g.Key, g => g.Count());

        return areas
            .Where(a => areaCounts.ContainsKey(a.Id))
            .Select(a => new FilterOptionDto
            {
                Id = a.Id.ToString(),
                Name = a.Name,
                Count = areaCounts[a.Id]
            })
            .OrderBy(f => f.Name)
            .ToList();
    }

    private async Task<List<FilterOptionDto>> BuildCollaborationRequirementFilters(SearchOpportunitiesQuery request)
    {
        var requirements = await _collaborationRequirementRepository.GetAllAsync();
        
        var (allOpportunities, _) = await _opportunityRepository.SearchOpportunitiesAsync(
            pageNumber: 1,
            pageSize: int.MaxValue,
            searchTerm: request.SearchTerm,
            sectorIds: request.SectorIds,
            opportunityTypeIds: request.OpportunityTypeIds,
            thematicAreaIds: request.ThematicAreaIds,
            expectedOutcomeIds: request.ExpectedOutcomeIds,
            asNoTracking: true);

        var requirementCounts = new Dictionary<int, int>();
        foreach (var opp in allOpportunities)
        {
            foreach (var req in opp.CollaborationRequirements)
            {
                if (requirementCounts.ContainsKey(req.Id))
                    requirementCounts[req.Id]++;
                else
                    requirementCounts[req.Id] = 1;
            }
        }

        return requirements
            .Where(r => requirementCounts.ContainsKey(r.Id))
            .Select(r => new FilterOptionDto
            {
                Id = r.Id.ToString(),
                Name = r.Name,
                Count = requirementCounts[r.Id]
            })
            .OrderBy(f => f.Name)
            .ToList();
    }

    private async Task<List<FilterOptionDto>> BuildExpectedOutcomeFilters(SearchOpportunitiesQuery request)
    {
        var outcomes = await _expectedOutcomesRepository.GetAllAsync();
        
        var (allOpportunities, _) = await _opportunityRepository.SearchOpportunitiesAsync(
            pageNumber: 1,
            pageSize: int.MaxValue,
            searchTerm: request.SearchTerm,
            sectorIds: request.SectorIds,
            opportunityTypeIds: request.OpportunityTypeIds,
            thematicAreaIds: request.ThematicAreaIds,
            collaborationRequirementIds: request.CollaborationRequirementIds,
            asNoTracking: true);

        var outcomeCounts = new Dictionary<int, int>();
        foreach (var opp in allOpportunities)
        {
            foreach (var outcome in opp.ExpectedOutcomes)
            {
                if (outcomeCounts.ContainsKey(outcome.Id))
                    outcomeCounts[outcome.Id]++;
                else
                    outcomeCounts[outcome.Id] = 1;
            }
        }

        return outcomes
            .Where(o => outcomeCounts.ContainsKey(o.Id))
            .Select(o => new FilterOptionDto
            {
                Id = o.Id.ToString(),
                Name = o.Name,
                Count = outcomeCounts[o.Id]
            })
            .OrderBy(f => f.Name)
            .ToList();
    }
}
