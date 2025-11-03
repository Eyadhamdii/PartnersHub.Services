namespace PartnersHub.Synergy.Application.Dashboard.DTOs;

/// <summary>
/// Dashboard KPI statistics
/// </summary>
public class DashboardKPIsDto
{
    public PortfolioCompaniesKPI PortfolioCompanies { get; set; } = null!;
    public ActiveOpportunitiesKPI ActiveOpportunities { get; set; } = null!;
    public SuccessStoriesKPI SuccessStories { get; set; } = null!;
}

public class PortfolioCompaniesKPI
{
    /// <summary>
    /// Total portfolio companies registered across Synergy (all PIF portfolio companies)
    /// </summary>
    public int TotalRegistered { get; set; }

    /// <summary>
    /// Total count of distinct PCs the user's company has collaborated with
    /// </summary>
    public int YourCollaborations { get; set; }
}

public class ActiveOpportunitiesKPI
{
    /// <summary>
    /// Total active and completed opportunities across Synergy (all PIF portfolio companies)
    /// </summary>
    public int TotalAcrossSynergy { get; set; }

    /// <summary>
    /// Total active and completed opportunities created by the user's own company
    /// </summary>
    public int YourCompanyOpportunities { get; set; }
}

public class SuccessStoriesKPI
{
    /// <summary>
    /// Total published success stories across Synergy (all PIF portfolio companies)
    /// </summary>
    public int TotalPublished { get; set; }

    /// <summary>
    /// Total published success stories created by the user's own company
    /// </summary>
    public int YourCompanyStories { get; set; }
}
