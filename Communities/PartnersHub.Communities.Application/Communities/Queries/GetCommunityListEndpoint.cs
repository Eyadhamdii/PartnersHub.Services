using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PartnersHub.Communities.Application.Common.Interfaces;
using PartnersHub.Communities.Application.Common.Interfaces.Rpository;
using PartnersHub.Communities.Domain.Aggregates.Community;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Communities.Application.Communities.Queries
{
    public class GetCommunityListEndpoint : IMinimalEndpoint
    {
        public static void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapGet("/CommunitiesList", Handle).Produces<List<Community>>();
        }

        public static async Task<IResult> Handle(ICommunitiesRepository communitiesRepository)
        {
            return TypedResults.Ok(await communitiesRepository.GetAllAsync());
        }

    }

}
