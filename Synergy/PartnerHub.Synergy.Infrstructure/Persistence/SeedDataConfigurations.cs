using Microsoft.EntityFrameworkCore;
using PartnersHub.Synergy.Domain.Aggregates;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Domain.ValueObjects;

public static class SeedDataConfigurations
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OpportunityType>().HasData(
            new OpportunityType { Id = 1, Name = "Sponsorship" },
            new OpportunityType { Id = 2, Name = "Commercial Collaboration" },
            new OpportunityType { Id = 3, Name = "Strategic Collaboration" }
        );

        modelBuilder.Entity<ThematicArea>().HasData(
            new ThematicArea { Id = 1, Name = "ESG" },
            new ThematicArea { Id = 2, Name = "Digital Transformation" },
            new ThematicArea { Id = 3, Name = "Innovation" },
            new ThematicArea { Id = 4, Name = "Sustainability" }
        );

        modelBuilder.Entity<CollaborationRequirement>().HasData(
            new CollaborationRequirement { Id = 1, Name = "Technology transfer" },
            new CollaborationRequirement { Id = 2, Name = "Joint R&D" },
            new CollaborationRequirement { Id = 3, Name = "Co-creation" },
            new CollaborationRequirement { Id = 4, Name = "Other" }
        );

        modelBuilder.Entity<ExpectedOutcome>().HasData(
            new ExpectedOutcome { Id = 1, Name = "Revenue growth" },
            new ExpectedOutcome { Id = 2, Name = "Cost savings" },
            new ExpectedOutcome { Id = 3, Name = "Increased Efficiency" },
            new ExpectedOutcome { Id = 4, Name = "Other" }
        );

        modelBuilder.Entity<SuccessStoryType>().HasData(
            new SuccessStoryType { Id = 1, Name = "Partnership" },
            new SuccessStoryType { Id = 2, Name = "Collaboration" },
            new SuccessStoryType { Id = 3, Name = "Joint Venture" }
            );



    }
}
