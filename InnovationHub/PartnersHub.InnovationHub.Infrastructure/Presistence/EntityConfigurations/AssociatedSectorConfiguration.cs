using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;


namespace PartnersHub.InnovationHub.Infrastructure.Presistence.EntityConfigurations;

public class AssociatedSectorConfiguration : IEntityTypeConfiguration<ChallengeRequestAssociatedSector>
{
    public void Configure(EntityTypeBuilder<ChallengeRequestAssociatedSector> builder)
    {
        builder.ToTable("AssociatedSectors");

        builder.HasKey(asct => asct.Id);

        builder.Property(asct => asct.Id)
            .ValueGeneratedNever();

        builder.Property(asct => asct.Name)
            .HasColumnName("Name")
            .HasMaxLength(255)
            .IsRequired();
    }
}

