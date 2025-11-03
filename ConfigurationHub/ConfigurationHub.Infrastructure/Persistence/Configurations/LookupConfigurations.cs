using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

namespace PartnersHub.ConfigurationHub.Infrastructure.Persistence.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector> {
    public void Configure(EntityTypeBuilder<Sector> builder) {
        builder.ToTable("Sectors");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.DescriptionAr)
            .HasMaxLength(1000);

        builder.Property(s => s.DescriptionEn)
            .HasMaxLength(1000);

        builder.Property(s => s.DisplayOrder)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.HasIndex(s => s.Code)
            .IsUnique();

        builder.HasIndex(s => s.IsActive);
    }
}

public class SubSectorConfiguration : IEntityTypeConfiguration<SubSector> {
    public void Configure(EntityTypeBuilder<SubSector> builder) {
        builder.ToTable("SubSectors");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.DescriptionAr)
            .HasMaxLength(1000);

        builder.Property(s => s.DescriptionEn)
            .HasMaxLength(1000);

        builder.Property(s => s.DisplayOrder)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.HasIndex(s => new { s.SectorId, s.Code });
        builder.HasIndex(s => s.IsActive);
    }
}

public class AssetTypeConfiguration : IEntityTypeConfiguration<AssetType> {
    public void Configure(EntityTypeBuilder<AssetType> builder) {
        builder.ToTable("AssetTypes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.DescriptionAr)
            .HasMaxLength(1000);

        builder.Property(a => a.DescriptionEn)
            .HasMaxLength(1000);

        builder.Property(a => a.DisplayOrder)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .IsRequired();

        builder.HasIndex(a => a.Code)
            .IsUnique();

        builder.HasIndex(a => a.IsActive);
    }
}

public class UnitOfMeasurementConfiguration : IEntityTypeConfiguration<UnitOfMeasurement> {
    public void Configure(EntityTypeBuilder<UnitOfMeasurement> builder) {
        builder.ToTable("UnitsOfMeasurement");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Symbol)
            .HasMaxLength(20);

        builder.Property(u => u.DisplayOrder)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.HasIndex(u => u.Code)
            .IsUnique();

        builder.HasIndex(u => u.IsActive);
    }
}