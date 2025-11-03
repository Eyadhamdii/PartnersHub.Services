using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Communities.Domain.Aggregates.Community;
using PartnersHub.Communities.Domain.ValueObjects;

namespace PartnersHub.Communities.Infrastructure.Persistence.EntityConfigurations;

public class CommunityConfiguration : IEntityTypeConfiguration<Community>
{
    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasConversion(
                name => name.Value,
                value => CommunityName.Create(value))
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(c => c.Description)
            .HasConversion(
                description => description.Value,
                value => CommunityDescription.Create(value))
            .HasMaxLength(1000);
            
        builder.Property(c => c.ImageUrl)
            .HasConversion(
                imageUrl => imageUrl.Value,
                value => ImageUrl.Create(value))
            .HasMaxLength(2048);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();

        // Ignore domain events collection as it shouldn't be persisted
        builder.Ignore(c => c.DomainEvents);
    }
}
