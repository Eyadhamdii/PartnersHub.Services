using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to accept a request by PC Admin
/// </summary>
public record AcceptRequestByPcAdminCommand : IRequest<bool> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
}

/// <summary>
/// Command to reject a request by PC Admin
/// </summary>
public record RejectRequestByPcAdminCommand : IRequest<bool> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}

/// <summary>
/// Command to submit or resubmit a request with optional updates
/// Allows updating request data (basic info, items, distributions) before submitting
/// All requests go to PC Admin for approval first
/// </summary>
public record SubmitRequestCommand : IRequest<string> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
    
    // Optional: Update basic info before submitting
    public string? ProjectName { get; init; }
    public string? ProjectDescription { get; init; }
    public Guid? SectorId { get; init; }
    public Guid? SubSectorId { get; init; }
    public Guid? AssetTypeId { get; init; }
    public string? AssetTypeOtherDescription { get; init; }
    public TenderingStage? TenderingStage { get; init; }
    public FundingModel? FundingModel { get; init; }
    
    // Optional: Construction details
    public int? StartConstructionQuarter { get; init; }
    public int? StartConstructionYear { get; init; }
    public int? EndConstructionQuarter { get; init; }
    public int? EndConstructionYear { get; init; }
    
    /// <summary>
    /// Optional: Complete list of items for the request
    /// If provided, items not in this list will be removed
    /// Items with ID will be updated, items without ID will be added
    /// If not provided or empty, existing items are kept as-is
    /// </summary>
    public List<SubmitRequestItemDto>? Items { get; init; }
}

/// <summary>
/// DTO for updating an item during submit
/// </summary>
public record SubmitRequestItemDto {
    /// <summary>
    /// Item ID - if provided, updates existing item; if null, creates new item
    /// </summary>
    public Guid? Id { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public Guid UomId { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    
    /// <summary>
    /// Complete list of financial distributions for this item
    /// Distributions not in this list will be removed
    /// </summary>
    public List<SubmitFinancialDistributionDto> FinancialDistributions { get; init; } = new();
}

/// <summary>
/// DTO for updating a financial distribution during submit
/// </summary>
public record SubmitFinancialDistributionDto {
    public Guid? Id { get; init; }
    public AmountType AmountType { get; init; }
    /// <summary>
    /// Year for this distribution. Accepts:
    /// - Offset (1-99): Year 1, Year 2, etc.
    /// - Full year (2000-2099): 2024, 2025, etc.
    /// </summary>
    public int Year { get; init; }
    public decimal Amount { get; init; }
}

/// <summary>
/// Command to accept a request by Infrabase Admin
/// </summary>
public record AcceptRequestByInfrabaseAdminCommand : IRequest<bool> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
}

/// <summary>
/// Command to reject a request by Infrabase Admin
/// </summary>
public record RejectRequestByInfrabaseAdminCommand : IRequest<bool> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}

/// <summary>
/// Command to request changes to a request by Infrabase Admin
/// </summary>
public record RequestChangesByInfrabaseAdminCommand : IRequest<bool> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
    public string ChangeRequestDescription { get; init; } = string.Empty;
}
