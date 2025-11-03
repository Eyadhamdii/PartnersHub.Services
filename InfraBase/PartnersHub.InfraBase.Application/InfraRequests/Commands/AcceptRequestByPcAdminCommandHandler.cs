using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public class AcceptRequestByPcAdminCommandHandler : IRequestHandler<AcceptRequestByPcAdminCommand, bool> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptRequestByPcAdminCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AcceptRequestByPcAdminCommand command, CancellationToken cancellationToken) {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        var result = request.AcceptByPcAdmin(command.UserId);
        if (result.IsFailure) {
            throw new InvalidOperationException(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
