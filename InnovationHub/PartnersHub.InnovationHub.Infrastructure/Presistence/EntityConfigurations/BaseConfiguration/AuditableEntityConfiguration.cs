using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.InnovationHub.Domain.Common;

public abstract class AuditableEntityConfiguration<TAuditable> :
    EntityConfiguration<TAuditable> 
    where TAuditable : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<TAuditable> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CreatedBy)
               .IsRequired();

        builder.Property(e => e.CreatedAt)
               .IsRequired();

        builder.Property(e => e.UpdatedBy);

        builder.Property(e => e.UpdatedAt);
    }
}