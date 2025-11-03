using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

namespace PartnersHub.InfraBase.Infrastructure.Persistence.EntityConfigurations;

public class InfraRequestAttachmentConfiguration : IEntityTypeConfiguration<InfraRequestAttachment> {
    public void Configure(EntityTypeBuilder<InfraRequestAttachment> builder) {
        builder.ToTable("InfraRequestAttachments");

        builder.HasKey(a => a.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(a => a.RequestId)
            .IsRequired();

        // Configure AttachmentMetadata as owned type
        builder.OwnsOne(a => a.Metadata, metadata => {
            metadata.Property(m => m.FileName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("FileName");

            metadata.Property(m => m.FileExtension)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("FileExtension");

            metadata.Property(m => m.FileSizeInBytes)
                .IsRequired()
                .HasColumnName("FileSizeInBytes");

            metadata.Property(m => m.ContentType)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ContentType");
        });

        builder.Property(a => a.SharePointFileId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.SharePointUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.SharePointLibrary)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.UploadedBy)
            .IsRequired();

        builder.Property(a => a.UploadedAt)
            .IsRequired();

        builder.Property(a => a.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.DeletedBy);

        builder.Property(a => a.DeletedAt);

        // Indexes
        builder.HasIndex(a => a.RequestId);
        builder.HasIndex(a => a.UploadedAt);
        builder.HasIndex(a => a.IsDeleted);

        // Configure as part of InfraRequest aggregate
        builder.HasOne<InfraRequest>()
            .WithMany()
            .HasForeignKey(a => a.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
