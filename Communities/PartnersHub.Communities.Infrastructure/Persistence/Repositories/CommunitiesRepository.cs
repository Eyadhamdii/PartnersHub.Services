using Microsoft.EntityFrameworkCore;
using PartnersHub.Communities.Application.Common.Interfaces.Rpository;
using PartnersHub.Communities.Domain.Aggregates.Community;
using System.Threading;

namespace PartnersHub.Communities.Infrastructure.Persistence.Repositories;

public class CommunitiesRepository : ICommunitiesRepository
{
    private readonly CommunitiesDbContext _context;

    public CommunitiesRepository(CommunitiesDbContext context)
    {
        _context = context;
    }

    public async Task<Community?> GetByIdAsync(Guid id,CancellationToken cancellationToken )
    {
        return await _context.Communities
            .Include(c => c.Followers)
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Community>> GetAllAsync()
    {
        return await _context.Communities
            .Include(c => c.Followers)
            .ToListAsync();
    }

    public async Task<IEnumerable<Community>> GetFollowedCommunitiesAsync(Guid userId)
    {
        return await _context.Communities
            .Include(c => c.Followers)
            .Where(c => c.Followers.Any(f => f.UserId == userId))
            .ToListAsync();
    }

    public async Task<bool> IsFollowingAsync(Guid communityId, Guid userId)
    {
        return await _context.CommunityFollowers
            .AnyAsync(f => f.CommunityId == communityId && f.UserId == userId);
    }

    public async Task<IEnumerable<CommunityPost>> GetCommunityPostsAsync(Guid communityId)
    {
        return await _context.CommunityPosts
            .Where(p => p.CommunityId == communityId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CommunityPost>> GetFollowedCommunitiesPostsAsync(Guid userId)
    {
        var followedCommunityIds = await _context.CommunityFollowers
            .Where(f => f.UserId == userId)
            .Select(f => f.CommunityId)
            .ToListAsync();

        return await _context.CommunityPosts
            .Where(p => followedCommunityIds.Contains(p.CommunityId) && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Community community)
    {
        await _context.Communities.AddAsync(community);
    }

    public void Update(Community community)
    {
        _context.Communities.Update(community);
    }

    public void Delete(Community community)
    {
        _context.Communities.Remove(community);
    }
}