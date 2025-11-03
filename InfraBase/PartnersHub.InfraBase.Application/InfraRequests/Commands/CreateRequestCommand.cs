using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to create a new infrastructure request with items and financial distributions
/// This allows creating a complete request in a single API call
/// Request is created as Draft status
/// </summary>
public record CreateRequestCommand : IRequest<Guid> {
    public string ProjectName { get; init; } = string.Empty;
    public string? ProjectDescription { get; init; }
    public Guid SectorId { get; init; }
    public Guid SubSectorId { get; init; }
    public Guid AssetTypeId { get; init; }
    public string? AssetTypeOtherDescription { get; init; }
    public TenderingStage TenderingStage { get; init; }
    public FundingModel FundingModel { get; init; }
    public Guid CreatedBy { get; init; }
    public Guid? CompanyId { get; init; }
    public string? CompanyName { get; init; }
    
    /// <summary>
    /// Items to add to the request at creation
    /// </summary>
    public List<CreateRequestItemDto> Items { get; init; } = new();
}

/// <summary>
/// DTO for creating an item with financial distributions
/// </summary>
public record CreateRequestItemDto {
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public Guid UomId { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    
    /// <summary>
    /// Financial distributions for this item
    /// </summary>
    public List<CreateFinancialDistributionDto> FinancialDistributions { get; init; } = new();
}

/// <summary>
/// DTO for creating a financial distribution
/// Year can be provided as:
/// - Offset year (1-99): Represents year 1, year 2, etc. from project start
/// - Full year (2000-2099): Represents calendar year (e.g., 2025, 2026)
/// </summary>
public record CreateFinancialDistributionDto {
    public AmountType AmountType { get; init; }
    /// <summary>
    /// Year for this distribution. Accepts:
    /// - Offset (1-99): Year 1, Year 2, etc.
    /// - Full year (2000-2099): 2024, 2025, etc.
    /// </summary>
    public int Year { get; init; }
    public decimal Amount { get; init; }
}
