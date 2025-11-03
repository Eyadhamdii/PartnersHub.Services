using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Communities.Domain.Aggregates.Community;
using PartnersHub.Communities.Domain.ValueObjects;

namespace PartnersHub.Communities.Infrastructure.Persistence.EntityConfigurations;

public class CommunityPostConfiguration : IEntityTypeConfiguration<CommunityPost>
{
    public void Configure(EntityTypeBuilder<CommunityPost> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Content)
            .HasConversion(
                content => content.Value,
                value => PostContent.Create(value))
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasOne<Community>()
            .WithMany()
            .HasForeignKey(p => p.CommunityId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events collection
        //builder.Ignore(p => p.DomainEvents);
    }
}
