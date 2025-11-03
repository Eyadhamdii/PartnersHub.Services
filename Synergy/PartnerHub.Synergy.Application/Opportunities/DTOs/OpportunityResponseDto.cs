using Microsoft.AspNetCore.Http;
using PartnersHub.Synergy.Application.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Opportunities.DTOs
{
    public class OpportunityResponseDto
    {
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int ThematicAreaId { get; set; }
        public string ThematicAreaName { get; set; } = null!;
        public string SectorName { get; set; } = null!;
        public Guid SectorId { get; set; }
        public AttachmentMetaDataDto? Attachment { get; set; }
        public List<GuidKeyValueDto> CollaboratedProfiles { get; set; } = new();
        public string CollaborationRationale { get; set; } = null!;
        public List<KeyValueDto> CollaborationRequirements { get; set; } = new();
        public string? CollaborationRequirementOther { get; set; }
        public List<KeyValueDto> ExpectedOutcomes { get; set; } = new();
        public string? ExpectedOutcomeOther { get; set; }
        public string? RepresentativeName { get; set; }
        public string? RepresentativePhone { get; set; }
        public string? RepresentaitveTitle { get; set; }
        public string RepresentativeEmail { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public Guid TermsAndConditionId { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
