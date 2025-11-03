using Microsoft.AspNetCore.Routing;
using PartnersHub.Communities.Application.Common.Interfaces;

namespace PartnersHub.Communities.Application.Common.Extensions
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint: class, IMinimalEndpoint
        {
            TEndpoint.AddRoute(app);
            return app; 
        }
    }
}
