using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.InfraBase.Apis.Common;
using System.Security.Claims;

namespace PartnersHub.InfraBase.Apis.Controllers;

/// <summary>
/// Base controller with common functionality
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase {
    protected readonly IMediator Mediator;
    protected readonly ILogger Logger;

    protected BaseApiController(IMediator mediator, ILogger logger) {
        Mediator = mediator;
        Logger = logger;
    }

    protected Guid GetUserId() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("ContactId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)) {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        
        return userId;
    }

    protected Guid GetCompanyId() {
        var companyIdClaim = User.FindFirst("CompanyId")?.Value;
        
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId)) {
            throw new UnauthorizedAccessException("Company ID not found in token");
        }
        
        return companyId;
    }

    protected string GetUserEmail() {
        return User.FindFirst(ClaimTypes.Email)?.Value 
            ?? User.FindFirst("Email")?.Value 
            ?? throw new UnauthorizedAccessException("Email not found in token");
    }

    /// <summary>
    /// Returns success response with data
    /// </summary>
    protected ActionResult<ApiResponse<T>> Ok<T>(T data, string? message = null) {
        return base.Ok(ApiResponse<T>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Returns bad request with error (generic)
    /// </summary>
    protected ActionResult<ApiResponse<T>> BadRequest<T>(string error) {
        return base.BadRequest(ApiResponse<T>.FailureResponse(error));
    }

    /// <summary>
    /// Returns bad request with multiple errors (generic)
    /// </summary>
    protected ActionResult<ApiResponse<T>> BadRequest<T>(List<string> errors) {
        return base.BadRequest(ApiResponse<T>.FailureResponse(errors));
    }

    /// <summary>
    /// Returns not found response (generic)
    /// </summary>
    protected ActionResult<ApiResponse<T>> NotFound<T>(string message) {
        return base.NotFound(ApiResponse<T>.FailureResponse(message));
    }
}