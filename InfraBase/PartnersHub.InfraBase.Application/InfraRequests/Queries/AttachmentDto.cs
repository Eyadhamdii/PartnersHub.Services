namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// DTO for attachment information
/// </summary>
public record AttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string FileExtension { get; init; } = string.Empty;
    public long FileSizeInBytes { get; init; }
    public string FileSizeFormatted { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string SharePointFileId { get; init; } = string.Empty;
    public string SharePointUrl { get; init; } = string.Empty;
    public string SharePointLibrary { get; init; } = string.Empty;
    public Guid UploadedBy { get; init; }
    public DateTime UploadedAt { get; init; }
}
