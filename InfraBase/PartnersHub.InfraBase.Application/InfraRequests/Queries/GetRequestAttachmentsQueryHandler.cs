using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Handler for GetRequestAttachmentsQuery
/// </summary>
public class GetRequestAttachmentsQueryHandler : IRequestHandler<GetRequestAttachmentsQuery, List<AttachmentDto>>
{
    private readonly IInfrabaseRequestRepository _repository;

    public GetRequestAttachmentsQueryHandler(IInfrabaseRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AttachmentDto>> Handle(GetRequestAttachmentsQuery query, CancellationToken cancellationToken)
    {
        var request = await _repository.GetByIdAsync(query.RequestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        var attachments = request.GetAttachments();

        return attachments.Select(a => new AttachmentDto
        {
            Id = a.Id,
            FileName = a.Metadata.FileName,
            FileExtension = a.Metadata.FileExtension,
            FileSizeInBytes = a.Metadata.FileSizeInBytes,
            FileSizeFormatted = a.Metadata.GetFileSizeFormatted(),
            ContentType = a.Metadata.ContentType,
            SharePointFileId = a.SharePointFileId,
            SharePointUrl = a.SharePointUrl,
            SharePointLibrary = a.SharePointLibrary,
            UploadedBy = a.UploadedBy,
            UploadedAt = a.UploadedAt
        }).ToList();
    }
}
