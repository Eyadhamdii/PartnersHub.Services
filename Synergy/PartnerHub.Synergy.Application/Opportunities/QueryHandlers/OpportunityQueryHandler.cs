using MediatR;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Application.Models;
using PartnersHub.Synergy.Application.Opportunities.DTOs;
using PartnersHub.Synergy.Application.Opportunity.Queries;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Domain.Common;

public class GetOpportunityDetailsQueryHandler : IRequestHandler<GetOpportunityDetailsQuery, Result<OpportunityResponseDto>>
{
    private readonly IOpportunityRepository _opportunityRepository;
    private readonly ISynergyCompanyRepository _synergyCompanyRepository;
    public GetOpportunityDetailsQueryHandler(IOpportunityRepository opportunityRepository, ISynergyCompanyRepository synergyCompanyRepository)
    {
        _opportunityRepository = opportunityRepository;
        _synergyCompanyRepository = synergyCompanyRepository;
    }

    public async Task<Result<OpportunityResponseDto>> Handle(GetOpportunityDetailsQuery request, CancellationToken cancellationToken)
    {
        Opportunity opportunity = await _opportunityRepository.GetByIdAsync(request.Id, true,
        o => o.OpportunityType,
        o => o.ThematicArea,
        o => o.Sector,
        o => o.ExpectedOutcomes,
        o => o.CollaborationRequirements,
        o => o.OpportunityType,
        o => o.CollaboratedCompanies      ,                          o => o.RepresentativeInformation);


        List<SynergyCompany> associatedSynergyCompanies = await _synergyCompanyRepository.GetByIdsAsync(opportunity.CollaboratedCompanies.Select(cc => cc.SynergyCompanyId).ToList());

        if (opportunity == null)
        {
            return Result<OpportunityResponseDto>.Failure("Opportunity doesn't exist");
        }
        SynergyCompany creatorCompany = await _synergyCompanyRepository.GetByIdAsync(opportunity.CompanyId);
        return Result<OpportunityResponseDto>.Success(Helpers.CreateResponseObject(opportunity, associatedSynergyCompanies, creatorCompany));
    }


}
public class GetPaginatedOpportunityDetailsQueryHandler : IRequestHandler<GetPaginatedOpportunityDetailsQuery, Result<PaginatedList<OpportunityResponseDto>>>
{


    public GetPaginatedOpportunityDetailsQueryHandler()
    {

    }

    public async Task<Result<PaginatedList<OpportunityResponseDto>>> Handle(GetPaginatedOpportunityDetailsQuery request, CancellationToken cancellationToken)
    {
        return null;
    }



}
public class GetOpportunitiesByCompanyIdQueryHandler : IRequestHandler<GetOpportunitiesByCompanyIdQuery, Result<List<OpportunityResponseDto>>>
{
    private readonly IOpportunityRepository _opportunityRepository;
    private readonly ISynergyCompanyRepository _synergyCompanyRepository;
    public GetOpportunitiesByCompanyIdQueryHandler(IOpportunityRepository opportunityRepository, ISynergyCompanyRepository synergyCompanyRepository)
    {
        _opportunityRepository = opportunityRepository;
        _synergyCompanyRepository = synergyCompanyRepository;
    }

    public async Task<Result<List<OpportunityResponseDto>>> Handle(GetOpportunitiesByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        Dictionary<Opportunity, List<SynergyCompany>> opportunitiesCompaniesDictionary = await _opportunityRepository.GetOpportunitiesWithCompanies(request.CompanyId, true,
                                o => o.OpportunityType,
                                o => o.ThematicArea,
                                o => o.Sector,
                                o => o.CollaboratedCompanies,
                                o => o.ExpectedOutcomes,
                                o => o.CollaborationRequirements,
                                o => o.ThematicArea,
                                o => o.OpportunityType,
                                o => o.Description,
                                o => o.Title,
                                o => o.RepresentativeInformation);


        if (opportunitiesCompaniesDictionary == null || opportunitiesCompaniesDictionary.Count == default(int))
        {
            return Result<List<OpportunityResponseDto>>.Success(new List<OpportunityResponseDto>(), "No Opportunities Exist");
        }
        List<OpportunityResponseDto> opportunityDetailsList = new List<OpportunityResponseDto>();
        List<SynergyCompany> creatorCompanies = await _synergyCompanyRepository.GetByIdsAsync(opportunitiesCompaniesDictionary.Select(o => o.Key).Select(o => o.CompanyId).ToList());
        foreach (var entry in opportunitiesCompaniesDictionary)
        {
            opportunityDetailsList.Add(Helpers.CreateResponseObject(entry.Key, entry.Value, creatorCompanies?.FirstOrDefault(cc => cc.Id == entry.Key.CompanyId)));
        }
        return Result<List<OpportunityResponseDto>>.Success(opportunityDetailsList);

    }




}

internal static class Helpers
{
    internal static OpportunityResponseDto CreateResponseObject(Opportunity opportunity, List<SynergyCompany> associatedSynergyCompanies, SynergyCompany? creatorCompany)
    {
        var opportunityDetailsDto = new OpportunityResponseDto
        {
            CompanyId = opportunity.CompanyId,
            CompanyName = creatorCompany?.Name?.Value,
            Title = opportunity.Title.Value,
            Description = opportunity.Description.Value,
            TypeId = opportunity.OpportunityTypeId,
            TypeName = string.Concat(opportunity.OpportunityType.Name.ToString().Select(c => char.IsUpper(c) ?
                " " + c.ToString() : c.ToString())),
            Status = string.Concat(opportunity.Status.ToString().Select(c => char.IsUpper(c) ?
                " " + c.ToString() : c.ToString())),
            ThematicAreaId = opportunity.ThematicAreaId,
            ThematicAreaName = opportunity.ThematicArea.Name,
            SectorName = opportunity.Sector.Value,
            SectorId = opportunity.Sector.Id,
            CollaborationRationale = opportunity.CollaborationRationale,
            CollaborationRequirements = opportunity.CollaborationRequirements.Select(cr => new KeyValueDto(cr.Id, cr.Name)).ToList(),
            CollaborationRequirementOther = opportunity.CollaborationRequirementOther,
            ExpectedOutcomes = opportunity.ExpectedOutcomes?.Select(eo => new KeyValueDto(eo.Id, eo.Name)).ToList(),
            ExpectedOutcomeOther = opportunity.ExpectedOutcomeOther,
            StartDate = opportunity.StartDate,
            EndDate = opportunity.EndDate,
            RepresentaitveTitle = opportunity.RepresentativeInformation.Position,
            RepresentativeEmail = opportunity.RepresentativeInformation.Email,
            RepresentativeName = opportunity.RepresentativeInformation.Name,
            RepresentativePhone = opportunity.RepresentativeInformation.Phone,
            TermsAndConditionId = opportunity.TermsAndConditionId,
            CreatedBy = opportunity.CreatedBy,
            CollaboratedProfiles = associatedSynergyCompanies
                .Select(c => new GuidKeyValueDto(c.Id, c.Name.Value.ToString()))
                .ToList(),
        };
        return opportunityDetailsDto;
    }
}

