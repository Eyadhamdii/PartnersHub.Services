using PartnersHub.Synergy.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.SuccessStories.DTOs
{
    public class SuccessStoryResponseDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public string SuccessStoryType { get; set; }
        public List<GuidKeyValueDto> CollaboratingPartners { get; set; }
        public string SuccessStoryStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
