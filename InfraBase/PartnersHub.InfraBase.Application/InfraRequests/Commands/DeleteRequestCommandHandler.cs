using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Handler for deleting a draft request
/// Only allows deletion if request is in Draft status
/// </summary>
public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, bool> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRequestCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRequestCommand command, CancellationToken cancellationToken) {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        // Only allow deletion of Draft requests
        if (request.Status != RequestStatus.Draft) {
            throw new InvalidOperationException("Only Draft requests can be deleted");
        }

        // Optional: Verify user is the creator
        //if (request.CreatedBy != command.UserId) {
        //    throw new UnauthorizedAccessException("You can only delete your own requests");
        //}

        // Delete the request
        _repository.Delete(request);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
