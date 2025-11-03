using Microsoft.AspNetCore.Mvc;
using PartnersHub.Synergy.Apis.Controllers.Base;

namespace PartnersHub.Synergy.Apis.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LookupController : ApiBaseController<LookupController>
{

    [HttpGet("collaboration-requirements")]
    public async Task<ActionResult<ApiResponse>> GetCollaborationRequirements()
    {
        var query = new GetCollaborationRequirementsQuery();
        return await Execute(query);
    }

    [HttpGet("expected-outcomes")]
    public async Task<ActionResult<ApiResponse>> GetExpectedOutcomes()
    {
        var query = new GetExpectedOutcomesQuery();
        return await Execute(query);
    }


    [HttpGet("opportunity-types")]
    public async Task<ActionResult<ApiResponse>> GetOpportunityTypes()
    {
        var query = new GetOpportunityTypesQuery();
        return await Execute(query);
    }

    [HttpGet("thematic-areas")]
    public async Task<ActionResult<ApiResponse>> GetThematicAreas()
    {
        var query = new GetThematicAreasQuery();
        return await Execute(query);
    }
    [HttpGet("successs-story-statuses")]
    public async Task<ActionResult<ApiResponse>> GetSuccessStoryStatuses()
    {
        var query = new GetSuccessStoryStatusesQuery();
        return await Execute(query);
    }
    [HttpGet("successs-story-collaboration-statuses")]
    public async Task<ActionResult<ApiResponse>> GetSuccessStoryCollaborationStatuses()
    {
        var query = new SuccessStoryCollaborationStatusesQuery();
        return await Execute(query);
    }
    [HttpGet("successs-story-types")]
    public async Task<ActionResult<ApiResponse>> GetSuccessStoryTypes()
    {
        var query = new GetSuccessStoryTypeQuery();
        return await Execute(query);
    }
    [HttpGet("synergy-companies")]
    public async Task<ActionResult<ApiResponse>> GetSynergyCompanies()
    {
        var query = new GetSynergyCompanyQuery();
        return await Execute(query);
    }
}

