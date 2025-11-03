using MediatR;
using PartnersHub.Synergy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Opportunities.Commands
{
    public class CreateOpportunityAttachmentMetaDataCommand :IRequest<Result>
    {
        public Guid OpportunityId { get; private set; }
        public string FileName { get; private set; } = null!;
        public string SharePointUrl { get; private set; } = null!;
        public string FileExtension { get; private set; } = null!;
        public long FileSizeInBytes { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public string? UploadedBy { get; private set; }
    }
}
