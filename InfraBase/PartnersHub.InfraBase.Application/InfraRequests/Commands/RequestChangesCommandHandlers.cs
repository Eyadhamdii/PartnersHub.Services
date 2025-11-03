using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Handler for RequestChangesByInfrabaseAdminCommand
/// Submit command handles both initial submission and resubmission
/// All requests go to PC Admin for approval first
/// </summary>
public class RequestChangesByInfrabaseAdminCommandHandler : IRequestHandler<RequestChangesByInfrabaseAdminCommand, bool> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestChangesByInfrabaseAdminCommandHandler(IInfrabaseRequestRepository repository, IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RequestChangesByInfrabaseAdminCommand command, CancellationToken cancellationToken) {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        var result = request.RequestChangesByInfrabaseAdmin(command.UserId, command.ChangeRequestDescription);
        if (result.IsFailure) {
            throw new InvalidOperationException(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}


