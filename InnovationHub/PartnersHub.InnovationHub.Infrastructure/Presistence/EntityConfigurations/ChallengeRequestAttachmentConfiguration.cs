using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;


namespace PartnersHub.InnovationHub.Infrastructure.Presistence.EntityConfigurations;

/// <summary>
/// Entity Framework configuration for ChallengeRequestAttachment entity.
/// </summary>
public class ChallengeRequestAttachmentConfiguration : EntityConfiguration<ChallengeRequestAttachment> 
{
    public override void Configure(EntityTypeBuilder<ChallengeRequestAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChallengeRequestAttachments");

        builder.Property(a => a.ChallengeRequestId)
            .IsRequired();

        // Configure Attachment as owned type (value object)
        builder.OwnsOne(a => a.Metadata, metadata => {
            metadata.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("FileName");

            metadata.Property(m => m.Extension)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("FileExtension");

            metadata.Property(m => m.SizeInBytes)
                .IsRequired()
                .HasColumnName("FileSizeInBytes");

            metadata.Property(m => m.Format)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("FileFormat");
        });

        // SharePoint properties
        builder.Property(a => a.SharePointFileId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.SharePointUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.SharePointLibrary)
            .IsRequired()
            .HasMaxLength(200);

        // Audit properties
        builder.Property(a => a.UploadedBy)
            .IsRequired();

        builder.Property(a => a.UploadedAt)
            .IsRequired();

        builder.Property(a => a.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.DeletedBy);

        builder.Property(a => a.DeletedAt);

        // Indexes for performance
        builder.HasIndex(a => a.ChallengeRequestId)
            .HasDatabaseName("IX_ChallengeRequestAttachments_ChallengeRequestId");

        builder.HasIndex(a => a.UploadedAt)
            .HasDatabaseName("IX_ChallengeRequestAttachments_UploadedAt");

        builder.HasIndex(a => a.IsDeleted)
            .HasDatabaseName("IX_ChallengeRequestAttachments_IsDeleted");

        builder.HasIndex(a => new { a.ChallengeRequestId, a.IsDeleted })
            .HasDatabaseName("IX_ChallengeRequestAttachments_ChallengeRequestId_IsDeleted");

        // Relationship configuration
        builder.HasOne<ChallengeRequest>()
            .WithMany()
            .HasForeignKey(a => a.ChallengeRequestId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ChallengeRequestAttachments_ChallengeRequests");
    }
}
