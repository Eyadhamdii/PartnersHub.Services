using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Domain.Common
{
    /// <summary>
    /// Type of terms and conditions
    /// </summary>
    public enum TermsAndConditionType : byte
    {
        SynergyTermsAndCondition = 0,
        GlobalTermsAndCondition = 1
    }

    /// <summary>
    /// Status of an opportunity submission
    /// </summary>
    public enum OpportunityStatus : byte
    {
        PendingApproval = 1,
        AdminApproved = 2,
        AssetManagerApproved = 3,
        AdminRejected = 4,
        AssetManagerRejected = 5,
        Published = 6
    }

    /// <summary>
    /// Status of a success story submission
    /// </summary>
    public enum SuccessStoryStatus : byte
    {
        Draft = 0,
        PendingReview = 1,
        Approved = 2,
        Rejected = 3,
        Published = 4
    }

    /// <summary>
    /// File extension types allowed for attachments
    /// </summary>
    public enum FileExtension : byte
    {
        Pdf = 0,
        Docx = 1,
        Xlsx = 2,
        Image = 3,
        Video = 4
    }
    public enum SuccessStroyCollaborationStatus : byte
    {
        Ongoing = 0,
        Successful = 1,
    }
}
