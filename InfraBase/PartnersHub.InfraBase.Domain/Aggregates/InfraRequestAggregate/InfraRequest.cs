using PartnersHub.InfraBase.Domain.Common;
using PartnersHub.InfraBase.Domain.Enums;
using PartnersHub.InfraBase.Domain.Events;
using PartnersHub.InfraBase.Domain.ValueObjects;

namespace PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

public class InfraRequest : AggregateRoot {
    private readonly List<InfraRequestItem> _items = new();
    private readonly List<InfraRequestHistory> _history = new();
    private readonly List<InfraRequestAttachment> _attachments = new();

    public string? RequestCode { get; private set; }
    public ProjectName ProjectName { get; private set; } = null!;
    public ProjectDescription? ProjectDescription { get; private set; }
    public Guid SectorId { get; private set; }
    public Guid SubSectorId { get; private set; }
    public Guid AssetTypeId { get; private set; }
    public string? AssetTypeOtherDescription { get; private set; }
    public TenderingStage TenderingStage { get; private set; }
    public FundingModel FundingModel { get; private set; }
    public int? StartConstructionQuarter { get; private set; }
    public int? StartConstructionYear { get; private set; }
    public int? EndConstructionQuarter { get; private set; }
    public int? EndConstructionYear { get; private set; }
    public RequestStatus Status { get; private set; }
    public decimal TotalRequestAmount => _items.Sum(i => i.TotalAmount);
    public Guid? SubmittedBy { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public RejectionReason? RejectionReason { get; private set; }
    public Guid? RejectedBy { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? CompanyId { get; private set; }
    public string? CompanyName { get; private set; }

    public IReadOnlyCollection<InfraRequestItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<InfraRequestHistory> History => _history.AsReadOnly();
    public IReadOnlyCollection<InfraRequestAttachment> Attachments => _attachments.AsReadOnly();

    private InfraRequest() { }

    private InfraRequest(ProjectName projectName, ProjectDescription? projectDescription, Guid sectorId, Guid subSectorId, Guid assetTypeId, 
        string? assetTypeOtherDescription, TenderingStage tenderingStage, FundingModel fundingModel, Guid createdBy, Guid? companyId = null, 
        string? companyName = null, int? startConstructionQuarter = null, int? startConstructionYear = null, int? endConstructionQuarter = null, 
        int? endConstructionYear = null) {
        ProjectName = projectName;
        ProjectDescription = projectDescription;
        SectorId = sectorId;
        SubSectorId = subSectorId;
        AssetTypeId = assetTypeId;
        AssetTypeOtherDescription = assetTypeOtherDescription;
        TenderingStage = tenderingStage;
        FundingModel = fundingModel;
        StartConstructionQuarter = startConstructionQuarter;
        StartConstructionYear = startConstructionYear;
        EndConstructionQuarter = endConstructionQuarter;
        EndConstructionYear = endConstructionYear;
        Status = RequestStatus.Draft;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        CompanyId = companyId;
        CompanyName = companyName;

        AddHistory("Created", createdBy, "Infrastructure request created as draft");
    }

    public static Result<InfraRequest> Create(string projectName, string? projectDescription, Guid sectorId, Guid subSectorId, Guid assetTypeId, 
        string? assetTypeOtherDescription, TenderingStage tenderingStage, FundingModel fundingModel, Guid createdBy, Guid? companyId = null, 
        string? companyName = null, int? startConstructionQuarter = null, int? startConstructionYear = null, int? endConstructionQuarter = null, 
        int? endConstructionYear = null) {
        var projectNameResult = ProjectName.Create(projectName);
        if (projectNameResult.IsFailure) {
            return Result<InfraRequest>.Failure(projectNameResult.Error!);
        }

        var projectDescriptionResult = ProjectDescription.Create(projectDescription);
        if (projectDescriptionResult.IsFailure) {
            return Result<InfraRequest>.Failure(projectDescriptionResult.Error!);
        }

        if (sectorId == Guid.Empty) {
            return Result<InfraRequest>.Failure("Sector is required");
        }

        if (subSectorId == Guid.Empty) {
            return Result<InfraRequest>.Failure("Sub sector is required");
        }

        if (assetTypeId == Guid.Empty) {
            return Result<InfraRequest>.Failure("Asset type is required");
        }

        if (startConstructionQuarter.HasValue && (startConstructionQuarter < 1 || startConstructionQuarter > 4)) {
            return Result<InfraRequest>.Failure("Start construction quarter must be between 1 and 4");
        }

        if (endConstructionQuarter.HasValue && (endConstructionQuarter < 1 || endConstructionQuarter > 4)) {
            return Result<InfraRequest>.Failure("End construction quarter must be between 1 and 4");
        }

        if (startConstructionYear.HasValue && (startConstructionYear < 2000 || startConstructionYear > 2099)) {
            return Result<InfraRequest>.Failure("Start construction year must be between 2000 and 2099");
        }

        if (endConstructionYear.HasValue && (endConstructionYear < 2000 || endConstructionYear > 2099)) {
            return Result<InfraRequest>.Failure("End construction year must be between 2000 and 2099");
        }

        if (startConstructionYear.HasValue && endConstructionYear.HasValue) {
            if (endConstructionYear < startConstructionYear) {
                return Result<InfraRequest>.Failure("End construction date must be after start construction date");
            }
            
            if (endConstructionYear == startConstructionYear && startConstructionQuarter.HasValue && endConstructionQuarter.HasValue &&
                endConstructionQuarter < startConstructionQuarter) {
                return Result<InfraRequest>.Failure("End construction date must be after start construction date");
            }
        }

        var request = new InfraRequest(projectNameResult.Value!, projectDescriptionResult.Value!, sectorId, subSectorId, assetTypeId, 
            assetTypeOtherDescription, tenderingStage, fundingModel, createdBy, companyId, companyName, startConstructionQuarter, 
            startConstructionYear, endConstructionQuarter, endConstructionYear);

        return Result<InfraRequest>.Success(request);
    }

    public void AssignRequestCode(string code) {
        if (string.IsNullOrWhiteSpace(code)) {
            throw new ArgumentException("Request code cannot be empty", nameof(code));
        }
        
        if (string.IsNullOrEmpty(RequestCode)) {
            RequestCode = code;
        }
    }

    public Result<bool> AddItem(string itemCode, string itemName, Guid uomId, decimal quantity, decimal unitPrice, Guid userId) {
        if (_items.Any(i => i.ItemName.Value.Equals(itemName, StringComparison.OrdinalIgnoreCase))) {
            return Result<bool>.Failure("Item name already exists in this request");
        }

        try {
            var item = new InfraRequestItem(itemCode, itemName, uomId, quantity, unitPrice);
            _items.Add(item);
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;

            AddHistory("Item Added", userId, $"Item '{itemName}' (Code: {itemCode}) added with quantity {quantity} and unit price {unitPrice:C}");
            AddDomainEvent(new RequestItemAddedEvent(Id, item.Id, itemName, item.TotalAmount));

            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex) {
            return Result<bool>.Failure(ex.Message);
        }
    }

    public Result<bool> RemoveItem(Guid itemId, Guid userId) {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) {
            return Result<bool>.Failure("Item not found");
        }

        var itemName = item.ItemName.Value;
        var itemCode = item.ItemCode;
        
        _items.Remove(item);
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        AddHistory("Item Removed", userId, $"Item '{itemName}' (Code: {itemCode}) removed from request");

        return Result<bool>.Success(true);
    }

    public Result<bool> UpdateItem(Guid itemId, decimal? quantity, decimal? unitPrice, Guid userId) {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) {
            return Result<bool>.Failure("Item not found");
        }

        var changes = new List<string>();
        var oldValues = new List<string>();
        var newValues = new List<string>();
        var oldQuantity = item.Quantity.Value;
        var oldUnitPrice = item.UnitPrice.Value;

        if (quantity.HasValue && quantity.Value != oldQuantity) {
            var quantityResult = item.UpdateQuantity(quantity.Value);
            if (quantityResult.IsFailure) {
                return quantityResult;
            }
            changes.Add("Quantity");
            oldValues.Add(oldQuantity.ToString());
            newValues.Add(quantity.Value.ToString());
        }

        if (unitPrice.HasValue && unitPrice.Value != oldUnitPrice) {
            var priceResult = item.UpdateUnitPrice(unitPrice.Value);
            if (priceResult.IsFailure) {
                return priceResult;
            }
            changes.Add("UnitPrice");
            oldValues.Add(oldUnitPrice.ToString("C"));
            newValues.Add(unitPrice.Value.ToString("C"));
        }

        if (changes.Any()) {
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;

            AddHistory("Item Updated", userId, $"Item '{item.ItemName.Value}' (Code: {item.ItemCode}) updated", 
                string.Join(", ", changes), string.Join(", ", oldValues), string.Join(", ", newValues));
        }

        return Result<bool>.Success(true);
    }

    public Result<bool> SaveAsDraft(Guid userId) {
        if (Status != RequestStatus.Draft) {
            return Result<bool>.Failure("Only draft requests can be saved as draft");
        }

        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        AddHistory("Saved as Draft", userId, "Request saved as draft");
        AddDomainEvent(new RequestSavedAsDraftEvent(Id, ProjectName.Value, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> Submit(Guid userId, string requestCode) {
        if (Status != RequestStatus.Draft && Status != RequestStatus.ChangeRequested && 
            Status != RequestStatus.RejectedByPcAdmin && Status != RequestStatus.InfrabaseRejected) {
            return Result<bool>.Failure("Can only submit requests with status Draft, ChangeRequested, RejectedByPcAdmin, or InfrabaseRejected");
        }

        if (_items.Count == 0) {
            return Result<bool>.Failure("Cannot submit request without items");
        }

        foreach (var item in _items) {
            if (item.FinancialDistributions.Count == 0) {
                return Result<bool>.Failure($"Item '{item.ItemName.Value}' must have financial distributions");
            }
        }

        var previousStatus = Status;
        RequestCode = requestCode;
        Status = RequestStatus.PendingPcAdminApproval;
        SubmittedBy = userId;
        SubmittedAt = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
        RejectionReason = null;
        RejectedBy = null;
        RejectedAt = null;

        var comments = previousStatus == RequestStatus.Draft ? "Request submitted for PC Admin approval" :
            previousStatus == RequestStatus.ChangeRequested ? "Request resubmitted after implementing requested changes" :
            "Request resubmitted after addressing rejection reasons";

        var action = previousStatus == RequestStatus.Draft ? "Submitted" : "Resubmitted";

        AddHistory(action, userId, comments);
        AddDomainEvent(new RequestSubmittedEvent(Id, RequestCode, Status.ToString(), userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> AcceptByPcAdmin(Guid userId) {
        if (Status != RequestStatus.PendingPcAdminApproval) {
            return Result<bool>.Failure("Only requests pending PC admin approval can be accepted");
        }

        if (string.IsNullOrEmpty(RequestCode)) {
            return Result<bool>.Failure("Request must have a request code");
        }

        Status = RequestStatus.PendingPcInfrabaseApproval;
        ApprovedBy = userId;
        ApprovedAt = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        AddHistory("Accepted by PC Admin", userId, "Request accepted and forwarded to Infrabase admin");
        AddDomainEvent(new RequestAcceptedByPcAdminEvent(Id, RequestCode, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> RejectByPcAdmin(Guid userId, string rejectionReason) {
        if (Status != RequestStatus.PendingPcAdminApproval) {
            return Result<bool>.Failure("Only requests pending PC admin approval can be rejected");
        }

        if (string.IsNullOrEmpty(RequestCode)) {
            return Result<bool>.Failure("Request must have a request code");
        }

        var rejectionReasonResult = RejectionReason.Create(rejectionReason);
        if (rejectionReasonResult.IsFailure) {
            return Result<bool>.Failure(rejectionReasonResult.Error!);
        }

        Status = RequestStatus.RejectedByPcAdmin;
        RejectionReason = rejectionReasonResult.Value;
        RejectedBy = userId;
        RejectedAt = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        AddHistory("Rejected by PC Admin", userId, rejectionReason);
        AddDomainEvent(new RequestRejectedByPcAdminEvent(Id, RequestCode, rejectionReason, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> AcceptByInfrabaseAdmin(Guid userId) {
        if (Status != RequestStatus.PendingPcInfrabaseApproval) {
            return Result<bool>.Failure("Only requests pending Infrabase admin approval can be accepted");
        }

        if (string.IsNullOrEmpty(RequestCode)) {
            return Result<bool>.Failure("Request must have a request code");
        }

        Status = RequestStatus.InfrabaseApproved;
        ApprovedBy = userId;
        ApprovedAt = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        AddHistory("Approved by Infrabase Admin", userId, "Request approved - Final approval");
        AddDomainEvent(new RequestApprovedByInfrabaseAdminEvent(Id, RequestCode, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> RejectByInfrabaseAdmin(Guid userId, string rejectionReason) {
        if (Status != RequestStatus.PendingPcInfrabaseApproval) {
            return Result<bool>.Failure("Only requests pending Infrabase admin approval can be rejected");
        }

        if (string.IsNullOrEmpty(RequestCode)) {
            return Result<bool>.Failure("Request must have a request code");
        }

        var rejectionReasonResult = RejectionReason.Create(rejectionReason);
        if (rejectionReasonResult.IsFailure) {
            return Result<bool>.Failure(rejectionReasonResult.Error!);
        }

        Status = RequestStatus.InfrabaseRejected;
        RejectionReason = rejectionReasonResult.Value;
        RejectedBy = userId;
        RejectedAt = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        AddHistory("Rejected by Infrabase Admin", userId, rejectionReason);
        AddDomainEvent(new RequestRejectedByInfrabaseAdminEvent(Id, RequestCode, rejectionReason, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> RequestChangesByInfrabaseAdmin(Guid userId, string changeRequestDescription) {
        if (Status != RequestStatus.PendingPcInfrabaseApproval) {
            return Result<bool>.Failure("Only requests pending Infrabase admin approval can have changes requested");
        }

        if (string.IsNullOrEmpty(RequestCode)) {
            return Result<bool>.Failure("Request must have a request code");
        }

        if (string.IsNullOrWhiteSpace(changeRequestDescription)) {
            return Result<bool>.Failure("Change request description is required");
        }

        if (changeRequestDescription.Length > 3000) {
            return Result<bool>.Failure("Change request description cannot exceed 3000 characters");
        }

        Status = RequestStatus.ChangeRequested;
        RejectionReason = RejectionReason.Create(changeRequestDescription).Value;
        RejectedBy = userId;
        RejectedAt = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;

        AddHistory("Changes Requested by Infrabase Admin", userId, changeRequestDescription);
        AddDomainEvent(new RequestChangesRequestedByInfrabaseAdminEvent(Id, RequestCode, changeRequestDescription, userId));

        return Result<bool>.Success(true);
    }

    public Result<bool> UpdateBasicInformation(string? projectName, string? projectDescription, Guid? sectorId, Guid? subSectorId, Guid? assetTypeId, 
        string? assetTypeOtherDescription, TenderingStage? tenderingStage, FundingModel? fundingModel, Guid userId, int? startConstructionQuarter = null, 
        int? startConstructionYear = null, int? endConstructionQuarter = null, int? endConstructionYear = null) {
        if (Status != RequestStatus.Draft && Status != RequestStatus.RejectedByPcAdmin && 
            Status != RequestStatus.InfrabaseRejected && Status != RequestStatus.ChangeRequested) {
            return Result<bool>.Failure("Only draft, rejected, or change-requested requests can be updated");
        }

        var changes = new List<string>();
        var oldValues = new List<string>();
        var newValues = new List<string>();

        if (projectName != null) {
            var projectNameResult = ProjectName.Create(projectName);
            if (projectNameResult.IsFailure) {
                return Result<bool>.Failure(projectNameResult.Error!);
            }

            if (ProjectName.Value != projectName) {
                changes.Add("ProjectName");
                oldValues.Add(ProjectName.Value);
                newValues.Add(projectName);
                ProjectName = projectNameResult.Value!;
            }
        }

        if (projectDescription != null) {
            var projectDescriptionResult = ProjectDescription.Create(projectDescription);
            if (projectDescriptionResult.IsFailure) {
                return Result<bool>.Failure(projectDescriptionResult.Error!);
            }

            var oldDesc = ProjectDescription?.Value ?? "";
            if (oldDesc != projectDescription) {
                changes.Add("ProjectDescription");
                oldValues.Add(oldDesc);
                newValues.Add(projectDescription);
                ProjectDescription = projectDescriptionResult.Value!;
            }
        }

        if (sectorId.HasValue && sectorId.Value != Guid.Empty && SectorId != sectorId.Value) {
            changes.Add("SectorId");
            oldValues.Add(SectorId.ToString());
            newValues.Add(sectorId.Value.ToString());
            SectorId = sectorId.Value;
        }

        if (subSectorId.HasValue && subSectorId.Value != Guid.Empty && SubSectorId != subSectorId.Value) {
            changes.Add("SubSectorId");
            oldValues.Add(SubSectorId.ToString());
            newValues.Add(subSectorId.Value.ToString());
            SubSectorId = subSectorId.Value;
        }

        if (assetTypeId.HasValue && assetTypeId.Value != Guid.Empty && AssetTypeId != assetTypeId.Value) {
            changes.Add("AssetTypeId");
            oldValues.Add(AssetTypeId.ToString());
            newValues.Add(assetTypeId.Value.ToString());
            AssetTypeId = assetTypeId.Value;
        }

        if (assetTypeOtherDescription != null && AssetTypeOtherDescription != assetTypeOtherDescription) {
            changes.Add("AssetTypeOtherDescription");
            oldValues.Add(AssetTypeOtherDescription ?? "");
            newValues.Add(assetTypeOtherDescription);
            AssetTypeOtherDescription = assetTypeOtherDescription;
        }

        if (tenderingStage.HasValue && TenderingStage != tenderingStage.Value) {
            changes.Add("TenderingStage");
            oldValues.Add(TenderingStage.ToString());
            newValues.Add(tenderingStage.Value.ToString());
            TenderingStage = tenderingStage.Value;
        }

        if (fundingModel.HasValue && FundingModel != fundingModel.Value) {
            changes.Add("FundingModel");
            oldValues.Add(FundingModel.ToString());
            newValues.Add(fundingModel.Value.ToString());
            FundingModel = fundingModel.Value;
        }

        if (startConstructionQuarter.HasValue && (startConstructionQuarter < 1 || startConstructionQuarter > 4)) {
            return Result<bool>.Failure("Start construction quarter must be between 1 and 4");
        }

        if (endConstructionQuarter.HasValue && (endConstructionQuarter < 1 || endConstructionQuarter > 4)) {
            return Result<bool>.Failure("End construction quarter must be between 1 and 4");
        }

        if (startConstructionYear.HasValue && (startConstructionYear < 2000 || startConstructionYear > 2099)) {
            return Result<bool>.Failure("Start construction year must be between 2000 and 2099");
        }

        if (endConstructionYear.HasValue && (endConstructionYear < 2000 || endConstructionYear > 2099)) {
            return Result<bool>.Failure("End construction year must be between 2000 and 2099");
        }

        if (startConstructionQuarter.HasValue && StartConstructionQuarter != startConstructionQuarter.Value) {
            changes.Add("StartConstructionQuarter");
            oldValues.Add(StartConstructionQuarter?.ToString() ?? "");
            newValues.Add(startConstructionQuarter.Value.ToString());
            StartConstructionQuarter = startConstructionQuarter.Value;
        }

        if (startConstructionYear.HasValue && StartConstructionYear != startConstructionYear.Value) {
            changes.Add("StartConstructionYear");
            oldValues.Add(StartConstructionYear?.ToString() ?? "");
            newValues.Add(startConstructionYear.Value.ToString());
            StartConstructionYear = startConstructionYear.Value;
        }

        if (endConstructionQuarter.HasValue && EndConstructionQuarter != endConstructionQuarter.Value) {
            changes.Add("EndConstructionQuarter");
            oldValues.Add(EndConstructionQuarter?.ToString() ?? "");
            newValues.Add(endConstructionQuarter.Value.ToString());
            EndConstructionQuarter = endConstructionQuarter.Value;
        }

        if (endConstructionYear.HasValue && EndConstructionYear != endConstructionYear.Value) {
            changes.Add("EndConstructionYear");
            oldValues.Add(EndConstructionYear?.ToString() ?? "");
            newValues.Add(endConstructionYear.Value.ToString());
            EndConstructionYear = endConstructionYear.Value;
        }

        if (StartConstructionYear.HasValue && EndConstructionYear.HasValue) {
            if (EndConstructionYear < StartConstructionYear) {
                return Result<bool>.Failure("End construction date must be after start construction date");
            }
            
            if (EndConstructionYear == StartConstructionYear && StartConstructionQuarter.HasValue && EndConstructionQuarter.HasValue &&
                EndConstructionQuarter < StartConstructionQuarter) {
                return Result<bool>.Failure("End construction date must be after start construction date");
            }
        }

        if (changes.Any()) {
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;

            AddHistory("Updated", userId, "Request information updated", string.Join(", ", changes), 
                string.Join(", ", oldValues), string.Join(", ", newValues));
        }

        return Result<bool>.Success(true);
    }

    public InfraRequestItem? GetItem(Guid itemId) {
        return _items.FirstOrDefault(i => i.Id == itemId);
    }

    public Result<InfraRequestAttachment> AddAttachment(string fileName, long fileSizeInBytes, string contentType, string sharePointFileId, 
        string sharePointUrl, string sharePointLibrary, Guid uploadedBy) {
        if (Status == RequestStatus.InfrabaseApproved) {
            return Result<InfraRequestAttachment>.Failure("Cannot add attachments to approved requests");
        }

        try {
            var attachment = new InfraRequestAttachment(Id, fileName, fileSizeInBytes, contentType, sharePointFileId, 
                sharePointUrl, sharePointLibrary, uploadedBy);

            _attachments.Add(attachment);
            UpdatedBy = uploadedBy;
            UpdatedAt = DateTime.UtcNow;

            AddHistory("Attachment Added", uploadedBy, $"Attachment '{fileName}' ({fileSizeInBytes / 1024:N0} KB) uploaded");

            return Result<InfraRequestAttachment>.Success(attachment);
        }
        catch (ArgumentException ex) {
            return Result<InfraRequestAttachment>.Failure(ex.Message);
        }
    }

    public Result<bool> RemoveAttachment(Guid attachmentId, Guid deletedBy) {
        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId && !a.IsDeleted);
        if (attachment == null) {
            return Result<bool>.Failure("Attachment not found");
        }

        if (Status == RequestStatus.InfrabaseApproved) {
            return Result<bool>.Failure("Cannot remove attachments from approved requests");
        }

        var result = attachment.MarkAsDeleted(deletedBy);
        if (result.IsFailure) {
            return result;
        }

        UpdatedAt = DateTime.UtcNow;
        AddHistory("Attachment Removed", deletedBy, $"Attachment '{attachment.Metadata.FileName}' removed");

        return Result<bool>.Success(true);
    }

    public IReadOnlyCollection<InfraRequestAttachment> GetAttachments() {
        return _attachments.Where(a => !a.IsDeleted).ToList().AsReadOnly();
    }

    public InfraRequestAttachment? GetAttachment(Guid attachmentId) {
        return _attachments.FirstOrDefault(a => a.Id == attachmentId && !a.IsDeleted);
    }

    private void AddHistory(string action, Guid performedBy, string? comments = null, string? fieldsChanged = null, 
        string? oldValues = null, string? newValues = null) {
        var history = new InfraRequestHistory(Id, Status, action, performedBy, comments, fieldsChanged, oldValues, newValues);
        _history.Add(history);
    }
}