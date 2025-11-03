using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;
using PartnersHub.Synergy.Domain.ValueObjects;

namespace PartnersHub.Synergy.Infrastructure.Persistence.EntityConfigurations;

public class SuccessStoryConfiguration : IEntityTypeConfiguration<SuccessStory>
{
    public void Configure(EntityTypeBuilder<SuccessStory> builder)
    {
        // Table Name
        builder.ToTable("SuccessStories");

        // Primary Key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        // Required properties
        builder.Property(s => s.CompanyId).IsRequired();
        builder.Property(s => s.SuccessStoryTypeId).IsRequired();
        builder.Property(s => s.CollaborationStatusId).IsRequired();
        builder.Property(s => s.TermsAndConditionId).IsRequired();
        builder.Property(s => s.CreatedBy).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.TermsAccepted).IsRequired();

        // Value Object Mappings
        builder.OwnsOne(s => s.Title, title =>
        {
            title.Property(t => t.Value)
                 .HasColumnName("Title")
                 .HasMaxLength(Title.MaxLength)
                 .IsRequired();
        });

        builder.HasOne(s => s.SuccessStoryType)
       .WithMany()
       .HasForeignKey(o => o.SuccessStoryTypeId);
        builder.OwnsOne(s => s.Description, description =>
        {
            description.Property(d => d.Value)
                       .HasColumnName("Description")
                       .HasMaxLength(Description.MaxLength);
            // Note: Description.Value is nullable in the domain, so IsRequired() is omitted.
        });

        // Enum/Status Mapping (stored as string for readability, or int by default)
        builder.Property(s => s.Status)
               .HasConversion<string>() // Store enum as string
               .HasMaxLength(50)
               .IsRequired();

        // DateTime properties
        builder.Property(s => s.StartDate).IsRequired();
        builder.Property(s => s.EndDate).IsRequired();

        // Optional properties
        builder.Property(s => s.RejectionReason).HasMaxLength(500);

        builder.HasMany(o => o.CollaboratedProfiles)
       .WithOne()
       .HasForeignKey(osc => osc.SuccessStoryId)
       .OnDelete(DeleteBehavior.Cascade);
        // Attachments (One-to-Many relationship)
        // Maps the private field '_attachments' and sets up the relationship
        builder.HasMany(s => s.Attachments)
               .WithOne() // Assuming Attachment doesn't need a direct navigation property back to SuccessStory
               .HasForeignKey(s => s.SuccessStoryId) // Shadow property for FK
               .OnDelete(DeleteBehavior.Cascade);




        builder.Navigation(s => s.AssociatedOpportunities)
   .UsePropertyAccessMode(PropertyAccessMode.Field);





        // Table Name
        builder.ToTable("SuccessStories");
    }
}