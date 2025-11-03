using PartnersHub.Synergy.Application.SuccessStories.DTOs;
using PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;
using PartnersHub.Synergy.Domain.Common;
using System.Linq.Expressions;

namespace PartnersHub.Synergy.Application.Interfaces.Repository;

    public interface ISuccessStoryRepository
    {
    public Task<SuccessStoryResponseDto> GetByIdAsync(Guid id);
        Task AddAsync(SuccessStory successStory);
    void Update(SuccessStory successStory);
    void Delete(SuccessStory successStory);
    Task<IEnumerable<SuccessStory>> GetAllAsync(bool asNoTracking = false, params Expression<Func<SuccessStory, object>>[] includes);
    
    /// <summary>
    /// Get paginated success stories with filtering, sorting, and search
    /// </summary>
    Task<(List<SuccessStory> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? companyId = null,
        SuccessStoryStatus? status = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true,
        bool asNoTracking = false);
    public Task<(List<SuccessStoryResponseDto> Items, int TotalCount)> Search(
        int pageNumber,
        int pageSize,
        Guid? companyId = null,
        string? partnerCompanyName = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        SuccessStoryStatus? status = null,
        List<Guid>? sectorIds = null,
        int? collaborationTypeId = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true,
        bool asNoTracking = false);
    // Dashboard statistics methods
    Task<int> GetTotalCountByStatusAsync(SuccessStoryStatus status, DateTime? fromDate = null);
    Task<int> GetCountByCompanyAndStatusAsync(Guid companyId, SuccessStoryStatus status, DateTime? fromDate = null);
    Task<List<Domain.Aggregates.SuccessStoryAggregate.SuccessStory>> GetByCompanyIdAsync(Guid companyId);
}
