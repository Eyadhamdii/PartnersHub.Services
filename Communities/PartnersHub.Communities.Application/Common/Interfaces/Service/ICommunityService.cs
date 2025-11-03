using PartnersHub.Communities.Domain.DbModel.Community;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Communities.Application.Common.Interfaces.Service
{
    public interface ICommunityService
    {
        Task<GetCommunityById> GetCommunityById(Guid CommunityId, CancellationToken cancellationToken);
    }
}
