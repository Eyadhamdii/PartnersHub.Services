using PartnersHub.Synergy.Application.Opportunities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.SuccessStories.DTOs
{
    public class SuccessStorySearchResponseDto
    {
        public List<SuccessStoryResponseDto> SuccessStories { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
