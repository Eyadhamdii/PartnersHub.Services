using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence.Configurations;

public class TermsAndConditionConfiguration : IEntityTypeConfiguration<TermsAndCondition> {
    public void Configure(EntityTypeBuilder<TermsAndCondition> builder) {
        builder.ToTable("TermsAndConditions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(t => t.TitleAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.TitleEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.ContentAr)
            .IsRequired();

        builder.Property(t => t.ContentEn)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(t => t.EffectiveDate)
            .IsRequired();

        builder.Property(t => t.RequiresAcceptance)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasIndex(t => new { t.Type, t.Status });
        builder.HasIndex(t => new { t.Version, t.Type });
    }
}