using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Domain.Common;
using PartnersHub.Synergy.Infrastructure.Persistence;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PartnersHub.Synergy.Infrastructure.Repositories;

public class OpportunityRepository : IOpportunityRepository
{
    private readonly SynergyDbContext _context;

    public OpportunityRepository(SynergyDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Opportunity opportunity)
    {
        await _context.Opportunities.AddAsync(opportunity);
    }

    public void Delete(Opportunity opportunity)
    {
        _context.Opportunities.Remove(opportunity);
    }

    public async Task<IEnumerable<Opportunity>> GetAllAsync(bool asNoTracking = false, params Expression<Func<Opportunity, object>>[] includes)
    {
        IQueryable<Opportunity> query = _context.Opportunities.AsNoTracking();

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<bool> IsOpportunityWithTitleAndCompanyExistsAsync(string title, Guid companyId)
    {
        return await _context.Opportunities
            .AnyAsync(o => o.Title.Value == title && o.CompanyId == companyId);
    }

    public async Task<Opportunity?> GetByIdAsync(Guid id, bool asNoTracking = false, params Expression<Func<Opportunity, object>>[] includes)
    {
        IQueryable<Opportunity> query = _context.Opportunities.Where(o => o.Id == id);

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync();
    }

    public void Update(Opportunity opportunity)
    {
        _context.Opportunities.Update(opportunity);
    }

    public async Task<(List<Opportunity> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? companyId = null,
        OpportunityStatus? status = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true,
        bool asNoTracking = false,
        params Expression<Func<Opportunity, object>>[] includes)
    {
        IQueryable<Opportunity> query = _context.Opportunities
            .Include(o => o.OpportunityType)
            .Include(o => o.Sector);

         if (asNoTracking)
            query = query.AsNoTracking();

        // Apply filters
        if (companyId.HasValue)
            query = query.Where(o => o.CompanyId == companyId.Value);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(o =>
                o.Title.Value.ToLower().Contains(term) ||
                o.Sector != null && o.Sector.Value.ToLower().Contains(term) ||
                o.OpportunityType != null && o.OpportunityType.Name.ToLower().Contains(term));
        }

        // Apply includes
        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = (sortBy?.ToLower(), sortDescending) switch
        {
            ("title", true) => query.OrderByDescending(o => o.Title.Value),
            ("title", false) => query.OrderBy(o => o.Title.Value),
            ("createdat", true) => query.OrderByDescending(o => o.CreatedAt),
            ("createdat", false) => query.OrderBy(o => o.CreatedAt),
            ("submissiondate", true) => query.OrderByDescending(o => o.CreatedAt),
            ("submissiondate", false) => query.OrderBy(o => o.CreatedAt),
            ("status", true) => query.OrderByDescending(o => o.Status),
            ("status", false) => query.OrderBy(o => o.Status),
            _ => query.OrderByDescending(o => o.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    #region Dashboard Statistics

    public async Task<int> GetTotalCountByStatusAsync(OpportunityStatus status, DateTime? fromDate = null)
    {
        var query = _context.Opportunities.Where(o => o.Status == status);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        return await query.CountAsync();
    }

    public async Task<int> GetCountByCompanyAndStatusAsync(Guid companyId, OpportunityStatus status, DateTime? fromDate = null)
    {
        var query = _context.Opportunities
            .Where(o => o.CompanyId == companyId && o.Status == status);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        return await query.CountAsync();
    }

    public async Task<int> GetDistinctCollaboratedCompaniesCountAsync(Guid companyId, DateTime? fromDate = null)
    {
        IQueryable<Opportunity> query = _context.Opportunities
            .Where(o => o.CompanyId == companyId)
            .Include(o => o.CollaboratedCompanies);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        var opportunities = await query.ToListAsync();
        var collaboratedCompanyIds = opportunities
            .SelectMany(o => o.CollaboratedCompanies)
            .Select(c => c.SynergyCompanyId)
            .Distinct()
            .Count();

        return collaboratedCompanyIds;
    }

    public async Task<List<Opportunity>> GetOpportunitiesByCollaboratedCompanyAsync(Guid companyId)
    {
        return await _context.Opportunities
            .Include(o => o.OpportunityType)
            .Include(o => o.ThematicArea)
            .Include(o => o.CollaboratedCompanies)
            .Where(o => o.CollaboratedCompanies.Any(c => c.SynergyCompanyId == companyId))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }


    public async Task<List<Opportunity>> GetByPublishingCompanyId(Guid companyId, bool asNoTracking = false, params Expression<Func<Opportunity, object>>[] includes)
    {
        IQueryable<Opportunity> query = _context.Opportunities.Where(o => o.CompanyId == companyId);

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<List<Opportunity>> GetByIds(List<Guid> opportunityIds, bool asNoTracking = false, params Expression<Func<Opportunity, object>>[] includes)
    {
        IQueryable<Opportunity> query = _context.Opportunities.Where(o => opportunityIds.Contains(o.Id));

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    private async Task<Dictionary<Opportunity, List<SynergyCompany>>> ApplyOpportunityCompaniesDictionaryQuery(List<Opportunity> opportunities)
    {
        // Extract the IDs of the pre-loaded opportunities
        var opportunityIds = opportunities.Select(o => o.Id).ToList();

        // Now, perform the JOIN/GROUPBY operation just for the associated companies
        // We filter the OpportunitySynergyCompanies by the IDs of the opportunities we just loaded.
        var companyAssociations = await _context.OpportunitySynergyCompanies
            .Where(occ => opportunityIds.Contains(occ.OpportunityId))
            .Join(_context.SynergyCompanies,
                occ => occ.SynergyCompanyId, // Join on SynergyCompanyId (the ID of the collaborating company)
                c => c.Id,           // Join on CompanyId (the actual ID in SynergyCompany)
                (occ, c) => new { occ.OpportunityId, Company = c }) // Project to anonymous type
            .ToListAsync();

        // Group the company associations by OpportunityId
        var companyDictionary = companyAssociations
            .GroupBy(x => x.OpportunityId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Company).ToList());

        // Merge the pre-loaded opportunities with the company lists
        var resultDictionary = new Dictionary<Opportunity, List<SynergyCompany>>();
        foreach (var opportunity in opportunities)
        {
            resultDictionary.Add(opportunity, companyDictionary.GetValueOrDefault(opportunity.Id, new List<SynergyCompany>()));
        }

        return resultDictionary;
    }


    public async Task<Dictionary<Opportunity, List<SynergyCompany>>> GetOpportunitiesWithCompanies(
        Guid companyId,
        bool asNoTracking = false,
        params Expression<Func<Opportunity, object>>[] includes)
    {
        var query = _context.Opportunities.Where(o => o.CompanyId == companyId);

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            query = includes.Aggregate(query, (q, i) => q.Include(i));
        }

        // *** FIX 2: Execute the query to get the fully-loaded entities first ***
        // This is the critical step that ensures Title, Description, and Sector (Value Objects) 
        // are correctly materialized by EF Core.
        var opportunities = await query.ToListAsync();

        // Now pass the list of fully loaded entities to the dictionary builder
        var opportunityDictionary = await ApplyOpportunityCompaniesDictionaryQuery(opportunities);
        return opportunityDictionary;
    }


    #endregion

    #region Search Opportunities

    public async Task<(List<Opportunity> Items, int TotalCount)> SearchOpportunitiesAsync(
        int pageNumber,
        int pageSize,
        Guid? companyId = null,
        string? searchTerm = null,
        List<Guid>? sectorIds = null,
        List<int>? opportunityTypeIds = null,
        List<int>? thematicAreaIds = null,
        List<int>? collaborationRequirementIds = null,
        List<int>? expectedOutcomeIds = null,
        List<OpportunityStatus>? statuses = null,
        string? sortBy = null,
        bool sortDescending = true,
        bool asNoTracking = true)
    {
        IQueryable<Opportunity> query = _context.Opportunities
            .Include(o => o.OpportunityType)
            .Include(o => o.ThematicArea)
            .Include(o => o.CollaborationRequirements)
            .Include(o => o.ExpectedOutcomes)
            .Include(o => o.CollaboratedCompanies);

        if (asNoTracking)
            query = query.AsNoTracking();

        // Filter by company (for user submissions)
        if (companyId.HasValue)
        {
            query = query.Where(o => o.CompanyId == companyId.Value);
        }

        // Filter by statuses (default to Published and AssetManagerApproved if not specified AND no companyId)
        if (statuses != null && statuses.Any())
        {
            query = query.Where(o => statuses.Contains(o.Status));
        }
        else if (!companyId.HasValue)
        {
            // Default: only show published opportunities for public search
            query = query.Where(o => 
                o.Status == OpportunityStatus.Published || 
                o.Status == OpportunityStatus.AssetManagerApproved);
        }
        // If companyId is provided and no statuses, show all statuses for that company

        // Apply search term
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(o =>
                o.Title.Value.ToLower().Contains(term) ||
                (o.Description != null && o.Description.Value != null && o.Description.Value.ToLower().Contains(term)));
        }

        // Apply sector filter
        if (sectorIds != null && sectorIds.Any())
        {
            query = query.Where(o => sectorIds.Contains(o.Sector.Id));
        }

        // Apply opportunity type filter
        if (opportunityTypeIds != null && opportunityTypeIds.Any())
        {
            query = query.Where(o => opportunityTypeIds.Contains(o.OpportunityTypeId));
        }

        // Apply thematic area filter
        if (thematicAreaIds != null && thematicAreaIds.Any())
        {
            query = query.Where(o => thematicAreaIds.Contains(o.ThematicAreaId));
        }

        // Apply collaboration requirement filter
        if (collaborationRequirementIds != null && collaborationRequirementIds.Any())
        {
            query = query.Where(o => o.CollaborationRequirements.Any(cr => 
                collaborationRequirementIds.Contains(cr.Id)));
        }

        // Apply expected outcome filter
        if (expectedOutcomeIds != null && expectedOutcomeIds.Any())
        {
            query = query.Where(o => o.ExpectedOutcomes.Any(eo => 
                expectedOutcomeIds.Contains(eo.Id)));
        }

        // Get total count AFTER filtering but BEFORE pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = (sortBy?.ToLower(), sortDescending) switch
        {
            ("title", true) => query.OrderByDescending(o => o.Title.Value),
            ("title", false) => query.OrderBy(o => o.Title.Value),
            ("startdate", true) => query.OrderByDescending(o => o.StartDate),
            ("startdate", false) => query.OrderBy(o => o.StartDate),
            ("enddate", true) => query.OrderByDescending(o => o.EndDate),
            ("enddate", false) => query.OrderBy(o => o.EndDate),
            ("status", true) => query.OrderByDescending(o => o.Status),
            ("status", false) => query.OrderBy(o => o.Status),
            ("submissiondate", true) or ("createdat", true) or _ when sortDescending => query.OrderByDescending(o => o.CreatedAt),
            ("submissiondate", false) or ("createdat", false) or _ => query.OrderBy(o => o.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    #endregion
}
