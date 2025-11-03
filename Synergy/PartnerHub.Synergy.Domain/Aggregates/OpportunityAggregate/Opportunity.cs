using PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Domain.Common;
using PartnersHub.Synergy.Domain.Events;
using PartnersHub.Synergy.Domain.ValueObjects;
using System.Net.Mail;
using OpportunityStatus = PartnersHub.Synergy.Domain.Common.OpportunityStatus;

namespace PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;

public class Opportunity : AggregateRoot
{
    private readonly List<ExpectedOutcome> _expectedOutcomes = new List<ExpectedOutcome>();
    private readonly List<CollaborationRequirement> _collaborationRequirements = new List<CollaborationRequirement>();
    private readonly List<OpportunitySynergyCompany> _collaboratedCompanies = new List<OpportunitySynergyCompany>();
    private readonly List<SuccessStoryOpportunity> _successStoryOpportunities = new List<SuccessStoryOpportunity>();
    private readonly List<OpportunityAttachment> _attachments = new List<OpportunityAttachment>();
    public Guid CompanyId { get; private set; }
    public Title Title { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public int OpportunityTypeId { get; private set; }
    public OpportunityType? OpportunityType { get; private set; }
    public int ThematicAreaId { get; private set; }
    public ThematicArea? ThematicArea { get; private set; }
    public Sector Sector { get; private set; }
    public IReadOnlyCollection<OpportunitySynergyCompany> CollaboratedCompanies => _collaboratedCompanies.AsReadOnly();
    public IReadOnlyCollection<OpportunityAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<SuccessStoryOpportunity> SuccessStoryOpportunities => _successStoryOpportunities.AsReadOnly();
    public string? CollaborationRationale { get; private set; }
    public IReadOnlyCollection<CollaborationRequirement> CollaborationRequirements => _collaborationRequirements.AsReadOnly();
    public string? CollaborationRequirementOther { get; private set; }
    public IReadOnlyCollection<ExpectedOutcome> ExpectedOutcomes => _expectedOutcomes.AsReadOnly();
    public string? ExpectedOutcomeOther { get; private set; }
    public RepresentativeInformation RepresentativeInformation { get; set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public Guid TermsAndConditionId { get; private set; }
    public bool TermsAccepted { get; private set; }
    public DateTime? TermsAcceptedAt { get; private set; }
    public Guid? TermsAcceptedBy { get; private set; }

    public OpportunityStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? RejectedBy { get; private set; }
    public DateTime? RejectedAt { get; private set; }

    private Opportunity() { }

    private Opportunity(Guid companyId, Title title, Description description, Sector sector, RepresentativeInformation representativeInfo,
        string collaborationRationale, int opportunityTypeId, int thematicAreaId, DateOnly startDate, DateOnly endDate,
        Guid termsAndConditionId, Guid createdBy)
    {
        CompanyId = companyId;
        Title = title;
        Description = description;
        Sector = sector;
        RepresentativeInformation = representativeInfo;
        OpportunityTypeId = opportunityTypeId;
        ThematicAreaId = thematicAreaId;
        StartDate = startDate;
        EndDate = endDate;
        TermsAndConditionId = termsAndConditionId;
        TermsAccepted = true;
        Status = OpportunityStatus.PendingApproval;
        CollaborationRationale = collaborationRationale;
        MarkAsCreated(createdBy);
    }

    public static Result<Opportunity> Create(Guid companyId, string title, string description, string sectorName,
        Guid sectorId, int opportunityTypeId, int thematicAreaId, string representativeEmail, string representativeName,
        string representativePhone, string representativePosition, string collaborationRationale, 
        DateOnly startDate, DateOnly endDate, Guid termsAndConditionId, Guid createdBy)
    {
        if (companyId == Guid.Empty)
            return Result<Opportunity>.Failure("Company ID is required");

        var titleResult = Title.Create(title);
        if (titleResult.IsFailure)
            return Result<Opportunity>.Failure(titleResult.Error!);

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure)
            return Result<Opportunity>.Failure(descriptionResult.Error!);


        var sectorResult = Sector.Create(sectorName, sectorId);
        if (sectorResult.IsFailure)
            return Result<Opportunity>.Failure(sectorResult.Error!);

        var representativeInfo = RepresentativeInformation.Create(representativeName, representativePosition, representativeEmail, representativePhone);
        if (representativeInfo.IsFailure)
            return Result<Opportunity>.Failure(representativeInfo.Error!);

        if (startDate >= endDate)
            return Result<Opportunity>.Failure("End date must be after start date");

        //if (termsAndConditionId == Guid.Empty)
        //    return Result<Opportunity>.Failure("Terms and condition is required");

        var opportunity = new Opportunity(companyId, titleResult.Value!, descriptionResult.Value!, sectorResult.Value, representativeInfo.Value,
            collaborationRationale, opportunityTypeId, thematicAreaId, startDate, endDate,
            termsAndConditionId, createdBy);

        return Result<Opportunity>.Success(opportunity);
    }

    public Result AcceptTermsAndConditions(Guid userId)
    {
        if (TermsAccepted)
            return Result.Failure("Terms and conditions already accepted");

        TermsAccepted = true;
        TermsAcceptedAt = DateTime.UtcNow;
        TermsAcceptedBy = userId;
        MarkAsUpdated(userId);

        return Result.Success();
    }

    public Result AddCollaboratedCompanies(List<Guid> companyIds)
    {
        if (companyIds == null || companyIds.Count == 0)
            return Result.Failure("At least one collaboration company is required");

        try
        {
            foreach (var companyId in companyIds)
            {
                if (!_collaboratedCompanies.Any(c => c.SynergyCompanyId == companyId))
                {
                    var collaboration = new OpportunitySynergyCompany(Id, companyId);
                    _collaboratedCompanies.Add(collaboration);
                }
            }
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result RemoveCollaboratedCompany(Guid companyId)
    {
        var collaboration = _collaboratedCompanies.FirstOrDefault(c => c.SynergyCompanyId == companyId);
        if (collaboration == null)
            return Result.Failure("Collaborated company not found");

        _collaboratedCompanies.Remove(collaboration);
        return Result.Success();
    }

    public Result AddAttachment(string fileName, string sharePointUrl, long fileSizeInBytes,
        string? uploadedBy = null)
    {
        try
        {
            var attachment = new OpportunityAttachment(Id, fileName, sharePointUrl, fileSizeInBytes, uploadedBy);
            _attachments.Add(attachment);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public Result RemoveAttachment(Guid attachmentId)
    {
        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment == null)
            return Result.Failure("Attachment not found");

        _attachments.Remove(attachment);
        return Result.Success();
    }

    public Result AddCollaborationRequirement(List<CollaborationRequirement> collaborationRequirements,
        string otherCollaborationRequirement = "")
    {
        if (collaborationRequirements == null || collaborationRequirements.Count == 0)
            return Result.Failure("Collaboration requirements are required");

        if (collaborationRequirements.Any(cr => cr.Name.Contains(Constants.Other)))
        {
            if (string.IsNullOrEmpty(otherCollaborationRequirement))
            {
                return Result.Failure("other collaboration requirment field is required");
            }
            CollaborationRequirementOther = otherCollaborationRequirement;
        }
            

        _collaborationRequirements.AddRange(collaborationRequirements);
        return Result.Success();
    }

    public Result AddExpectedOutcome(List<ExpectedOutcome> expectedOutcomes, string otherExpectedOutcome = "")
    {
        if (expectedOutcomes == null || expectedOutcomes.Count == 0)
            return Result.Failure("Expected outcomes are required");
        if (expectedOutcomes.Any(cr => cr.Name.Contains(Constants.Other)))
        {
            if (string.IsNullOrEmpty(otherExpectedOutcome))
            {
                return Result.Failure("other expected outcome field is required");
            }
            ExpectedOutcomeOther = otherExpectedOutcome;
        }


        _expectedOutcomes.AddRange(expectedOutcomes);
        return Result.Success();
    }

    public Result SetOpportunityType(OpportunityType opportunityType)
    {
        if (opportunityType == null)
            return Result.Failure("Opportunity type is invalid");
        OpportunityType = opportunityType;
        return Result.Success();
    }

    public Result SetThematicArea(ThematicArea thematicArea)
    {
        if (thematicArea == null)
            return Result.Failure("thematic area is invalid");
        ThematicArea = thematicArea;
        return Result.Success();
    }

    public Result SetCollaborationRationale(string rationale)
    {
        if (string.IsNullOrWhiteSpace(rationale))
            return Result.Failure("Collaboration rationale is required");

        CollaborationRationale = rationale.Trim();
        return Result.Success();
    }

    public Result Submit(Guid userId)
    {
        //if (Status != OpportunityStatus.Draft)
        //    return Result.Failure("Only draft opportunities can be submitted");

        if (!TermsAccepted)
            return Result.Failure("Terms and conditions must be accepted before submission");

        if (_collaboratedCompanies.Count == 0)
            return Result.Failure("At least one collaboration company is required");

        if (_collaborationRequirements.Count == 0)
            return Result.Failure("At least one collaboration requirement is required");

        if (_expectedOutcomes.Count == 0)
            return Result.Failure("At least one expected outcome is required");

        Status = OpportunityStatus.PendingApproval;
        MarkAsUpdated(userId);
        AddDomainEvent(new OpportunitySubmittedEvent(Id, CompanyId, userId));

        return Result.Success();
    }

    public Result<bool> ApproveByAdmin(Guid userId)
    {
        if (Status != OpportunityStatus.AssetManagerApproved)
            return Result<bool>.Failure("Only pending opportunities can be approved");

        Status = OpportunityStatus.AdminApproved;
        ApprovedBy = userId;
        ApprovedAt = DateTime.UtcNow;
        MarkAsUpdated(userId);
        AddDomainEvent(new OpportunityApprovedEvent(Id, CompanyId, Status, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> ApproveByAssetManager(Guid userId)
    {
        if (Status != OpportunityStatus.PendingApproval)
            return Result<bool>.Failure("Opportunity must be pending approval");

        Status = OpportunityStatus.AssetManagerApproved;
        ApprovedBy = userId;
        ApprovedAt = DateTime.UtcNow;
        MarkAsUpdated(userId);
        AddDomainEvent(new OpportunityApprovedEvent(Id, CompanyId, Status, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> RejectByAdmin(Guid userId, string rejectionReason)
    {
        if (Status != OpportunityStatus.AssetManagerApproved)
            return Result<bool>.Failure("Only approved opportunities can be rejected");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            return Result<bool>.Failure("Rejection reason is required");

        Status = OpportunityStatus.AdminRejected;
        RejectionReason = rejectionReason.Trim();
        RejectedBy = userId;
        RejectedAt = DateTime.UtcNow;
        MarkAsUpdated(userId);
        AddDomainEvent(new OpportunityRejectedEvent(Id, CompanyId, Status, rejectionReason, userId));

        return Result<bool>.Success(true);
    }

    public Result RejectByAssetManager(Guid userId, string rejectionReason)
    {
        if (Status != OpportunityStatus.PendingApproval)
            return Result.Failure("Opportunity must be admin approved before asset manager can reject");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            return Result.Failure("Rejection reason is required");

        Status = OpportunityStatus.AssetManagerRejected;
        RejectionReason = rejectionReason.Trim();
        RejectedBy = userId;
        RejectedAt = DateTime.UtcNow;
        MarkAsUpdated(userId);
        AddDomainEvent(new OpportunityRejectedEvent(Id, CompanyId, Status, rejectionReason, userId));

        return Result.Success();
    }

    public Result<bool> Publish(Guid userId)
    {
        if (Status != OpportunityStatus.AdminApproved)
            return Result<bool>.Failure("Only asset manager approved opportunities can be published");

        Status = OpportunityStatus.Published;
        MarkAsUpdated(userId);
        AddDomainEvent(new OpportunityPublishedEvent(Id, CompanyId, userId));

        return Result<bool>.Success(true);
    }

    public bool CanBeEditedBy(Guid userId)
    {
        //return Status == OpportunityStatus.Draft && CreatedBy == userId;
        return true;
    }
}
