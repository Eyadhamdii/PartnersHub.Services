
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Communities.Domain.Aggregates.Community;

namespace PartnersHub.Communities.Infrastructure.Persistence.EntityConfigurations;

public class CommunityFollowerConfiguration : IEntityTypeConfiguration<CommunityFollower>
{
    public void Configure(EntityTypeBuilder<CommunityFollower> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.CommunityId, e.UserId })
            .IsUnique();

        // Configure relationships
        builder.HasOne<Community>()
            .WithMany("_communityFollowers")
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
