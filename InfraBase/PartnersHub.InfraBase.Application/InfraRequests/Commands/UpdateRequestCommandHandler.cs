using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Handler for updating an infrastructure request including items and distributions
/// Provides a "replace entire structure" approach for easier frontend integration
/// </summary>
public class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, bool> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRequestCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRequestCommand command, CancellationToken cancellationToken) {
        // 1. Load request with items
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        // 2. Update basic information if provided
        var updateBasicInfoResult = request.UpdateBasicInformation(
            command.ProjectName,
            command.ProjectDescription,
            command.SectorId,
            command.SubSectorId,
            command.AssetTypeId,
            command.AssetTypeOtherDescription,
            command.TenderingStage,
            command.FundingModel,
            command.UserId);

        if (updateBasicInfoResult.IsFailure) {
            throw new InvalidOperationException(updateBasicInfoResult.Error);
        }

        // 3. Sync items (remove items not in the command)
        SyncItems(request, command);

        // 5. Save changes (entity already tracked, no Update() needed)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private void SyncItems(InfraRequest request, UpdateRequestCommand command) {
        // Remove items not in the command
        var currentItemIds = request.Items.Select(i => i.Id).ToList();
        var commandItemIds = command.Items
            .Where(i => i.Id.HasValue)
            .Select(i => i.Id!.Value)
            .ToList();
        var itemsToRemove = currentItemIds.Except(commandItemIds).ToList();

        foreach (var itemIdToRemove in itemsToRemove) {
            var removeResult = request.RemoveItem(itemIdToRemove, command.UserId);
            if (removeResult.IsFailure) {
                throw new InvalidOperationException($"Failed to remove item: {removeResult.Error}");
            }
        }

        // Add or update items
        foreach (var itemDto in command.Items) {
            if (itemDto.Id.HasValue) {
                UpdateExistingItem(request, itemDto, command.UserId);
            } else {
                AddItemWithDistributions(request, itemDto, command.UserId);
            }
        }
    }

    private void UpdateExistingItem(InfraRequest request, UpdateRequestItemDto itemDto, Guid userId) {
        var existingItem = request.GetItem(itemDto.Id!.Value);
        if (existingItem == null) {
            throw new InvalidOperationException($"Item with ID {itemDto.Id.Value} not found");
        }

        var updateResult = request.UpdateItem(
            itemDto.Id.Value,
            itemDto.Quantity,
            itemDto.UnitPrice,
            userId);

        if (updateResult.IsFailure) {
            throw new InvalidOperationException($"Failed to update item: {updateResult.Error}");
        }

        // Clear existing financial distributions
        var existingDistributions = existingItem.FinancialDistributions.ToList();
        foreach (var distribution in existingDistributions) {
            existingItem.RemoveFinancialDistribution(distribution.Id);
        }

        // Add new financial distributions
        if (itemDto.FinancialDistributions.Any()) {
            foreach (var distDto in itemDto.FinancialDistributions) {
                var distResult = existingItem.AddFinancialDistribution(
                    distDto.AmountType,
                    distDto.Year,
                    distDto.Amount);

                if (distResult.IsFailure) {
                    throw new InvalidOperationException($"Failed to add financial distribution: {distResult.Error}");
                }
            }
        }
    }

    private void AddItemWithDistributions(InfraRequest request, UpdateRequestItemDto itemDto, Guid userId) {
        var addItemResult = request.AddItem(
            itemDto.ItemCode,
            itemDto.ItemName,
            itemDto.UomId,
            itemDto.Quantity,
            itemDto.UnitPrice,
            userId);

        if (addItemResult.IsFailure) {
            throw new InvalidOperationException($"Failed to add item: {addItemResult.Error}");
        }

        // Get the added item (last in the collection)
        var addedItem = request.Items.Last();

        // Add financial distributions
        if (itemDto.FinancialDistributions.Any()) {
            foreach (var distDto in itemDto.FinancialDistributions) {
                var distResult = addedItem.AddFinancialDistribution(
                    distDto.AmountType,
                    distDto.Year,
                    distDto.Amount);

                if (distResult.IsFailure) {
                    throw new InvalidOperationException($"Failed to add financial distribution: {distResult.Error}");
                }
            }
        }
    }
}
