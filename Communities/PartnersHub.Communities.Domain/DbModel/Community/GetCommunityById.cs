using PartnersHub.Communities.Domain.Aggregates.Community;
using PartnersHub.Communities.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Communities.Domain.DbModel.Community
{
    public class GetCommunityById
    {
        public string Name { get;  set; }
        public string Description { get;  set; }
        public string ImageUrl { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public bool IsActive { get;  set; }

        public List<CommunityFollowerDto> Followers { get;  set; }
        public List<CommunityPostDto> Posts { get;  set; }

    }

    public class CommunityFollowerDto
    {
        public Guid CommunityId { get;  set; }
        public Guid UserId { get;  set; }
        public DateTime FollowedAt { get;  set; }

    }
    public class CommunityPostDto
    {
        public Guid CommunityId { get;  set; }
        public Guid AuthorId { get;  set; }
        public string Content { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public bool IsActive { get;  set; }

    }
}
