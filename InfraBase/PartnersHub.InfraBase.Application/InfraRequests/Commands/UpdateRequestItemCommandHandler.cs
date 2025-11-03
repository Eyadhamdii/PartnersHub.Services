using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Handler for updating a request item including financial distributions
/// Uses "replace all" approach for distributions - simpler for frontend
/// </summary>
public class UpdateRequestItemCommandHandler : IRequestHandler<UpdateRequestItemCommand, bool>
{
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRequestItemCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRequestItemCommand command, CancellationToken cancellationToken)
    {
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            throw new InvalidOperationException("Request not found");
        }

        // Update item quantity and unit price
        var result = request.UpdateItem(command.ItemId, command.Quantity, command.UnitPrice, command.UserId);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        // Get the updated item
        var item = request.GetItem(command.ItemId);
        if (item == null)
        {
            throw new InvalidOperationException($"Item with ID {command.ItemId} not found");
        }

        // Remove all existing financial distributions
        var existingDistributions = item.FinancialDistributions.ToList();
        foreach (var distribution in existingDistributions)
        {
            item.RemoveFinancialDistribution(distribution.Id);
        }

        // Add new financial distributions
        if (command.FinancialDistributions.Any())
        {
            foreach (var distDto in command.FinancialDistributions)
            {
                var addResult = item.AddFinancialDistribution(
                    distDto.AmountType,
                    distDto.Year,
                    distDto.Amount);

                if (addResult.IsFailure)
                {
                    throw new InvalidOperationException($"Failed to add financial distribution: {addResult.Error}");
                }
            }
        }

        // No need to call _repository.Update() - entity is already tracked
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
