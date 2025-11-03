using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

namespace PartnersHub.ConfigurationHub.Application.Lookups.Queries;

// Sector Query Handlers
public class GetAllSectorsQueryHandler : IRequestHandler<GetAllSectorsQuery, IEnumerable<SectorDto>> {
    private readonly ISectorRepository _repository;

    public GetAllSectorsQueryHandler(ISectorRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<SectorDto>> Handle(GetAllSectorsQuery request, CancellationToken cancellationToken) {
        var sectors = await _repository.GetAllAsync(cancellationToken);
        return sectors.Select(s => new SectorDto {
            Id = s.Id,
            Code = s.Code,
            NameAr = s.NameAr,
            NameEn = s.NameEn,
            DescriptionAr = s.DescriptionAr,
            DescriptionEn = s.DescriptionEn,
            DisplayOrder = s.DisplayOrder,
            IsActive = s.IsActive
        });
    }
}

public class GetActiveSectorsQueryHandler : IRequestHandler<GetActiveSectorsQuery, IEnumerable<SectorDto>> {
    private readonly ISectorRepository _repository;

    public GetActiveSectorsQueryHandler(ISectorRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<SectorDto>> Handle(GetActiveSectorsQuery request, CancellationToken cancellationToken) {
        var sectors = await _repository.GetActiveAsync(cancellationToken);
        return sectors.Select(s => new SectorDto {
            Id = s.Id,
            Code = s.Code,
            NameAr = s.NameAr,
            NameEn = s.NameEn,
            DescriptionAr = s.DescriptionAr,
            DescriptionEn = s.DescriptionEn,
            DisplayOrder = s.DisplayOrder,
            IsActive = s.IsActive
        });
    }
}

public class GetSectorByIdQueryHandler : IRequestHandler<GetSectorByIdQuery, SectorDto?> {
    private readonly ISectorRepository _repository;

    public GetSectorByIdQueryHandler(ISectorRepository repository) {
        _repository = repository;
    }

    public async Task<SectorDto?> Handle(GetSectorByIdQuery request, CancellationToken cancellationToken) {
        var sector = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (sector == null)
            return null;

        return new SectorDto {
            Id = sector.Id,
            Code = sector.Code,
            NameAr = sector.NameAr,
            NameEn = sector.NameEn,
            DescriptionAr = sector.DescriptionAr,
            DescriptionEn = sector.DescriptionEn,
            DisplayOrder = sector.DisplayOrder,
            IsActive = sector.IsActive
        };
    }
}

// SubSector Query Handlers
public class GetAllSubSectorsQueryHandler : IRequestHandler<GetAllSubSectorsQuery, IEnumerable<SubSectorDto>> {
    private readonly ISubSectorRepository _repository;

    public GetAllSubSectorsQueryHandler(ISubSectorRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<SubSectorDto>> Handle(GetAllSubSectorsQuery request, CancellationToken cancellationToken) {
        var subSectors = await _repository.GetAllAsync(cancellationToken);
        return subSectors.Select(s => new SubSectorDto {
            Id = s.Id,
            SectorId = s.SectorId,
            Code = s.Code,
            NameAr = s.NameAr,
            NameEn = s.NameEn,
            DescriptionAr = s.DescriptionAr,
            DescriptionEn = s.DescriptionEn,
            DisplayOrder = s.DisplayOrder,
            IsActive = s.IsActive
        });
    }
}

public class GetActiveSubSectorsQueryHandler : IRequestHandler<GetActiveSubSectorsQuery, IEnumerable<SubSectorDto>> {
    private readonly ISubSectorRepository _repository;

    public GetActiveSubSectorsQueryHandler(ISubSectorRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<SubSectorDto>> Handle(GetActiveSubSectorsQuery request, CancellationToken cancellationToken) {
        var subSectors = await _repository.GetActiveAsync(cancellationToken);
        return subSectors.Select(s => new SubSectorDto {
            Id = s.Id,
            SectorId = s.SectorId,
            Code = s.Code,
            NameAr = s.NameAr,
            NameEn = s.NameEn,
            DescriptionAr = s.DescriptionAr,
            DescriptionEn = s.DescriptionEn,
            DisplayOrder = s.DisplayOrder,
            IsActive = s.IsActive
        });
    }
}

public class GetSubSectorsBySectorIdQueryHandler : IRequestHandler<GetSubSectorsBySectorIdQuery, IEnumerable<SubSectorDto>> {
    private readonly ISubSectorRepository _repository;

    public GetSubSectorsBySectorIdQueryHandler(ISubSectorRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<SubSectorDto>> Handle(GetSubSectorsBySectorIdQuery request, CancellationToken cancellationToken) {
        var subSectors = await _repository.GetBySectorIdAsync(request.SectorId, cancellationToken);
        return subSectors.Select(s => new SubSectorDto {
            Id = s.Id,
            SectorId = s.SectorId,
            Code = s.Code,
            NameAr = s.NameAr,
            NameEn = s.NameEn,
            DescriptionAr = s.DescriptionAr,
            DescriptionEn = s.DescriptionEn,
            DisplayOrder = s.DisplayOrder,
            IsActive = s.IsActive
        });
    }
}

// AssetType Query Handlers
public class GetAllAssetTypesQueryHandler : IRequestHandler<GetAllAssetTypesQuery, IEnumerable<AssetTypeDto>> {
    private readonly IAssetTypeRepository _repository;

    public GetAllAssetTypesQueryHandler(IAssetTypeRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<AssetTypeDto>> Handle(GetAllAssetTypesQuery request, CancellationToken cancellationToken) {
        var assetTypes = await _repository.GetAllAsync(cancellationToken);
        return assetTypes.Select(a => new AssetTypeDto {
            Id = a.Id,
            Code = a.Code,
            NameAr = a.NameAr,
            NameEn = a.NameEn,
            DescriptionAr = a.DescriptionAr,
            DescriptionEn = a.DescriptionEn,
            DisplayOrder = a.DisplayOrder,
            IsActive = a.IsActive
        });
    }
}

public class GetActiveAssetTypesQueryHandler : IRequestHandler<GetActiveAssetTypesQuery, IEnumerable<AssetTypeDto>> {
    private readonly IAssetTypeRepository _repository;

    public GetActiveAssetTypesQueryHandler(IAssetTypeRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<AssetTypeDto>> Handle(GetActiveAssetTypesQuery request, CancellationToken cancellationToken) {
        var assetTypes = await _repository.GetActiveAsync(cancellationToken);
        return assetTypes.Select(a => new AssetTypeDto {
            Id = a.Id,
            Code = a.Code,
            NameAr = a.NameAr,
            NameEn = a.NameEn,
            DescriptionAr = a.DescriptionAr,
            DescriptionEn = a.DescriptionEn,
            DisplayOrder = a.DisplayOrder,
            IsActive = a.IsActive
        });
    }
}

// UnitOfMeasurement Query Handlers
public class GetAllUnitsOfMeasurementQueryHandler : IRequestHandler<GetAllUnitsOfMeasurementQuery, IEnumerable<UnitOfMeasurementDto>> {
    private readonly IUnitOfMeasurementRepository _repository;

    public GetAllUnitsOfMeasurementQueryHandler(IUnitOfMeasurementRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<UnitOfMeasurementDto>> Handle(GetAllUnitsOfMeasurementQuery request, CancellationToken cancellationToken) {
        var uoms = await _repository.GetAllAsync(cancellationToken);
        return uoms.Select(u => new UnitOfMeasurementDto {
            Id = u.Id,
            Code = u.Code,
            NameAr = u.NameAr,
            NameEn = u.NameEn,
            Symbol = u.Symbol,
            DisplayOrder = u.DisplayOrder,
            IsActive = u.IsActive
        });
    }
}

public class GetActiveUnitsOfMeasurementQueryHandler : IRequestHandler<GetActiveUnitsOfMeasurementQuery, IEnumerable<UnitOfMeasurementDto>> {
    private readonly IUnitOfMeasurementRepository _repository;

    public GetActiveUnitsOfMeasurementQueryHandler(IUnitOfMeasurementRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<UnitOfMeasurementDto>> Handle(GetActiveUnitsOfMeasurementQuery request, CancellationToken cancellationToken) {
        var uoms = await _repository.GetActiveAsync(cancellationToken);
        return uoms.Select(u => new UnitOfMeasurementDto {
            Id = u.Id,
            Code = u.Code,
            NameAr = u.NameAr,
            NameEn = u.NameEn,
            Symbol = u.Symbol,
            DisplayOrder = u.DisplayOrder,
            IsActive = u.IsActive
        });
    }
}