namespace PartnersHub.Synergy.Application.Opportunities.DTOs;

/// <summary>
/// Result DTO for opportunity search listing
/// </summary>
public class SearchOpportunitiesResultDto
{
    public List<OpportunitySearchCardDto> Opportunities { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    // Available filters for frontend
    public List<FilterOptionDto> AvailableSectors { get; set; } = new();
    public List<FilterOptionDto> AvailableOpportunityTypes { get; set; } = new();
    public List<FilterOptionDto> AvailableThematicAreas { get; set; } = new();
    public List<FilterOptionDto> AvailableCollaborationRequirements { get; set; } = new();
    public List<FilterOptionDto> AvailableExpectedOutcomes { get; set; } = new();
    public List<string> AvailableStatuses { get; set; } = new();
}

/// <summary>
/// Card DTO for opportunity search results
/// </summary>
public class OpportunitySearchCardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    
    // Company info
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string? CompanyLogoUrl { get; set; }
    
    // Classification
    public int OpportunityTypeId { get; set; }
    public string OpportunityTypeName { get; set; } = null!;
    public int ThematicAreaId { get; set; }
    public string ThematicAreaName { get; set; } = null!;
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = null!;
    
    // Collaboration details
    public List<string> CollaborationRequirements { get; set; } = new();
    public List<string> ExpectedOutcomes { get; set; } = new();
    public int CollaboratedCompaniesCount { get; set; }
    
    // Timeline
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Filter option for dropdowns
/// </summary>
public class FilterOptionDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Count { get; set; }
}
