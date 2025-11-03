using PartnersHub.InfraBase.Domain.Common;
using PartnersHub.InfraBase.Domain.ValueObjects;

namespace PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

/// <summary>
/// Represents an attachment associated with an InfraRequest
/// Stores reference to file in SharePoint and metadata
/// Can only be created through the InfraRequest aggregate root
/// </summary>
public class InfraRequestAttachment : Entity {
    public Guid RequestId { get; private set; }
    public AttachmentMetadata Metadata { get; private set; } = null!;
    public string SharePointFileId { get; private set; } = string.Empty;
    public string SharePointUrl { get; private set; } = string.Empty;
    public string SharePointLibrary { get; private set; } = string.Empty;
    
    // Audit
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private InfraRequestAttachment() { }

    internal InfraRequestAttachment(
        Guid requestId,
        string fileName,
        long fileSizeInBytes,
        string contentType,
        string sharePointFileId,
        string sharePointUrl,
        string sharePointLibrary,
        Guid uploadedBy) {

        var metadataResult = AttachmentMetadata.Create(fileName, fileSizeInBytes, contentType);
        if (metadataResult.IsFailure) {
            throw new ArgumentException(metadataResult.Error);
        }

        if (string.IsNullOrWhiteSpace(sharePointFileId)) {
            throw new ArgumentException("SharePoint file ID is required");
        }

        if (string.IsNullOrWhiteSpace(sharePointUrl)) {
            throw new ArgumentException("SharePoint URL is required");
        }

        if (string.IsNullOrWhiteSpace(sharePointLibrary)) {
            throw new ArgumentException("SharePoint library is required");
        }

        RequestId = requestId;
        Metadata = metadataResult.Value!;
        SharePointFileId = sharePointFileId;
        SharePointUrl = sharePointUrl;
        SharePointLibrary = sharePointLibrary;
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public Result<bool> MarkAsDeleted(Guid deletedBy) {
        if (IsDeleted) {
            return Result<bool>.Failure("Attachment is already deleted");
        }

        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
        
        return Result<bool>.Success(true);
    }
}
