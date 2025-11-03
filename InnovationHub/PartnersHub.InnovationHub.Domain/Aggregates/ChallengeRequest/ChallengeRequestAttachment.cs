using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.ValueObjects;

namespace PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;

/// <summary>
/// Represents a file attachment associated with a challenge request.
/// Stores metadata and SharePoint references for document management.
/// </summary>
public class ChallengeRequestAttachment : Entity
{
    /// <summary>
    /// Gets the ID of the challenge request this attachment belongs to.
    /// </summary>
    public Guid ChallengeRequestId { get; private set; }
    
    /// <summary>
    /// Gets the attachment metadata (name, extension, size, format).
    /// </summary>
    public Attachment Metadata { get; private set; } = null!;

    // SharePoint references
    /// <summary>
    /// Gets the SharePoint file identifier.
    /// </summary>
    public string SharePointFileId { get; private set; } = string.Empty;
    
    /// <summary>
    /// Gets the SharePoint URL where the file is stored.
    /// </summary>
    public string SharePointUrl { get; private set; } = string.Empty;
    
    /// <summary>
    /// Gets the SharePoint library name.
    /// </summary>
    public string SharePointLibrary { get; private set; } = string.Empty;

    // Audit fields
    /// <summary>
    /// Gets the ID of the user who uploaded this attachment.
    /// </summary>
    public Guid UploadedBy { get; private set; }
    
    /// <summary>
    /// Gets the date and time when this attachment was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; private set; }
    
    /// <summary>
    /// Gets a value indicating whether this attachment has been soft-deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }
    
    /// <summary>
    /// Gets the ID of the user who deleted this attachment, if applicable.
    /// </summary>
    public Guid? DeletedBy { get; private set; }
    
    /// <summary>
    /// Gets the date and time when this attachment was deleted, if applicable.
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    // EF Core constructor
    private ChallengeRequestAttachment() { }

    private ChallengeRequestAttachment(
        Guid challengeRequestId,
        Attachment metadata,
        string sharePointFileId,
        string sharePointUrl,
        string sharePointLibrary,
        Guid uploadedBy)
    {
        ChallengeRequestId = challengeRequestId;
        Metadata = metadata;
        SharePointFileId = sharePointFileId;
        SharePointUrl = sharePointUrl;
        SharePointLibrary = sharePointLibrary;
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Creates a new challenge request attachment with validation.
    /// </summary>
    /// <param name="challengeRequestId">The ID of the challenge request.</param>
    /// <param name="metadata">The attachment metadata.</param>
    /// <param name="sharePointFileId">The SharePoint file ID.</param>
    /// <param name="sharePointUrl">The SharePoint URL.</param>
    /// <param name="sharePointLibrary">The SharePoint library name.</param>
    /// <param name="uploadedBy">The ID of the user uploading the file.</param>
    /// <returns>A Result containing the created attachment or an error message.</returns>
    public static Result<ChallengeRequestAttachment> Create(
        Guid challengeRequestId,
        Attachment metadata,
        string sharePointFileId,
        string sharePointUrl,
        string sharePointLibrary,
        Guid uploadedBy)
    {
        if (challengeRequestId == Guid.Empty)
            return Result<ChallengeRequestAttachment>.Failure("Challenge request ID is required");

        if (metadata == null)
            return Result<ChallengeRequestAttachment>.Failure("Attachment metadata is required");

        if (string.IsNullOrWhiteSpace(sharePointFileId))
            return Result<ChallengeRequestAttachment>.Failure("SharePoint file ID is required");

        if (string.IsNullOrWhiteSpace(sharePointUrl))
            return Result<ChallengeRequestAttachment>.Failure("SharePoint URL is required");

        if (string.IsNullOrWhiteSpace(sharePointLibrary))
            return Result<ChallengeRequestAttachment>.Failure("SharePoint library is required");

        if (uploadedBy == Guid.Empty)
            return Result<ChallengeRequestAttachment>.Failure("Uploaded by user is required");

        var attachment = new ChallengeRequestAttachment(
            challengeRequestId,
            metadata,
            sharePointFileId,
            sharePointUrl,
            sharePointLibrary,
            uploadedBy);

        return Result<ChallengeRequestAttachment>.Success(attachment);
    }

    /// <summary>
    /// Marks this attachment as deleted (soft delete).
    /// </summary>
    /// <param name="deletedBy">The ID of the user deleting the attachment.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result MarkAsDeleted(Guid deletedBy)
    {
        if (IsDeleted)
            return Result.Failure("Attachment is already deleted");

        if (deletedBy == Guid.Empty)
            return Result.Failure("Deleted by user is required");

        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;

        return Result.Success();
    }
}


