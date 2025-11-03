using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.InfraBase.Apis.Common;
using PartnersHub.InfraBase.Application.InfraRequests.Queries;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Apis.Controllers;

/// <summary>
/// Controller for dashboard statistics and summaries
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController : BaseApiController {
    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
        : base(mediator, logger) {
    }

    /// <summary>
    /// Gets dashboard statistics for current user's requests
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Dashboard statistics</returns>
    [HttpGet("my-stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetMyDashboardStats() {
        var userId = GetUserId();
        var query = new GetMyDashboardStatsQuery { UserId = userId };
        var stats = await Mediator.Send(query);
        return Ok(stats);
    }

    /// <summary>
    /// Gets dashboard statistics for company requests
    /// </summary>
    /// <param name="companyId">Company ID</param>
    /// <returns>Dashboard statistics</returns>
    [HttpGet("team-stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetTeamDashboardStats() {
        var companyId = GetCompanyId();
        var query = new GetTeamDashboardStatsQuery { CompanyId = companyId };
        var stats = await Mediator.Send(query);
        return Ok(stats);
    }

    /// <summary>
    /// Gets paginated list of user's own REQUESTS (not change requests) with filters
    /// Change requests are handled by separate endpoints
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="status">Filter by status (optional)</param>
    /// <param name="searchTerm">Search term (max 500 characters)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="sortBy">Sort by column name (optional)</param>
    /// <param name="sortDescending">Sort descending (default: true)</param>
    /// <returns>Paginated list of requests</returns>
    [HttpGet("my-requests")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<RequestSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<RequestSummaryDto>>>> GetMyRequests(
        [FromQuery] RequestStatus? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true) {
        
        // Validate search term length
        if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length > 500) {
            return BadRequest<PaginatedResult<RequestSummaryDto>>("Search term cannot exceed 500 characters");
        }

        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max page size

        var userId = GetUserId();
        var query = new GetMyRequestsQuery {
            UserId = userId,
            Status = status,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets paginated list of team REQUESTS 
    /// </summary>
    /// <param name="companyId">Company ID</param>
    /// <param name="status">Filter by status (optional)</param>
    /// <param name="searchTerm">Search term (max 500 characters)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="sortBy">Sort by column name (optional)</param>
    /// <param name="sortDescending">Sort descending (default: true)</param>
    /// <returns>Paginated list of team requests</returns>
    [HttpGet("team-requests")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<RequestSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<RequestSummaryDto>>>> GetTeamRequests(
        [FromQuery] RequestStatus? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true) {
        
        // Validate search term length
        if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length > 500) {
            return BadRequest<PaginatedResult<RequestSummaryDto>>("Search term cannot exceed 500 characters");
        }

        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max page size

        var companyId = GetCompanyId();
        var query = new GetTeamRequestsQuery {
            CompanyId = companyId,
            Status = status,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }
}
