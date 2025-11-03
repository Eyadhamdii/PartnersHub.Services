using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

public interface IInfrabaseRequestRepository {
    // Query methods
    Task<InfraRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InfraRequest?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<InfraRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InfraRequest>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<InfraRequest>> GetByCreatedByAsync(Guid userId, CancellationToken cancellationToken = default);
    
    // Paginated queries
    Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetAllPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetByStatusPaginatedAsync(
        RequestStatus status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetByCreatedByPaginatedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    // Lightweight paginated query for list views (no navigation properties)
    Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetSummaryPaginatedAsync(
        Guid? companyId = null,
        RequestStatus? status = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    // Utility methods
    Task<bool> IsProjectNameUniqueAsync(string projectName, Guid? excludeRequestId = null, CancellationToken cancellationToken = default);
    Task<string> GetNextRequestCodeAsync(CancellationToken cancellationToken = default);
    
    // Command methods
    Task AddAsync(InfraRequest request, CancellationToken cancellationToken = default);
    void Delete(InfraRequest request);
}