using Microsoft.EntityFrameworkCore;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeTechnologiesRequest;
using PartnersHub.InnovationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Infrastructure.Presistence
{
    public class InnovationHubDbContext : DbContext
    {
        public InnovationHubDbContext(DbContextOptions<InnovationHubDbContext> options)
            : base(options)
        {
        }
        public DbSet<ChallengeRequest> challengeRequests => Set<ChallengeRequest>();
        public DbSet<ChallengeRequestRevisionComment> challengeRequestRevisionComments => Set<ChallengeRequestRevisionComment>();
        public DbSet<ChallengeTrackingHistory> challengeTrackingHistories => Set<ChallengeTrackingHistory>();
        public DbSet<ChallengeTechnologiesRequest> challengeTechnologiesRequests => Set<ChallengeTechnologiesRequest>();
        public DbSet<Technology> technologies => Set<Technology>();
        public DbSet<ChallengeRequestAssociatedProvider> associatedProviders => Set<ChallengeRequestAssociatedProvider>();
        public DbSet<ChallengeRequestAssociatedSector> associatedSectors => Set<ChallengeRequestAssociatedSector>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
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
}
