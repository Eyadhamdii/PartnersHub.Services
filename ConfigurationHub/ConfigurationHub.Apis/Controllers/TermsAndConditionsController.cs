using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;
using PartnersHub.ConfigurationHub.Application.TermsAndConditions.Queries;
using PartnersHub.ConfigurationHub.Domain.Enums;

namespace PartnersHub.ConfigurationHub.Apis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TermsAndConditionsController : ControllerBase {
    private readonly IMediator _mediator;
    private readonly ILogger<TermsAndConditionsController> _logger;

    public TermsAndConditionsController(IMediator mediator, ILogger<TermsAndConditionsController> logger) {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all terms and conditions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TermsAndConditionDto>>> GetAll() {
        var query = new GetAllTermsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get terms and conditions by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TermsAndConditionDto>> GetById(Guid id) {
        var query = new GetTermsByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Get active terms and conditions by type
    /// </summary>
    [HttpGet("active/{type}")]
    public async Task<ActionResult<TermsAndConditionDto>> GetActiveByType(TermsType type) {
        var query = new GetActiveTermsByTypeQuery { Type = type };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"No active terms found for type: {type}");

        return Ok(result);
    }

    /// <summary>
    /// Get all terms and conditions by type
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<TermsAndConditionDto>>> GetByType(TermsType type) {
        var query = new GetTermsByTypeQuery { Type = type };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}