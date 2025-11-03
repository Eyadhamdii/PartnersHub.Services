using System;
using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;
using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Infrastructure.Persistence;

public class InfrabaseDbContext : DbContext {
    private readonly IMediator? _mediator;

    public InfrabaseDbContext(DbContextOptions<InfrabaseDbContext> options)
        : base(options) {
    }

    public InfrabaseDbContext(
        DbContextOptions<InfrabaseDbContext> options,
        IMediator mediator)
        : base(options) {
        _mediator = mediator;
    }

    public DbSet<InfraRequest> InfraRequests => Set<InfraRequest>();
    public DbSet<InfraRequestItem> InfraRequestItems => Set<InfraRequestItem>();
    public DbSet<InfraRequestItemFinancialDistribution> InfraRequestItemFinancialDistributions => Set<InfraRequestItemFinancialDistribution>();
    public DbSet<InfraRequestHistory> InfraRequestHistories => Set<InfraRequestHistory>();
    public DbSet<InfraRequestAttachment> InfraRequestAttachments => Set<InfraRequestAttachment>();


    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
        try {
            var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (_mediator != null) {
                foreach (var entity in entitiesWithEvents) {
                    var events = entity.DomainEvents.ToArray();

                    foreach (var domainEvent in events) {
                        await _mediator.Publish(domainEvent, cancellationToken);
                    }

                    entity.ClearDomainEvents();
                }
            } else {
                foreach (var entity in entitiesWithEvents) {
                    entity.ClearDomainEvents();
                }
            }

            return result;
        } catch (DbUpdateConcurrencyException ex) {
            foreach (var entry in ex.Entries) {
                if (entry.Entity is InfraRequest || entry.Entity is InfraRequestHistory) {
                    var databaseValues = await entry.GetDatabaseValuesAsync();
                    if (databaseValues != null) {
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                } else {
                    throw new NotSupportedException(
                        $"Concurrency conflict not supported for {entry.Metadata.Name}");
                }
            }
            throw;
        }
    }
}