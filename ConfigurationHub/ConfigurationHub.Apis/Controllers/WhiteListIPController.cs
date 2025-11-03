using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;
using PartnersHub.ConfigurationHub.Application.WhiteListIPs.Commands;
using PartnersHub.ConfigurationHub.Application.WhiteListIPs.Queries;

namespace PartnersHub.ConfigurationHub.Apis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WhiteListIPsController : ControllerBase {
    private readonly IMediator _mediator;
    private readonly ILogger<WhiteListIPsController> _logger;

    public WhiteListIPsController(IMediator mediator, ILogger<WhiteListIPsController> logger) {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all whitelist IPs
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WhiteListIPDto>>> GetAll() {
        var query = new GetAllWhiteListIPsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get active whitelist IPs
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<WhiteListIPDto>>> GetActive() {
        var query = new GetActiveWhiteListIPsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get whitelist IP by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WhiteListIPDto>> GetById(Guid id) {
        var query = new GetWhiteListIPByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Check if an IP address is whitelisted
    /// </summary>
    [HttpGet("check/{ipAddress}")]
    public async Task<ActionResult<bool>> CheckIP(string ipAddress) {
        var query = new IsIPWhitelistedQuery { IPAddress = ipAddress };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new whitelist IP
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateWhiteListIPCommand command) {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update a whitelist IP
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateWhiteListIPCommand command) {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Delete a whitelist IP
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id) {
        var command = new DeleteWhiteListIPCommand { Id = id };
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}