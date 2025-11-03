using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence;

public class ConfigurationHubDbContext : DbContext {
    public ConfigurationHubDbContext(DbContextOptions<ConfigurationHubDbContext> options)
        : base(options) {
    }

    // Configuration
    public DbSet<WhiteListIP> WhiteListIPs => Set<WhiteListIP>();
    public DbSet<TermsAndCondition> TermsAndConditions => Set<TermsAndCondition>();

    // Lookups
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<SubSector> SubSectors => Set<SubSector>();
    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<UnitOfMeasurement> UnitsOfMeasurement => Set<UnitOfMeasurement>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Ignore domain events - they should not be persisted
        modelBuilder.Ignore<DomainEvent>();

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        // Clear domain events after saving to prevent re-publishing
        var result = await base.SaveChangesAsync(cancellationToken);

        var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        foreach (var entity in entitiesWithEvents) {
            entity.ClearDomainEvents();
        }

        return result;
    }
}