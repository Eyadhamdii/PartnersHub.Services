using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to add an attachment to a request
/// </summary>
public record AddAttachmentCommand : IRequest<Guid>
{
    public Guid RequestId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public long FileSizeInBytes { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public string SharePointFileId { get; init; } = string.Empty;
    public string SharePointUrl { get; init; } = string.Empty;
    public string SharePointLibrary { get; init; } = string.Empty;
    public Guid UploadedBy { get; init; }
}

/// <summary>
/// Command to remove an attachment from a request
/// </summary>
public record RemoveAttachmentCommand : IRequest<bool>
{
    public Guid RequestId { get; init; }
    public Guid AttachmentId { get; init; }
    public Guid DeletedBy { get; init; }
}
