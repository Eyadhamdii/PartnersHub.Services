using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Handler for AddAttachmentCommand
/// </summary>
public class AddAttachmentCommandHandler : IRequestHandler<AddAttachmentCommand, Guid>
{
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddAttachmentCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddAttachmentCommand command, CancellationToken cancellationToken)
    {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        var result = request.AddAttachment(
            command.FileName,
            command.FileSizeInBytes,
            command.ContentType,
            command.SharePointFileId,
            command.SharePointUrl,
            command.SharePointLibrary,
            command.UploadedBy);

        if (result.IsFailure) {
            throw new InvalidOperationException(result.Error);
        }

        // Attachment is already tracked through aggregate - save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value!.Id;
    }
}

/// <summary>
/// Handler for RemoveAttachmentCommand
/// </summary>
public class RemoveAttachmentCommandHandler : IRequestHandler<RemoveAttachmentCommand, bool>
{
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAttachmentCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveAttachmentCommand command, CancellationToken cancellationToken)
    {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        var result = request.RemoveAttachment(command.AttachmentId, command.DeletedBy);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        // No need to call _repository.Update() - entity is already tracked
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
