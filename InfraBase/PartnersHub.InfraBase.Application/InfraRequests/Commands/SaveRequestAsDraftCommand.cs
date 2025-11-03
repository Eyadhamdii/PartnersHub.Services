using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Unified command to save a request as draft (handles both create and update)
/// If RequestId is null or empty Guid, creates new request
/// If RequestId is provided, updates existing request
/// All saves result in Draft status
/// </summary>
public record SaveRequestAsDraftCommand : IRequest<Guid> {
    /// <summary>
    /// Request ID - null/empty for new request, valid ID for update
    /// </summary>
    public Guid? RequestId { get; init; }
    
    public string ProjectName { get; init; } = string.Empty;
    public string? ProjectDescription { get; init; }
    public Guid SectorId { get; init; }
    public Guid SubSectorId { get; init; }
    public Guid AssetTypeId { get; init; }
    public string? AssetTypeOtherDescription { get; init; }
    public TenderingStage TenderingStage { get; init; }
    public FundingModel FundingModel { get; init; }
    
    // Construction Details
    public int? StartConstructionQuarter { get; init; }
    public int? StartConstructionYear { get; init; }
    public int? EndConstructionQuarter { get; init; }
    public int? EndConstructionYear { get; init; }
    
    public Guid UserId { get; init; }  // Used as CreatedBy for new, UpdatedBy for existing
    public Guid? CompanyId { get; init; }
    public string? CompanyName { get; init; }
    
    /// <summary>
    /// Items for the request
    /// For create: All items are new
    /// For update: Items with ID are updated, items without ID are added, items not in list are removed
    /// </summary>
    public List<SaveRequestItemDto> Items { get; init; } = new();
}

/// <summary>
/// DTO for saving an item (works for both create and update)
/// </summary>
public record SaveRequestItemDto {
    /// <summary>
    /// Item ID - null for new item, valid ID for update
    /// </summary>
    public Guid? Id { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public Guid UomId { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    
    /// <summary>
    /// Financial distributions for this item
    /// All distributions are replaced
    /// </summary>
    public List<SaveFinancialDistributionDto> FinancialDistributions { get; init; } = new();
}

/// <summary>
/// DTO for saving a financial distribution
/// </summary>
public record SaveFinancialDistributionDto {
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
