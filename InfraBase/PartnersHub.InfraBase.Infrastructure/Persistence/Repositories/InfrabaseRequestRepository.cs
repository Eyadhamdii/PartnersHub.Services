using Microsoft.EntityFrameworkCore;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Infrastructure.Persistence.Repositories;

public class InfrabaseRequestRepository : IInfrabaseRequestRepository {
    private readonly InfrabaseDbContext _context;

    public InfrabaseRequestRepository(InfrabaseDbContext context) {
        _context = context;
    }

    public async Task<InfraRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _context.InfraRequests
            .Include(r => r.Items)
                .ThenInclude(i => i.FinancialDistributions)
            .Include(r => r.History)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<InfraRequest?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default) {
        // Load WITH items and distributions but WITHOUT History
        // This allows modifications to items while letting History be added through domain methods
        return await _context.InfraRequests
            .Include(r => r.Items)
                .ThenInclude(i => i.FinancialDistributions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<InfraRequest>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _context.InfraRequests
            .Include(r => r.Items)
            .Include(r => r.History)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InfraRequest>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default) {
        return await _context.InfraRequests
            .Include(r => r.Items)
            .Include(r => r.History)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InfraRequest>> GetByCreatedByAsync(Guid userId, CancellationToken cancellationToken = default) {
        return await _context.InfraRequests
            .Include(r => r.Items)
            .Include(r => r.History)
            .Where(r => r.CreatedBy == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    // Paginated queries
    public async Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetAllPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) {
        
        var query = _context.InfraRequests
            .Include(r => r.Items)
            .Include(r => r.History)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetByStatusPaginatedAsync(
        RequestStatus status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) {
        
        var query = _context.InfraRequests
            .Include(r => r.Items)
            .Include(r => r.History)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetByCreatedByPaginatedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) {
        
        var query = _context.InfraRequests
            .Include(r => r.Items)
            .Include(r => r.History)
            .Where(r => r.CreatedBy == userId)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IEnumerable<InfraRequest> Items, int TotalCount)> GetSummaryPaginatedAsync(
        Guid? companyId = null,
        RequestStatus? status = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default) {
        
        // Use AsNoTracking for read-only list queries - better performance
        var query = _context.InfraRequests.AsNoTracking();

        // Apply filters
        if (companyId.HasValue && companyId.Value != Guid.Empty) {
            query = query.Where(r => r.CompanyId == companyId.Value);
        }

        if (status.HasValue) {
            query = query.Where(r => r.Status == status.Value);
        }

        // Order by most recent first
        query = query.OrderByDescending(r => r.SubmittedAt ?? r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        // Project to lightweight objects WITHOUT loading navigation properties
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> IsProjectNameUniqueAsync(string projectName, Guid? excludeRequestId = null, CancellationToken cancellationToken = default) {
        var query = _context.InfraRequests.AsQueryable();

        if (excludeRequestId.HasValue) {
            query = query.Where(r => r.Id != excludeRequestId.Value);
        }

        return !await query.AnyAsync(r => r.ProjectName.Value == projectName, cancellationToken);
    }

    public async Task<string> GetNextRequestCodeAsync(CancellationToken cancellationToken = default) {
        var maxCode = await _context.InfraRequests
            .Where(r => r.RequestCode != null)
            .OrderByDescending(r => r.RequestCode)
            .Select(r => r.RequestCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(maxCode)) {
            return "IFRA-REQ-00001";
        }

        var numericPart = maxCode.Replace("IFRA-REQ-", "", StringComparison.OrdinalIgnoreCase);
        if (int.TryParse(numericPart, out var number)) {
            return $"IFRA-REQ-{(number + 1):D5}";
        }

        return "IFRA-REQ-00001";
    }

    public async Task AddAsync(InfraRequest request, CancellationToken cancellationToken = default) {
        await _context.InfraRequests.AddAsync(request, cancellationToken);
    }

    public void Delete(InfraRequest request) {
        _context.InfraRequests.Remove(request);
    }
}