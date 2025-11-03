using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public class SubmitRequestCommandHandler : IRequestHandler<SubmitRequestCommand, string> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitRequestCommandHandler(IInfrabaseRequestRepository repository, IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(SubmitRequestCommand command, CancellationToken cancellationToken) {
        var request = await _repository.GetByIdWithItemsAsync(command.RequestId, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        await ApplyUpdatesIfProvided(request, command, cancellationToken);

        var submitResult = request.Submit(command.UserId, request.RequestCode!);
        if (submitResult.IsFailure) {
            throw new InvalidOperationException(submitResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return request.RequestCode!;
    }

    private async Task ApplyUpdatesIfProvided(
        Domain.Aggregates.InfraRequestAggregate.InfraRequest request,
        SubmitRequestCommand command,
        CancellationToken cancellationToken) {
        var hasUpdates = HasUpdates(command);
        if (!hasUpdates) {
            return;
        }

        if (HasBasicInfoUpdates(command)) {
            var updateResult = request.UpdateBasicInformation(
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

            if (updateResult.IsFailure) {
                throw new InvalidOperationException(updateResult.Error);
            }
        }

        if (command.Items != null && command.Items.Any()) {
            await UpdateItems(request, command, cancellationToken);
        }
    }

    private async Task UpdateItems(
        Domain.Aggregates.InfraRequestAggregate.InfraRequest request,
        SubmitRequestCommand command,
        CancellationToken cancellationToken) {
        var commandItemsDict = command.Items!
            .Where(i => i.Id.HasValue)
            .ToDictionary(i => i.Id!.Value);

        var itemsToRemove = request.Items
            .Where(i => !commandItemsDict.ContainsKey(i.Id))
            .Select(i => i.Id)
            .ToList();

        foreach (var itemId in itemsToRemove) {
            var removeResult = request.RemoveItem(itemId, command.UserId);
            if (removeResult.IsFailure) {
                throw new InvalidOperationException($"Failed to remove item: {removeResult.Error}");
            }
        }

        foreach (var itemDto in command.Items!) {
            if (itemDto.Id.HasValue) {
                await UpdateExistingItemOptimized(request, itemDto, command.UserId);
            } else {
                AddNewItem(request, itemDto, command.UserId);
            }
        }
    }

    private async Task UpdateExistingItemOptimized(
        Domain.Aggregates.InfraRequestAggregate.InfraRequest request,
        SubmitRequestItemDto itemDto,
        Guid userId) {
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

        SyncFinancialDistributions(existingItem, itemDto.FinancialDistributions);
        await Task.CompletedTask;
    }

    private void SyncFinancialDistributions(
        Domain.Aggregates.InfraRequestAggregate.InfraRequestItem item,
        List<SubmitFinancialDistributionDto> dtoDistributions) {
        var existingDistributions = item.FinancialDistributions.ToList();
        var dtoDistributionsDict = dtoDistributions
            .Where(d => d.Id.HasValue)
            .ToDictionary(d => d.Id!.Value);

        var distributionsToRemove = existingDistributions
            .Where(d => !dtoDistributionsDict.ContainsKey(d.Id))
            .ToList();

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
                var addResult = item.AddFinancialDistribution(
                    distDto.AmountType,
                    distDto.Year,
                    distDto.Amount);

                if (addResult.IsFailure) {
                    throw new InvalidOperationException($"Failed to add distribution: {addResult.Error}");
                }
            }
        }
    }

    private void AddNewItem(
        Domain.Aggregates.InfraRequestAggregate.InfraRequest request,
        SubmitRequestItemDto itemDto,
        Guid userId) {
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

        var newItem = request.Items.OrderByDescending(i => i.CreatedAt).First();
        foreach (var distDto in itemDto.FinancialDistributions) {
            var addDistResult = newItem.AddFinancialDistribution(
                distDto.AmountType,
                distDto.Year,
                distDto.Amount);

            if (addDistResult.IsFailure) {
                throw new InvalidOperationException($"Failed to add distribution: {addDistResult.Error}");
            }
        }
    }

    private bool HasBasicInfoUpdates(SubmitRequestCommand command) {
        return command.ProjectName != null ||
               command.ProjectDescription != null ||
               command.SectorId.HasValue ||
               command.SubSectorId.HasValue ||
               command.AssetTypeId.HasValue ||
               command.AssetTypeOtherDescription != null ||
               command.TenderingStage.HasValue ||
               command.FundingModel.HasValue ||
               command.StartConstructionQuarter.HasValue ||
               command.StartConstructionYear.HasValue ||
               command.EndConstructionQuarter.HasValue ||
               command.EndConstructionYear.HasValue;
    }

    private bool HasUpdates(SubmitRequestCommand command) {
        return HasBasicInfoUpdates(command) || (command.Items != null && command.Items.Any());
    }
}
