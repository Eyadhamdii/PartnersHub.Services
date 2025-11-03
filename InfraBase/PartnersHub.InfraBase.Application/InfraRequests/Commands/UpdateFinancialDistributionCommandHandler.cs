using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public class UpdateFinancialDistributionCommandHandler : IRequestHandler<UpdateFinancialDistributionCommand, bool>
{
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFinancialDistributionCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateFinancialDistributionCommand command, CancellationToken cancellationToken)
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

        var distribution = item.FinancialDistributions.FirstOrDefault(fd => fd.Id == command.DistributionId);
        if (distribution == null)
        {
            throw new InvalidOperationException("Financial distribution not found");
        }

        var result = distribution.UpdateAmount(command.Amount);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
