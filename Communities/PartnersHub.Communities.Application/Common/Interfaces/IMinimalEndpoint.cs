using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Communities.Application.Common.Interfaces
{
    public interface IMinimalEndpoint
    {
        static abstract void AddRoute(IEndpointRouteBuilder app);
    }
}
