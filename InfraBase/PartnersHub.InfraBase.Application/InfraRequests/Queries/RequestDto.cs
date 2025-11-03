using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

public record RequestDto {
    public Guid Id { get; init; }
    public string? RequestCode { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public string? ProjectDescription { get; init; }
    
    // Sector
    public Guid SectorId { get; init; }
    public string? SectorName { get; init; }  // From ConfigurationHub
    
    // SubSector
    public Guid SubSectorId { get; init; }
    public string? SubSectorName { get; init; }  // From ConfigurationHub
    
    // AssetType
    public Guid AssetTypeId { get; init; }
    public string? AssetTypeName { get; init; }  // From ConfigurationHub
    public string? AssetTypeOtherDescription { get; init; }
    
    public TenderingStage TenderingStage { get; init; }
    public FundingModel FundingModel { get; init; }
    
    // Construction Details
    public int? StartConstructionQuarter { get; init; }
    public int? StartConstructionYear { get; init; }
    public int? EndConstructionQuarter { get; init; }
    public int? EndConstructionYear { get; init; }
    
    public RequestStatus Status { get; init; }
    public decimal TotalRequestAmount { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public string? RejectionReason { get; init; }
    public Guid? RejectedBy { get; init; }
    public DateTime? RejectedAt { get; init; }
    public Guid? ApprovedBy { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public Guid? CompanyId { get; init; }
    public string? CompanyName { get; init; }
    public List<RequestItemDto> Items { get; init; } = new();
    public List<RequestHistoryDto> History { get; init; } = new();
    public List<AttachmentDto> Attachments { get; init; } = new();
}

public record RequestItemDto {
    public Guid Id { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public Guid UomId { get; init; }
    public string? UomName { get; init; }  // From ConfigurationHub
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalAmount { get; init; }
    public List<FinancialDistributionDto> FinancialDistributions { get; init; } = new();
}

public record FinancialDistributionDto {
    public Guid Id { get; init; }
    public AmountType AmountType { get; init; }
    public int Year { get; init; }
    public decimal Amount { get; init; }
}

public record RequestHistoryDto {
    public Guid Id { get; init; }
    public RequestStatus Status { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? Comments { get; init; }
    public Guid PerformedBy { get; init; }
    public DateTime PerformedAt { get; init; }
    public string? FieldsChanged { get; init; }
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
}
