using MediatR;
using Microsoft.AspNetCore.Http;
using PartnersHub.Synergy.Application.Opportunities.DTOs;
using PartnersHub.Synergy.Domain.Common;


public record CreateOpportunityCommand : IRequest<Result<Guid>>
{
    public Guid CompanyId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int TypeId { get; set; }
    public int ThematicAreaId { get; set; }
    public string SectorName { get; set; }
    public Guid SectorId { get; set; }
    public List<Guid> CollaboratedProfiles { get; set; }
    public string CollaborationRationale { get; set; }
    public List<int> CollaborationRequirements { get; set; }
    public string CollaborationRequirementOther { get; set; }
    public List<int> ExpectedOutcomes { get; set; }
    public string ExpectedOutcomeOther { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string RepresentativeName { get; set; }
    public string RepresentativeTitle { get; set; }
    public string RepresentativeEmail { get; set; }
    public string RepresentativePhone { get; set; }
    public Guid TermsAndConditionId { get; set; }
    public Guid? CreatedBy { get; set; }
}
