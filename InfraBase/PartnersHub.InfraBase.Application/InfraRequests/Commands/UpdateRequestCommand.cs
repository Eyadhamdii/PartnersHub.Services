using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to update an entire request including basic info, items, and financial distributions
/// This allows updating a complete request structure in a single API call
/// Only works for Draft, ChangeRequested, or Rejected requests
/// </summary>
public record UpdateRequestCommand : IRequest<bool> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }
    
    // Basic Info (optional - only update if provided)
    public string? ProjectName { get; init; }
    public string? ProjectDescription { get; init; }
    public Guid? SectorId { get; init; }
    public Guid? SubSectorId { get; init; }
    public Guid? AssetTypeId { get; init; }
    public string? AssetTypeOtherDescription { get; init; }
    public TenderingStage? TenderingStage { get; init; }
    public FundingModel? FundingModel { get; init; }
    
    /// <summary>
    /// Complete list of items for the request
    /// Items not in this list will be removed
    /// Items with ID will be updated, items without ID will be added
    /// </summary>
    public List<UpdateRequestItemDto> Items { get; init; } = new();
}

/// <summary>
/// DTO for updating an item (includes ID for existing items)
/// </summary>
public record UpdateRequestItemDto {
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
    public List<UpdateFinancialDistributionDto> FinancialDistributions { get; init; } = new();
}

/// <summary>
/// DTO for updating a financial distribution
/// </summary>
public record UpdateFinancialDistributionDto {
    /// <summary>
    /// Distribution ID - if provided, updates existing; if null, creates new
    /// </summary>
    public Guid? Id { get; init; }
    public AmountType AmountType { get; init; }
    public int Year { get; init; }
    public decimal Amount { get; init; }
}
