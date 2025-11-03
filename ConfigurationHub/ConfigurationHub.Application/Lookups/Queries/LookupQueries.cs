using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;

namespace PartnersHub.ConfigurationHub.Application.Lookups.Queries;

// Sector Queries
public record GetAllSectorsQuery : IRequest<IEnumerable<SectorDto>>;
public record GetActiveSectorsQuery : IRequest<IEnumerable<SectorDto>>;
public record GetSectorByIdQuery : IRequest<SectorDto?> {
    public Guid Id { get; init; }
}

// SubSector Queries
public record GetAllSubSectorsQuery : IRequest<IEnumerable<SubSectorDto>>;
public record GetActiveSubSectorsQuery : IRequest<IEnumerable<SubSectorDto>>;
public record GetSubSectorsBySectorIdQuery : IRequest<IEnumerable<SubSectorDto>> {
    public Guid SectorId { get; init; }
}

// AssetType Queries
public record GetAllAssetTypesQuery : IRequest<IEnumerable<AssetTypeDto>>;
public record GetActiveAssetTypesQuery : IRequest<IEnumerable<AssetTypeDto>>;

// UnitOfMeasurement Queries
public record GetAllUnitsOfMeasurementQuery : IRequest<IEnumerable<UnitOfMeasurementDto>>;
public record GetActiveUnitsOfMeasurementQuery : IRequest<IEnumerable<UnitOfMeasurementDto>>;