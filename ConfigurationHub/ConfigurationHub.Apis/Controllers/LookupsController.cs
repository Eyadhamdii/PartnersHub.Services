using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;
using PartnersHub.ConfigurationHub.Application.Lookups.Queries;

namespace PartnersHub.ConfigurationHub.Apis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController : ControllerBase {
    private readonly IMediator _mediator;
    private readonly ILogger<LookupsController> _logger;

    public LookupsController(IMediator mediator, ILogger<LookupsController> logger) {
        _mediator = mediator;
        _logger = logger;
    }

    #region Sectors

    /// <summary>
    /// Get all sectors
    /// </summary>
    [HttpGet("sectors")]
    public async Task<ActionResult<IEnumerable<SectorDto>>> GetAllSectors() {
        var query = new GetAllSectorsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get active sectors
    /// </summary>
    [HttpGet("sectors/active")]
    public async Task<ActionResult<IEnumerable<SectorDto>>> GetActiveSectors() {
        var query = new GetActiveSectorsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get sector by ID
    /// </summary>
    [HttpGet("sectors/{id:guid}")]
    public async Task<ActionResult<SectorDto>> GetSectorById(Guid id) {
        var query = new GetSectorByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    #endregion

    #region SubSectors

    /// <summary>
    /// Get all sub-sectors
    /// </summary>
    [HttpGet("subsectors")]
    public async Task<ActionResult<IEnumerable<SubSectorDto>>> GetAllSubSectors() {
        var query = new GetAllSubSectorsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get active sub-sectors
    /// </summary>
    [HttpGet("subsectors/active")]
    public async Task<ActionResult<IEnumerable<SubSectorDto>>> GetActiveSubSectors() {
        var query = new GetActiveSubSectorsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get sub-sectors by sector ID
    /// </summary>
    [HttpGet("sectors/{sectorId:guid}/subsectors")]
    public async Task<ActionResult<IEnumerable<SubSectorDto>>> GetSubSectorsBySectorId(Guid sectorId) {
        var query = new GetSubSectorsBySectorIdQuery { SectorId = sectorId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region AssetTypes

    /// <summary>
    /// Get all asset types
    /// </summary>
    [HttpGet("assettypes")]
    public async Task<ActionResult<IEnumerable<AssetTypeDto>>> GetAllAssetTypes() {
        var query = new GetAllAssetTypesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get active asset types
    /// </summary>
    [HttpGet("assettypes/active")]
    public async Task<ActionResult<IEnumerable<AssetTypeDto>>> GetActiveAssetTypes() {
        var query = new GetActiveAssetTypesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region UnitsOfMeasurement

    /// <summary>
    /// Get all units of measurement
    /// </summary>
    [HttpGet("uoms")]
    public async Task<ActionResult<IEnumerable<UnitOfMeasurementDto>>> GetAllUnitsOfMeasurement() {
        var query = new GetAllUnitsOfMeasurementQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get active units of measurement
    /// </summary>
    [HttpGet("uoms/active")]
    public async Task<ActionResult<IEnumerable<UnitOfMeasurementDto>>> GetActiveUnitsOfMeasurement() {
        var query = new GetActiveUnitsOfMeasurementQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion
}