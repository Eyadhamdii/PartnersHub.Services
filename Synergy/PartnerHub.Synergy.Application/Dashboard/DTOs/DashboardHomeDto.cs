namespace PartnersHub.Synergy.Application.Dashboard.DTOs;

/// <summary>
/// Complete dashboard data - all home page statistics in one response
/// </summary>
public class DashboardHomeDto
{
    public DashboardKPIsDto KPIs { get; set; } = null!;
    public List<RecentOpportunityCardDto> RecentOpportunities { get; set; } = new();
    public List<RecentSuccessStoryCardDto> RecentSuccessStories { get; set; } = new();
    public List<RecentCompanyCardDto> RecentCompanies { get; set; } = new();
}
