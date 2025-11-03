using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;

public class SuccessStoryAttachment : Entity
{
    public Guid SuccessStoryId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string SharePointUrl { get; private set; } = null!;
    public string FileExtension { get; private set; } = null!;
    public long FileSizeInBytes { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string? UploadedBy { get; private set; }

    private SuccessStoryAttachment() { }

    public SuccessStoryAttachment(Guid successStoryId, string fileName, string sharePointUrl,
        long fileSizeInBytes, string? uploadedBy)
    {
        if (successStoryId == Guid.Empty)
            throw new ArgumentException("Success story ID is required", nameof(successStoryId));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required", nameof(fileName));

        if (string.IsNullOrWhiteSpace(sharePointUrl))
            throw new ArgumentException("SharePoint URL is required", nameof(sharePointUrl));

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("File must have a valid extension", nameof(fileName));

        SuccessStoryId = successStoryId;
        FileName = fileName.Trim();
        SharePointUrl = sharePointUrl.Trim();
        FileExtension = extension;
        FileSizeInBytes = fileSizeInBytes;
        UploadedAt = DateTime.UtcNow;
        UploadedBy = uploadedBy?.Trim();
    }
}
