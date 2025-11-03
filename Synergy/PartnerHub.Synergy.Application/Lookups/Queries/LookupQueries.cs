using MediatR;
using PartnersHub.Synergy.Application.Models;

public class GetCollaborationRequirementsQuery : IRequest<IEnumerable<KeyValueDto>>
{
}

public class GetExpectedOutcomesQuery : IRequest<IEnumerable<KeyValueDto>>
{
}



public class GetOpportunityTypesQuery : IRequest<IEnumerable<KeyValueDto>>
{
}

public class GetThematicAreasQuery : IRequest<IEnumerable<KeyValueDto>>
{
}
public class GetSuccessStoryStatusesQuery : IRequest<IEnumerable<KeyValueDto>>
{

}
public class SuccessStoryCollaborationStatusesQuery : IRequest<IEnumerable<KeyValueDto>>
{

}
public class GetSuccessStoryStoryTypeQuery : IRequest<IEnumerable<KeyValueDto>>
{

}
public class GetSuccessStoryTypeQuery : IRequest<IEnumerable<KeyValueDto>>
{

}
public class GetSynergyCompanyQuery : IRequest<IEnumerable<GuidKeyValueDto>>
{

}