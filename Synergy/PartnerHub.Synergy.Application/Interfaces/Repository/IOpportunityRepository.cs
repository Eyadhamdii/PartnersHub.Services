using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Domain.Common;
using System.Linq.Expressions;
using OpportunityEntity = PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate.Opportunity;
using SynergyCompanyEntity = PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate.SynergyCompany;

namespace PartnersHub.Synergy.Application.Interfaces.Repository;

public interface IOpportunityRepository
{
    Task<OpportunityEntity?> GetByIdAsync(Guid id, bool asNoTracking = false, params Expression<Func<OpportunityEntity, object>>[] includes);
    Task AddAsync(OpportunityEntity opportunity);
    void Update(OpportunityEntity opportunity);
    void Delete(OpportunityEntity opportunity);
    Task<bool> IsOpportunityWithTitleAndCompanyExistsAsync(string title, Guid companyId);
    Task<IEnumerable<OpportunityEntity>> GetAllAsync(bool asNoTracking = false, params Expression<Func<OpportunityEntity, object>>[] includes);
    Task<List<OpportunityEntity>> GetByPublishingCompanyId(Guid companyId, bool asNoTracking = false, params Expression<Func<OpportunityEntity, object>>[] includes);
    
    /// <summary>
    /// Unified method for searching opportunities with comprehensive filtering
    /// Handles both company-specific queries (user submissions) and platform-wide searches (public opportunities)
    /// </summary>
    Task<(List<OpportunityEntity> Items, int TotalCount)> SearchOpportunitiesAsync(
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
        bool asNoTracking = true);

    /// <summary>
    /// Legacy method - kept for backward compatibility. Use SearchOpportunitiesAsync instead.
    /// </summary>
    [Obsolete("Use SearchOpportunitiesAsync for better performance and more filters")]
    Task<(List<OpportunityEntity> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? companyId = null,
        OpportunityStatus? status = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true,
        bool asNoTracking = false,
        params Expression<Func<OpportunityEntity, object>>[] includes);

    // Dashboard statistics methods
    Task<int> GetTotalCountByStatusAsync(OpportunityStatus status, DateTime? fromDate = null);
    Task<int> GetCountByCompanyAndStatusAsync(Guid companyId, OpportunityStatus status, DateTime? fromDate = null);
    Task<int> GetDistinctCollaboratedCompaniesCountAsync(Guid companyId, DateTime? fromDate = null);
    Task<List<OpportunityEntity>> GetByIds(List<Guid> opportunityIds, bool asNoTracking = false, params Expression<Func<OpportunityEntity, object>>[] includes);
    Task<Dictionary<OpportunityEntity, List<SynergyCompanyEntity>>> GetOpportunitiesWithCompanies(Guid companyId, bool asNoTracking = false, params Expression<Func<OpportunityEntity, object>>[] includes);
    Task<List<OpportunityEntity>> GetOpportunitiesByCollaboratedCompanyAsync(Guid companyId);
}