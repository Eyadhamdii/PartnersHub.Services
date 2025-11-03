using Microsoft.AspNetCore.Mvc;
using PartnersHub.Synergy.Apis.Controllers.Base;
using PartnersHub.Synergy.Application.SynergyCompany.Queries;
using MediatR;

namespace PartnersHub.Synergy.Apis.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SynergyCompaniesController : ApiBaseController<SynergyCompaniesController>
{
    private readonly IMediator _mediator;

    public SynergyCompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<RegisteredCompaniesResultDto>> GetRegisteredCompanies(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sectorIds = null,
        [FromQuery] string? countries = null,
        [FromQuery] string? cities = null)
    {
        var query = new GetRegisteredCompaniesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            SectorIds = ParseGuids(sectorIds),
            Countries = ParseCommaSeparated(countries),
            Cities = ParseCommaSeparated(cities)
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDetailsDto>> GetCompanyDetails(Guid id)
    {
        var query = new GetCompanyDetailsQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"Company with ID {id} not found" });

        return Ok(result);
    }


    private List<string>? ParseCommaSeparated(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private List<Guid>? ParseGuids(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => Guid.TryParse(s, out var guid) ? guid : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();
    }
}
