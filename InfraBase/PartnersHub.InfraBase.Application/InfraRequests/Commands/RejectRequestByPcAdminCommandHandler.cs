using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public class RejectRequestByPcAdminCommandHandler : IRequestHandler<RejectRequestByPcAdminCommand, bool> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectRequestByPcAdminCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RejectRequestByPcAdminCommand command, CancellationToken cancellationToken) {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        var result = request.RejectByPcAdmin(command.UserId, command.RejectionReason);
        if (result.IsFailure) {
            throw new InvalidOperationException(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
