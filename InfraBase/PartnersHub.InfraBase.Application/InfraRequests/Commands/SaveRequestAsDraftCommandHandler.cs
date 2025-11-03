using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Unified handler for saving requests as draft
/// Handles both create (RequestId is null) and update (RequestId provided)
/// </summary>
public class SaveRequestAsDraftCommandHandler : IRequestHandler<SaveRequestAsDraftCommand, Guid> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveRequestAsDraftCommandHandler(IInfrabaseRequestRepository repository, IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(SaveRequestAsDraftCommand command, CancellationToken cancellationToken) {
        var isCreate = !command.RequestId.HasValue || command.RequestId.Value == Guid.Empty;
        return isCreate ? await CreateNewRequest(command, cancellationToken) : await UpdateExistingRequest(command, cancellationToken);
    }

    private async Task<Guid> CreateNewRequest(SaveRequestAsDraftCommand command, CancellationToken cancellationToken) {
        // 1. Create the request
        var requestResult = InfraRequest.Create(command.ProjectName, command.ProjectDescription, command.SectorId, command.SubSectorId, 
            command.AssetTypeId, command.AssetTypeOtherDescription, command.TenderingStage, command.FundingModel, command.UserId, 
            command.CompanyId, command.CompanyName, command.StartConstructionQuarter, command.StartConstructionYear, 
            command.EndConstructionQuarter, command.EndConstructionYear);

        if (requestResult.IsFailure) {
            throw new InvalidOperationException(requestResult.Error);
        }

        var request = requestResult.Value!;

        // 2. Generate and assign request code
        var requestCode = await _repository.GetNextRequestCodeAsync(cancellationToken);
        request.AssignRequestCode(requestCode);

        // 3. Add items if provided
        if (command.Items.Any()) {
            foreach (var itemDto in command.Items) {
                AddItemWithDistributions(request, itemDto);
            }
        }

        // 4. Save to database
        await _repository.AddAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    private async Task<Guid> UpdateExistingRequest(SaveRequestAsDraftCommand command, CancellationToken cancellationToken) {
        var request = await _repository.GetByIdWithItemsAsync(command.RequestId!.Value, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        // 2. Update basic information
        var updateBasicInfoResult = request.UpdateBasicInformation(
            command.ProjectName,
            command.ProjectDescription,
            command.SectorId,
            command.SubSectorId,
            command.AssetTypeId,
            command.AssetTypeOtherDescription,
            command.TenderingStage,
            command.FundingModel,
            command.UserId,
            command.StartConstructionQuarter,
            command.StartConstructionYear,
            command.EndConstructionQuarter,
            command.EndConstructionYear);

        if (updateBasicInfoResult.IsFailure) {
            throw new InvalidOperationException(updateBasicInfoResult.Error);
        }

        // 3. Sync items
        SyncItems(request, command);

        // 4. Save changes - entity is already tracked
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    private void SyncItems(InfraRequest request, SaveRequestAsDraftCommand command) {
        var commandItemsDict = command.Items.Where(i => i.Id.HasValue).ToDictionary(i => i.Id!.Value);
        var itemsToRemove = request.Items.Where(i => !commandItemsDict.ContainsKey(i.Id)).Select(i => i.Id).ToList();

        foreach (var itemId in itemsToRemove) {
            var removeResult = request.RemoveItem(itemId, command.UserId);
            if (removeResult.IsFailure) {
                throw new InvalidOperationException($"Failed to remove item: {removeResult.Error}");
            }
        }

        foreach (var itemDto in command.Items) {
            if (itemDto.Id.HasValue) {
                UpdateExistingItemOptimized(request, itemDto, command.UserId);
            } else {
                AddItemWithDistributions(request, itemDto);
            }
        }
    }

    private void UpdateExistingItemOptimized(InfraRequest request, SaveRequestItemDto itemDto, Guid userId) {
        var existingItem = request.GetItem(itemDto.Id!.Value);
        if (existingItem == null) {
            throw new InvalidOperationException($"Item with ID {itemDto.Id.Value} not found");
        }

        var updateResult = request.UpdateItem(itemDto.Id.Value, itemDto.Quantity, itemDto.UnitPrice, userId);
        if (updateResult.IsFailure) {
            throw new InvalidOperationException($"Failed to update item: {updateResult.Error}");
        }

        SyncFinancialDistributions(existingItem, itemDto.FinancialDistributions);
    }

    private void SyncFinancialDistributions(InfraRequestItem item, List<SaveFinancialDistributionDto> dtoDistributions) {
        var existingDistributions = item.FinancialDistributions.ToList();
        var dtoDistributionsDict = dtoDistributions.Where(d => d.Id.HasValue).ToDictionary(d => d.Id!.Value);
        var distributionsToRemove = existingDistributions.Where(d => !dtoDistributionsDict.ContainsKey(d.Id)).ToList();

        foreach (var dist in distributionsToRemove) {
            item.RemoveFinancialDistribution(dist.Id);
        }

        foreach (var distDto in dtoDistributions) {
            if (distDto.Id.HasValue) {
                var existingDist = existingDistributions.FirstOrDefault(d => d.Id == distDto.Id.Value);
                if (existingDist != null) {
                    var updateResult = existingDist.UpdateAmount(distDto.Amount);
                    if (updateResult.IsFailure) {
                        throw new InvalidOperationException($"Failed to update distribution: {updateResult.Error}");
                    }
                }
            } else {
                var addResult = item.AddFinancialDistribution(distDto.AmountType, distDto.Year, distDto.Amount);
                if (addResult.IsFailure) {
                    throw new InvalidOperationException($"Failed to add distribution: {addResult.Error}");
                }
            }
        }
    }

    private void AddItemWithDistributions(InfraRequest request, SaveRequestItemDto itemDto) {
        var addItemResult = request.AddItem(itemDto.ItemCode, itemDto.ItemName, itemDto.UomId, itemDto.Quantity, 
            itemDto.UnitPrice, request.UpdatedBy ?? request.CreatedBy);

        if (addItemResult.IsFailure) {
            throw new InvalidOperationException($"Failed to add item: {addItemResult.Error}");
        }

        var addedItem = request.Items.Last();

        if (itemDto.FinancialDistributions.Any()) {
            foreach (var distDto in itemDto.FinancialDistributions) {
                var distResult = addedItem.AddFinancialDistribution(distDto.AmountType, distDto.Year, distDto.Amount);
                if (distResult.IsFailure) {
                    throw new InvalidOperationException($"Failed to add financial distribution: {distResult.Error}");
                }
            }
        }
    }
}
