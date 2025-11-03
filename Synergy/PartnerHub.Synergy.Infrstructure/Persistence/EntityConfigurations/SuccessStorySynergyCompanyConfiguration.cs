using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Infrastructure.Persistence.EntityConfigurations
{
    public class SuccessStorySynergyCompanyConfiguration : IEntityTypeConfiguration<SuccessStorySynergyCompany>
    {
        public void Configure(EntityTypeBuilder<SuccessStorySynergyCompany> builder)
        {
            builder.ToTable("SuccessStorySynergyCompanies");
            builder.HasKey(ssc => ssc.Id);
            builder.Property(ssc => ssc.Id).ValueGeneratedNever();

            builder.Property(ssc => ssc.SuccessStoryId).IsRequired();
            builder.Property(ssc => ssc.SynergyCompanyId).IsRequired();

            builder.HasIndex(ssc => ssc.SuccessStoryId);
            builder.HasIndex(ssc => ssc.SynergyCompanyId);
            builder.HasIndex(ssc => new { ssc.SuccessStoryId, ssc.SynergyCompanyId }).IsUnique();
        }
    }
}
