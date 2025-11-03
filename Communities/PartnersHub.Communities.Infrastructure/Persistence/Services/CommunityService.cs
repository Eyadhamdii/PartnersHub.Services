using PartnersHub.Communities.Application.Common.Interfaces.Rpository;
using PartnersHub.Communities.Application.Common.Interfaces.Service;
using PartnersHub.Communities.Domain.DbModel.Community;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Communities.Infrastructure.Persistence.Services
{
    public class CommunityService(ICommunitiesRepository communitiesRepository): ICommunityService
    {
        public async Task<GetCommunityById> GetCommunityById(Guid CommunityId ,CancellationToken cancellationToken)
        {
            var community =  await communitiesRepository.GetByIdAsync(CommunityId, cancellationToken);

            return new GetCommunityById()
            {
                CreatedAt = community.CreatedAt,
                Description = community.Description.Value,
                Followers = community.Followers.Select(x => new CommunityFollowerDto
                {
                    CommunityId = x.Id,
                    FollowedAt = x.FollowedAt,
                    UserId = x.UserId
                }).ToList(),
                ImageUrl = community.ImageUrl.Value,
                IsActive = community.IsActive,
                Name = community.Name,
                Posts = community.Posts.Select(x => new CommunityPostDto
                {
                    AuthorId = x.AuthorId,
                    CommunityId = x.CommunityId,
                    Content = x.Content.Value,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                }).ToList(),
            };
        }
    }
}
