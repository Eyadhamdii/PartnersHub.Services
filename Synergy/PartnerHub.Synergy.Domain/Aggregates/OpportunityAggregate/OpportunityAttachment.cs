using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;

public class OpportunityAttachment : Entity
{
    public Guid OpportunityId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string SharePointUrl { get; private set; } = null!;
    public string FileExtension { get; private set; } = null!;
    public long FileSizeInBytes { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string? UploadedBy { get; private set; }

    private OpportunityAttachment() { }

    public OpportunityAttachment(Guid opportunityId, string fileName, string sharePointUrl,
        long fileSizeInBytes, string? uploadedBy)
    {
        if (opportunityId == Guid.Empty)
            throw new ArgumentException("Opportunity ID is required", nameof(opportunityId));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required", nameof(fileName));

        if (string.IsNullOrWhiteSpace(sharePointUrl))
            throw new ArgumentException("SharePoint URL is required", nameof(sharePointUrl));

        if (!Uri.IsWellFormedUriString(sharePointUrl, UriKind.Absolute))
            throw new ArgumentException("SharePoint URL must be a valid absolute URL", nameof(sharePointUrl));

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("File must have a valid extension", nameof(fileName));

        OpportunityId = opportunityId;
        FileName = fileName.Trim();
        SharePointUrl = sharePointUrl.Trim();
        FileExtension = extension;
        FileSizeInBytes = fileSizeInBytes;
        UploadedAt = DateTime.UtcNow;
        UploadedBy = uploadedBy?.Trim();
    }
}
