
using PartnersHub.Communities.Application.Common.Extensions;
using PartnersHub.Communities.Application.Communities.Queries;

namespace PartnersHub.Communities.Apis.Controllers
{
    public class CommunitiesModule : IModule
    {
        public static void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("api/Community").WithTags("Community").WithGroupName("community-module").WithOpenApi().RequireAuthorization();


            group.MapEndpoint<GetCommunityListEndpoint>();
        }
    }
}
