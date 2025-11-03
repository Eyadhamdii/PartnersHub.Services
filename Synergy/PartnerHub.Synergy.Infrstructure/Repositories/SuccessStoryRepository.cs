using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Application.Models;
using PartnersHub.Synergy.Application.SuccessStories.DTOs;
using PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;
using PartnersHub.Synergy.Domain.Common;
using PartnersHub.Synergy.Infrastructure.Persistence;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PartnersHub.Synergy.Infrastructure.Repositories;

    public class SuccessStoryRepository : ISuccessStoryRepository
    {
        private readonly SynergyDbContext _context;

        public SuccessStoryRepository(SynergyDbContext context)
        {
            _context = context;
        }

    public async Task<SuccessStoryResponseDto> GetByIdAsync(Guid id)
    {




        var query =
            from story in _context.SuccessStories
            join company in _context.SynergyCompanies on story.CompanyId equals company.Id
            join type in _context.SuccessStoryTypes on story.SuccessStoryTypeId equals type.Id
            join collaborator in _context.SuccessStorySynergyCompanies on story.Id equals collaborator.SuccessStoryId into collaboratorGroup
            select new
            {
                Story = story,
                Company = company,
                Type = type,
                Collaborators = from cg in collaboratorGroup
                                join sc in _context.SynergyCompanies on cg.SynergyCompanyId equals sc.Id
                                select sc
            };

        query = query.Where(g => g.Story.Id == id);


        var result = await (query.Select(x => new SuccessStoryResponseDto()
        {
            Id = x.Story.Id,
            Title = x.Story.Title.Value,
            Description = x.Story.Description.Value,
            CompanyName = x.Company.Name.Value,
            CompanyId = x.Company.Id,
            EndDate = x.Story.EndDate,
            StartDate = x.Story.StartDate,
            SuccessStoryStatus = ((SuccessStoryStatus)x.Story.Status).ToString(),
            SuccessStoryType = x.Type.Name,
            CollaboratingPartners = x.Collaborators.Select(c => new GuidKeyValueDto(c.Id, c.Name.Value)).ToList()

        }).FirstOrDefaultAsync());

        return result;
    }
    /*
    public async Task<SuccessStory?> GetByIdAsync(Guid id, bool asNoTracking = false, params Expression<Func<SuccessStory, object>>[] includes)
    {
        IQueryable<SuccessStory> query = _context.SuccessStories.Where(s => s.Id == id);

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync();
    }*/

    public async Task AddAsync(SuccessStory successStory)
    {
        await _context.SuccessStories.AddAsync(successStory);
    }

    public void Update(SuccessStory successStory)
    {
        _context.SuccessStories.Update(successStory);
    }

    public void Delete(SuccessStory successStory)
    {
        _context.SuccessStories.Remove(successStory);
    }

    public async Task<IEnumerable<SuccessStory>> GetAllAsync(bool asNoTracking = false, params Expression<Func<SuccessStory, object>>[] includes)
    {
        IQueryable<SuccessStory> query = _context.SuccessStories;

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<(List<SuccessStory> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? companyId = null,
        SuccessStoryStatus? status = null,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = true,
        bool asNoTracking = false)
    {
        IQueryable<SuccessStory> query = _context.SuccessStories;

        if (asNoTracking)
            query = query.AsNoTracking();

        // Apply filters
        if (companyId.HasValue)
            query = query.Where(s => s.CompanyId == companyId.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(s => s.Title.Value.ToLower().Contains(term));
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = (sortBy?.ToLower(), sortDescending) switch
        {
            ("title", true) => query.OrderByDescending(s => s.Title.Value),
            ("title", false) => query.OrderBy(s => s.Title.Value),
            ("createdat", true) => query.OrderByDescending(s => s.CreatedAt),
            ("createdat", false) => query.OrderBy(s => s.CreatedAt),
            ("submissiondate", true) => query.OrderByDescending(s => s.CreatedAt),
            ("submissiondate", false) => query.OrderBy(s => s.CreatedAt),
            ("status", true) => query.OrderByDescending(s => s.Status),
            ("status", false) => query.OrderBy(s => s.Status),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<(List<SuccessStoryResponseDto> Items, int TotalCount)> Search(
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
            bool asNoTracking = false)
    {
        var query =
            from story in _context.SuccessStories
            join company in _context.SynergyCompanies on story.CompanyId equals company.Id
            join type in _context.SuccessStoryTypes on story.SuccessStoryTypeId equals type.Id
            join collaborator in _context.SuccessStorySynergyCompanies on story.Id equals collaborator.SuccessStoryId into collaboratorGroup
            select new
            {
                Story = story,
                Company = company,
                Type = type,
                Collaborators = from cg in collaboratorGroup
                                join sc in _context.SynergyCompanies on cg.SynergyCompanyId equals sc.Id
                                select sc
            };

        if (companyId.HasValue)
            query = query.Where(x => x.Story.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(partnerCompanyName))
        {
            query = query.Where(x => x.Collaborators.Any(c => c.Name.Value.ToLower() == partnerCompanyName.ToLower()));
        }

        if (startDate.HasValue)
            query = query.Where(x => x.Story.StartDate >= startDate);

        if (endDate.HasValue)
            query = query.Where(x => x.Story.EndDate <= endDate);

        if (sectorIds is { Count: > 0 })
            query = query.Where(x => _context.SynergyCompanySectors.
            Any(s => s.CompanyId == x.Company.Id && sectorIds.Contains(s.SectorId)));
        // ...
        int count = await query.CountAsync();
        query = (sortBy?.ToLower(), sortDescending) switch
        {
            ("title", true) => query.OrderByDescending(x => x.Story.Title.Value),
            ("title", false) => query.OrderBy(x => x.Story.Title.Value),
            ("createdat", true) => query.OrderByDescending(x => x.Story.CreatedAt),
            ("createdat", false) => query.OrderBy(x => x.Story.CreatedAt),
            ("submissiondate", true) => query.OrderByDescending(x => x.Story.CreatedAt),
            ("submissiondate", false) => query.OrderBy(x => x.Story.CreatedAt),
            ("status", true) => query.OrderByDescending(x => x.Story.Status),
            ("status", false) => query.OrderBy(x => x.Story.Status),
            _ => query.OrderByDescending(x => x.Story.CreatedAt)
        };
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SuccessStoryResponseDto
            {
                Id = x.Story.Id,
                CompanyId = x.Company.Id,
                CompanyName = x.Company.Name.Value,
                Title = x.Story.Title.Value,
                Description = x.Story.Description.Value,
                SuccessStoryType = x.Type.Name,
                StartDate = x.Story.StartDate,
                EndDate = x.Story.EndDate,
                SuccessStoryStatus = ((SuccessStoryStatus)x.Story.Status).ToString(),
                CollaboratingPartners = x.Collaborators.Select(c => new GuidKeyValueDto(c.Id, c.Name.Value)).ToList()
            })
            .ToListAsync();

        return (items, count);
    }

    #region Dashboard Statistics

    public async Task<int> GetTotalCountByStatusAsync(SuccessStoryStatus status, DateTime? fromDate = null)
    {
        var query = _context.SuccessStories.Where(s => s.Status == status);

        if (fromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= fromDate.Value);

        return await query.CountAsync();
    }

    public async Task<int> GetCountByCompanyAndStatusAsync(Guid companyId, SuccessStoryStatus status, DateTime? fromDate = null)
    {
        var query = _context.SuccessStories
            .Where(s => s.CompanyId == companyId && s.Status == status);

        if (fromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= fromDate.Value);

        return await query.CountAsync();
    }

    public async Task<List<SuccessStory>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.SuccessStories
            .Where(s => s.CompanyId == companyId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }


    #endregion
}
