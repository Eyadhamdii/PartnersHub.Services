using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.InnovationHub.Domain.Enums
{
    public enum ChallengeStatus
    {
        Draft,
        Archived,
        Pending,
        RevisionsRequest,
        Approved,

    }
    public enum RequestStatus
    {
        PendingReview,
        Approved,
        Rejected
    }

    public enum TechnologyStage
    {
        Scouting,
        Engagment,
        Piloting,
        Scaling
    }
    public enum TechnologyStatus
    {
        Active,
        Archived
    }
    public enum Format
    {
        Documents,
        Video,
        Images
    }
    public enum Extension
    {
        PDF,
        DOCX,
        PPTX,
        XLSX,
        MP4,
        MOV,
        WEBM,
        PNG,
        JPG,
        WEBP,
        XLS,
        JPEG
    }

    public enum PriorityLevel
    {
        Urgent = 1,
        High = 2,
        Medium = 3,
        Low = 4
    }
}
