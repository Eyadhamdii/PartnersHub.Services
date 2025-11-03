using MediatR;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Application.Models;
using PartnersHub.Synergy.Domain.Common;

public class GetCollaborationRequirementsQueryHandler : IRequestHandler<GetCollaborationRequirementsQuery, IEnumerable<KeyValueDto>>
{
    private readonly ICollaborationRequirementRepository _repository;

    public GetCollaborationRequirementsQueryHandler(ICollaborationRequirementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(GetCollaborationRequirementsQuery request, CancellationToken cancellationToken)
    {
        var collaborationRequirements = await _repository.GetAllAsync();
        return collaborationRequirements.Select(cr => new KeyValueDto(cr.Id, cr.Name));
    }
}

public class GetExpectedOutcomesQueryHandler : IRequestHandler<GetExpectedOutcomesQuery, IEnumerable<KeyValueDto>>
{
    private readonly IExpectedOutcomesRepository _repository;

    public GetExpectedOutcomesQueryHandler(IExpectedOutcomesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(GetExpectedOutcomesQuery request, CancellationToken cancellationToken)
    {
        var expectedOutcomes = await _repository.GetAllAsync();
        return expectedOutcomes.Select(eo => new KeyValueDto(eo.Id, eo.Name));
    }
}


public class GetOpportunityTypesQueryHandler : IRequestHandler<GetOpportunityTypesQuery, IEnumerable<KeyValueDto>>
{
    private readonly IOpportunityTypeRepository _repository;

    public GetOpportunityTypesQueryHandler(IOpportunityTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(GetOpportunityTypesQuery request, CancellationToken cancellationToken)
    {
        var opportunityTypes = await _repository.GetAllAsync();
        return opportunityTypes.Select(ot => new KeyValueDto(ot.Id, ot.Name));
    }
}



public class GetThematicAreasQueryHandler : IRequestHandler<GetThematicAreasQuery, IEnumerable<KeyValueDto>>
{
    private readonly IThematicAreaRepository _repository;

    public GetThematicAreasQueryHandler(IThematicAreaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(GetThematicAreasQuery request, CancellationToken cancellationToken)
    {
        var thematicAreas = await _repository.GetAllAsync();
        return thematicAreas.Select(ta => new KeyValueDto(ta.Id, ta.Name));
    }
}
public class GetSuccessStoryStatusesQueryHandler : IRequestHandler<GetSuccessStoryStatusesQuery, IEnumerable<KeyValueDto>>
{

    public GetSuccessStoryStatusesQueryHandler()
    {
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(GetSuccessStoryStatusesQuery request, CancellationToken cancellationToken)
    {
        var successStoryStatuses = new List<KeyValueDto>
            {
                new KeyValueDto((byte)SuccessStoryStatus.Draft, "Draft"),
                new KeyValueDto((byte)SuccessStoryStatus.PendingReview, "Pending Review" ),
                new KeyValueDto ((byte) SuccessStoryStatus.Approved, "Approved"),
                new KeyValueDto ((byte) SuccessStoryStatus.Rejected, "Rejected"),
                new KeyValueDto ((byte) SuccessStoryStatus.Published, "Published"),
            };
        return successStoryStatuses;
    }
   
}
public class SuccessStoryCollaborationStatusesQueryHandler : IRequestHandler<SuccessStoryCollaborationStatusesQuery, IEnumerable<KeyValueDto>>
{

    public SuccessStoryCollaborationStatusesQueryHandler()
    {
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(SuccessStoryCollaborationStatusesQuery request, CancellationToken cancellationToken)
    {
        var successStoryCollaborationStatuses = new List<KeyValueDto>
            {
                new KeyValueDto((byte)SuccessStroyCollaborationStatus.Ongoing, SuccessStroyCollaborationStatus.Ongoing.ToString()),
                new KeyValueDto((byte)SuccessStroyCollaborationStatus.Successful, SuccessStroyCollaborationStatus.Successful.ToString() ),
            };
        return successStoryCollaborationStatuses;
    }

}
public class GetSuccessStoryTypeQueryHandler : IRequestHandler<GetSuccessStoryTypeQuery, IEnumerable<KeyValueDto>>
{
    private readonly ISuccessStroyTypeRepository _repository;

    public GetSuccessStoryTypeQueryHandler(ISuccessStroyTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(GetSuccessStoryTypeQuery request, CancellationToken cancellationToken)
    {
        var successStoryTypes = await _repository.GetAllAsync();
        return successStoryTypes.Select(ta => new KeyValueDto(ta.Id, ta.Name));
    }
}
public class GetSynergyCompanyQueryHandler : IRequestHandler<GetSynergyCompanyQuery, IEnumerable<GuidKeyValueDto>>
{
    private readonly ISynergyCompanyRepository _repository;
    public GetSynergyCompanyQueryHandler(ISynergyCompanyRepository repository)
    {
        _repository = repository;
    }
    public async Task<IEnumerable<GuidKeyValueDto>> Handle(GetSynergyCompanyQuery request, CancellationToken cancellationToken)
    {
        var synergyCompanies = await _repository.GetAllAsync();
        return synergyCompanies.Select(sc => new GuidKeyValueDto(sc.Id, sc.Name.Value));
    }
}