using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public class RemoveFinancialDistributionCommandHandler : IRequestHandler<RemoveFinancialDistributionCommand, bool>
{
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFinancialDistributionCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveFinancialDistributionCommand command, CancellationToken cancellationToken)
    {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        var item = request.GetItem(command.ItemId);
        if (item == null)
        {
            throw new InvalidOperationException("Item not found");
        }

        item.RemoveFinancialDistribution(command.DistributionId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
