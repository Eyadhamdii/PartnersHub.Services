using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.InfraBase.Apis.Common;
using PartnersHub.InfraBase.Application.Common.Models;
using PartnersHub.InfraBase.Application.InfraRequests.Commands;
using PartnersHub.InfraBase.Application.InfraRequests.Queries;
using PartnersHub.InfraBase.Domain.Enums;
using PartnersHub.InfraBase.Application.Common.Interfaces;

namespace PartnersHub.InfraBase.Apis.Controllers;

/// <summary>
/// Controller for managing infrastructure requests
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InfraRequestsController : BaseApiController {
    public InfraRequestsController(IMediator mediator, ILogger<InfraRequestsController> logger)
        : base(mediator, logger) {
    }

    /// <summary>
    /// Saves a request as draft (unified endpoint for both create and update)
    /// If RequestId is null/empty, creates new request
    /// If RequestId is provided, updates existing request
    /// Always results in Draft status
    /// </summary>
    /// <param name="command">Request data including items and distributions</param>
    /// <returns>ID of the request (newly created or updated)</returns>
    [HttpPost("save-draft")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> SaveRequestAsDraft([FromBody] SaveRequestAsDraftCommand command) {
        var requestId = await Mediator.Send(command);
        
        var isCreate = !command.RequestId.HasValue || command.RequestId.Value == Guid.Empty;
        var message = isCreate 
            ? "Request created as draft successfully"
            : "Request updated as draft successfully";
        
        return Ok(requestId, message);
    }

    /// <summary>
    /// Creates a new infra request with items and financial distributions
    /// Request is created as Draft status 
    /// </summary>
    /// <param name="command">Complete request data including items and distributions</param>
    /// <returns>ID of the created request</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateRequest([FromBody] CreateRequestCommand command) {
        var requestId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRequestById), new { id = requestId }, 
            ApiResponse<Guid>.SuccessResponse(requestId, "Request created as draft successfully"));
    }

    /// <summary>
    /// Gets a request by ID with complete details including lookup descriptions
    /// Returns all request information, items, financial distributions, history, and attachments
    /// Includes Sector, SubSector, AssetType, and UOM names from ConfigurationHub
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <returns>Complete request details with lookup descriptions</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RequestDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RequestDto>>> GetRequestById(Guid id) {
        var query = new GetRequestByIdQuery { RequestId = id };
        var result = await Mediator.Send(query);
        
        if (result == null) {
            return NotFound<RequestDto>($"Request with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all requests with optional status filter and pagination
    /// Supports both pageNumber (1-based) and pageIndex (0-based) for backwards compatibility
    /// When companyId or requestType is provided, returns lightweight summary DTOs without navigation collections
    /// Supports multiple status filters separated by comma (e.g., status=Draft,PendingPcAdminApproval)
    /// </summary>
    /// <param name="status">Filter by status (optional, supports comma-separated values)</param>
    /// <param name="pageNumber">Page number (default: 1, 1-based)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="pageIndex">Page index (0-based, overrides pageNumber if provided)</param>
    /// <param name="requestType">Request type filter (use "request" for lightweight response)</param>
    /// <param name="companyId">Company ID filter (triggers lightweight response)</param>
    /// <returns>Paginated list of requests (full or summary depending on filters)</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<RequestSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<RequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRequests(
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? pageIndex = null,
        [FromQuery] string? requestType = null,
        [FromQuery] Guid? companyId = null) {
        
        // Support both pageNumber (1-based) and pageIndex (0-based) for backwards compatibility
        if (pageIndex.HasValue) {
            // Use pageIndex if provided (0-based, convert to 1-based pageNumber)
            pageNumber = pageIndex.Value + 1;
        }
        
        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max page size
        
        // Parse status parameter (supports comma-separated values)
        RequestStatus? singleStatus = null;
        List<RequestStatus>? multipleStatuses = null;
        
        if (!string.IsNullOrWhiteSpace(status)) {
            var statusValues = status.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            if (statusValues.Length == 1) {
                // Single status
                if (Enum.TryParse<RequestStatus>(statusValues[0], true, out var parsedStatus)) {
                    singleStatus = parsedStatus;
                } else {
                    return new BadRequestObjectResult(ApiResponse<object>.FailureResponse($"Invalid status value: {statusValues[0]}"));
                }
            } else if (statusValues.Length > 1) {
                // Multiple statuses
                multipleStatuses = new List<RequestStatus>();
                foreach (var statusValue in statusValues) {
                    if (Enum.TryParse<RequestStatus>(statusValue, true, out var parsedStatus)) {
                        multipleStatuses.Add(parsedStatus);
                    } else {
                        return new BadRequestObjectResult(ApiResponse<object>.FailureResponse($"Invalid status value: {statusValue}"));
                    }
                }
            }
        }
        
        // If company filter is provided or requestType is "request", use lightweight summary query
        if (companyId.HasValue || requestType == "request") {
            var summaryQuery = new GetRequestsSummaryQuery {
                CompanyId = companyId,
                Status = singleStatus,
                Statuses = multipleStatuses,
                PageIndex = pageNumber - 1,
                PageSize = pageSize
            };
            
            var (items, totalCount) = await Mediator.Send(summaryQuery);
            
            // Return lightweight summary response with pagination metadata
            var response = new PaginatedResult<RequestSummaryDto> {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            return new OkObjectResult(ApiResponse<PaginatedResult<RequestSummaryDto>>.SuccessResponse(response));
        }
        
        // Otherwise use full query (for backwards compatibility)
        var query = new GetAllRequestsQuery { 
            Status = singleStatus,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await Mediator.Send(query);
        return new OkObjectResult(ApiResponse<PaginatedList<RequestDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets requests by user ID with pagination
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <returns>Paginated list of user's requests</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<RequestDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<RequestDto>>>> GetRequestsByUser(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10) {
        
        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max page size
        
        var query = new GetRequestsByUserQuery { 
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Submits a request for approval with optional updates
    /// Allows updating request data (basic info, items, distributions) before submitting
    /// If no updates provided, submits the request as-is
    /// All requests go to PC Admin for approval first
    /// </summary>
    /// <param name="requestId">Request ID from route</param>
    /// <param name="command">Submit command with optional request updates</param>
    /// <returns>Request code</returns>
    [HttpPost("{requestId}/submit")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<string>>> SubmitRequest(
        Guid requestId,
        [FromBody] SubmitRequestCommand command) {
        if (command.RequestId != requestId) {
            return BadRequest<string>("Request ID mismatch");
        }
        
        var userId = GetUserId();
        var commandWithUser = command with { UserId = userId };
        var requestCode = await Mediator.Send(commandWithUser);
        return Ok(requestCode);
    }

    /// <summary>
    /// Accepts a request by PC Admin
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="userId">User ID performing the action</param>
    /// <returns>Success result</returns>
    [HttpPost("{requestId}/pc-admin-accept")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> AcceptRequestByPcAdmin(Guid requestId) {
        var userId = GetUserId();
        var command = new AcceptRequestByPcAdminCommand { 
            RequestId = requestId, 
            UserId = userId 
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Rejects a request by PC Admin
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="command">Rejection data including user ID and reason</param>
    /// <returns>Success result</returns>
    [HttpPost("{requestId}/pc-admin-reject")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> RejectRequestByPcAdmin(Guid requestId, [FromBody] RejectRequestByPcAdminCommand command) {
        // Ensure the requestId from route matches the command
        if (command.RequestId != requestId) {
            return BadRequest<bool>("Request ID mismatch");
        }
        
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Accepts a request by Infrabase Admin
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="userId">Infrabase Admin user ID</param>
    /// <returns>Success result</returns>
    [HttpPost("{requestId}/infrabase-admin-accept")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> AcceptRequestByInfrabaseAdmin(
        Guid requestId,
        [FromQuery] Guid userId) {
        var command = new AcceptRequestByInfrabaseAdminCommand { 
            RequestId = requestId, 
            UserId = userId 
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Rejects a request by Infrabase Admin
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="command">Rejection data including user ID and reason</param>
    /// <returns>Success result</returns>
    [HttpPost("{requestId}/infrabase-admin-reject")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> RejectRequestByInfrabaseAdmin(
        Guid requestId,
        [FromBody] RejectRequestByInfrabaseAdminCommand command) {
        // Ensure the requestId from route matches the command
        if (command.RequestId != requestId) {
            return BadRequest<bool>("Request ID mismatch");
        }
        
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Infrabase Admin requests changes to the request instead of rejecting
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="command">Change request data including description</param>
    /// <returns>Success result</returns>
    [HttpPost("{requestId}/infrabase-admin-request-changes")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> RequestChangesByInfrabaseAdmin(
        Guid requestId,
        [FromBody] RequestChangesByInfrabaseAdminCommand command) {
        if (command.RequestId != requestId) {
            return BadRequest<bool>("Request ID mismatch");
        }
        
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates complete request including basic info, items, and financial distributions
    /// This replaces the entire request structure - items/distributions not in the request will be removed
    /// Only works for Draft, ChangeRequested, or Rejected requests
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="command">Complete request update data</param>
    /// <returns>Success result</returns>
    [HttpPut("{requestId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateRequest(
        Guid requestId,
        [FromBody] UpdateRequestCommand command) {
        // Ensure the requestId from route matches the command
        if (command.RequestId != requestId) {
            return BadRequest<bool>("Request ID mismatch");
        }
        
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a draft request
    /// Only Draft requests created by the user can be deleted
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="userId">User ID performing the deletion</param>
    /// <returns>Success result</returns>
    [HttpDelete("{requestId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRequest(
        Guid requestId,
        [FromQuery] Guid userId) {
        var command = new DeleteRequestCommand { 
            RequestId = requestId,
            UserId = userId
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Uploads an attachment to a request
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="command">Attachment data with SharePoint reference</param>
    /// <returns>ID of the created attachment</returns>
    [HttpPost("{requestId}/attachments")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> AddAttachment(
        Guid requestId,
        [FromBody] AddAttachmentCommand command) {
        // Ensure the requestId from route matches the command
        if (command.RequestId != requestId) {
            return BadRequest<Guid>("Request ID mismatch");
        }
        
        var attachmentId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRequestAttachments), new { requestId }, 
            ApiResponse<Guid>.SuccessResponse(attachmentId, "Attachment uploaded successfully"));
    }

    /// <summary>
    /// Gets all attachments for a request
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <returns>List of attachments</returns>
    [HttpGet("{requestId}/attachments")]
    [ProducesResponseType(typeof(ApiResponse<List<AttachmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<AttachmentDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<AttachmentDto>>>> GetRequestAttachments(Guid requestId) {
        var query = new GetRequestAttachmentsQuery { RequestId = requestId };
        var attachments = await Mediator.Send(query);
        return Ok(attachments);
    }

    /// <summary>
    /// Removes an attachment from a request
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="attachmentId">Attachment ID</param>
    /// <param name="userId">User ID performing the deletion</param>
    /// <returns>Success result</returns>
    [HttpDelete("{requestId}/attachments/{attachmentId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveAttachment(
        Guid requestId,
        Guid attachmentId,
        [FromQuery] Guid userId) {
        var command = new RemoveAttachmentCommand { 
            RequestId = requestId,
            AttachmentId = attachmentId,
            DeletedBy = userId
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    // ==================== REQUEST ITEMS MANAGEMENT ====================

    /// <summary>
    /// Adds an item to a request
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="command">Item data</param>
    /// <returns>ID of the created item</returns>
    [HttpPost("{requestId}/items")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> AddRequestItem(
        Guid requestId,
        [FromBody] AddRequestItemCommand command) {
        if (command.RequestId != requestId) {
            return BadRequest<Guid>("Request ID mismatch");
        }
        
        var userId = GetUserId();
        var commandWithUser = command with { UserId = userId };
        var itemId = await Mediator.Send(commandWithUser);
        return CreatedAtAction(nameof(GetRequestById), new { id = requestId }, 
            ApiResponse<Guid>.SuccessResponse(itemId, "Item added successfully"));
    }

    /// <summary>
    /// Updates an item in a request
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="command">Update data</param>
    /// <returns>Success result</returns>
    [HttpPut("{requestId}/items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateRequestItem(
        Guid requestId,
        Guid itemId,
        [FromBody] UpdateRequestItemCommand command) {
        if (command.RequestId != requestId || command.ItemId != itemId) {
            return BadRequest<bool>("Request ID or Item ID mismatch");
        }
        
        var userId = GetUserId();
        var commandWithUser = command with { UserId = userId };
        var result = await Mediator.Send(commandWithUser);
        return Ok(result);
    }

    /// <summary>
    /// Removes an item from a request
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="userId">User ID performing the action</param>
    /// <returns>Success result</returns>
    [HttpDelete("{requestId}/items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveRequestItem(
        Guid requestId,
        Guid itemId) {
        var userId = GetUserId();
        var command = new RemoveRequestItemCommand { 
            RequestId = requestId,
            ItemId = itemId,
            UserId = userId
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    // ==================== FINANCIAL DISTRIBUTIONS MANAGEMENT ====================

    /// <summary>
    /// Adds a financial distribution to an item
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="command">Distribution data</param>
    /// <returns>ID of the created distribution</returns>
    [HttpPost("{requestId}/items/{itemId}/distributions")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> AddFinancialDistribution(
        Guid requestId,
        Guid itemId,
        [FromBody] AddFinancialDistributionCommand command) {
        // Ensure the requestId and itemId from route match the command
        if (command.RequestId != requestId || command.ItemId != itemId) {
            return BadRequest<Guid>("Request ID or Item ID mismatch");
        }
        
        var distributionId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRequestById), new { id = requestId }, 
            ApiResponse<Guid>.SuccessResponse(distributionId, "Financial distribution added successfully"));
    }

    /// <summary>
    /// Updates a financial distribution amount
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="distributionId">Distribution ID</param>
    /// <param name="command">Update data</param>
    /// <returns>Success result</returns>
    [HttpPut("{requestId}/items/{itemId}/distributions/{distributionId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateFinancialDistribution(
        Guid requestId,
        Guid itemId,
        Guid distributionId,
        [FromBody] UpdateFinancialDistributionCommand command) {
        // Ensure the IDs from route match the command
        if (command.RequestId != requestId || command.ItemId != itemId || command.DistributionId != distributionId) {
            return BadRequest<bool>("ID mismatch");
        }
        
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Removes a financial distribution from an item
    /// </summary>
    /// <param name="requestId">Request ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="distributionId">Distribution ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("{requestId}/items/{itemId}/distributions/{distributionId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveFinancialDistribution(
        Guid requestId,
        Guid itemId,
        Guid distributionId) {
        var command = new RemoveFinancialDistributionCommand { 
            RequestId = requestId,
            ItemId = itemId,
            DistributionId = distributionId
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
