using Microsoft.EntityFrameworkCore;
using PartnersHub.Communities.Domain.Aggregates.Community;
using PartnersHub.Communities.Domain.Common;
using PartnersHub.Communities.Application.Common.Interfaces;
using System.Reflection;

namespace PartnersHub.Communities.Infrastructure.Persistence;

public class CommunitiesDbContext : DbContext
{
    public CommunitiesDbContext(DbContextOptions<CommunitiesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Community> Communities => Set<Community>();
    public DbSet<CommunityFollower> CommunityFollowers => Set<CommunityFollower>();
    public DbSet<CommunityPost> CommunityPosts => Set<CommunityPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ignore domain events - they should not be persisted
        modelBuilder.Ignore<DomainEvent>();

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Clear domain events after saving to prevent re-publishing
        var result = await base.SaveChangesAsync(cancellationToken);

        var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        return result;
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly CommunitiesDbContext _context;

    public UnitOfWork(CommunitiesDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}