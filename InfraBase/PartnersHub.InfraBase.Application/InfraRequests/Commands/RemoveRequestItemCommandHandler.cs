using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public class RemoveRequestItemCommandHandler : IRequestHandler<RemoveRequestItemCommand, bool>
{
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRequestItemCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveRequestItemCommand command, CancellationToken cancellationToken)
    {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        var result = request.RemoveItem(command.ItemId, command.UserId);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        // No need to call _repository.Update() - entity is already tracked
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
