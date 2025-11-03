namespace PartnersHub.Synergy.Application.Dashboard.DTOs;

/// <summary>
/// User's opportunity submission for dashboard
/// </summary>
public class UserOpportunitySubmissionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime SubmissionDate { get; set; }
    public string CollaborationType { get; set; } = null!;
    public string Sector { get; set; } = null!;
    public string Status { get; set; } = null!; // Pending, Published, Returned
}

/// <summary>
/// User's success story submission for dashboard
/// </summary>
public class UserSuccessStorySubmissionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime SubmissionDate { get; set; }
    public string Type { get; set; } = null!; // Partnership / Collaboration / Joint Venture
    public string SubmissionStatus { get; set; } = null!; // Pending, Published, Returned
}
