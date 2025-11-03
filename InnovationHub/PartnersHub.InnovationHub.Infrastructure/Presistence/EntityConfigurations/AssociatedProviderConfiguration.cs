using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;


namespace PartnersHub.InnovationHub.Infrastructure.Presistence.EntityConfigurations;

public class AssociatedProviderConfiguration : IEntityTypeConfiguration<ChallengeRequestAssociatedProvider>
{
    public void Configure(EntityTypeBuilder<ChallengeRequestAssociatedProvider> builder)
    {
        builder.ToTable("AssociatedProviders");

        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Id)
            .ValueGeneratedNever();

        builder.Property(ap => ap.Name)
            .HasColumnName("Name")
            .HasMaxLength(255)
            .IsRequired();
    }
}

