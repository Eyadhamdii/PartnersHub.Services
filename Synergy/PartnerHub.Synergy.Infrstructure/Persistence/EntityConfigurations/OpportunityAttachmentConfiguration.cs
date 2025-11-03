using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;

namespace PartnersHub.Synergy.Infrastructure.Persistence.EntityConfigurations;

public class OpportunityAttachmentConfiguration : IEntityTypeConfiguration<OpportunityAttachment>
{
    public void Configure(EntityTypeBuilder<OpportunityAttachment> builder)
    {
        builder.ToTable("OpportunityAttachments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.OpportunityId).IsRequired();
        builder.Property(a => a.FileName).IsRequired().HasMaxLength(255);
        builder.Property(a => a.SharePointUrl).IsRequired().HasMaxLength(500);
        builder.Property(a => a.FileExtension).IsRequired().HasMaxLength(10);
        builder.Property(a => a.FileSizeInBytes).IsRequired();
        builder.Property(a => a.UploadedAt).IsRequired();
        builder.Property(a => a.UploadedBy).HasMaxLength(255);

        builder.HasIndex(a => a.OpportunityId);
    }
}
