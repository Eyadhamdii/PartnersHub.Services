using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Query to get all attachments for a request
/// </summary>
public record GetRequestAttachmentsQuery : IRequest<List<AttachmentDto>>
{
    public Guid RequestId { get; init; }
}
