using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Application.Interfaces;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;
using PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Domain.Common;
using System.Reflection;

namespace PartnersHub.Synergy.Infrastructure.Persistence;

public class SynergyDbContext : DbContext
{
    public SynergyDbContext(DbContextOptions<SynergyDbContext> options)
        : base(options)
    {
    }
    public DbSet<OpportunityType> OpportunityTypes => Set<OpportunityType>();
    public DbSet<ThematicArea> ThematicAreas => Set<ThematicArea>();
    public DbSet<CollaborationRequirement> CollaborationRequirements => Set<CollaborationRequirement>();
    public DbSet<SynergyCompany> SynergyCompanies => Set<SynergyCompany>();
    public DbSet<ExpectedOutcome> ExpectedOutcomes => Set<ExpectedOutcome>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<SuccessStory> SuccessStories => Set<SuccessStory>();
    public DbSet<SuccessStoryType> SuccessStoryTypes => Set<SuccessStoryType>();
    public DbSet<OpportunitySynergyCompany> OpportunitySynergyCompanies => Set<OpportunitySynergyCompany>();
    public DbSet<SuccessStorySynergyCompany> SuccessStorySynergyCompanies => Set<SuccessStorySynergyCompany>();
    public DbSet<SynergyCompanySector> SynergyCompanySectors => Set<SynergyCompanySector>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ignore domain events - they should not be persisted
        modelBuilder.Ignore<DomainEvent>();

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.SeedData();
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
    private readonly SynergyDbContext _context;

    public UnitOfWork(SynergyDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}