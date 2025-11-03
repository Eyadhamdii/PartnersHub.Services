using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Handler for creating an infrastructure request with items and distributions in one operation
/// Flow: SaveAsDraft > Submit > Approval workflow
/// </summary>
public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Guid> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRequestCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateRequestCommand command, CancellationToken cancellationToken) {
        // 1. Create the request
        var requestResult = InfraRequest.Create(
            command.ProjectName,
            command.ProjectDescription,
            command.SectorId,
            command.SubSectorId,
            command.AssetTypeId,
            command.AssetTypeOtherDescription,
            command.TenderingStage,
            command.FundingModel,
            command.CreatedBy,
            command.CompanyId,
            command.CompanyName);

        if (requestResult.IsFailure) {
            throw new InvalidOperationException(requestResult.Error);
        }

        var request = requestResult.Value!;

        // 2. Generate and assign request code at creation
        var requestCode = await _repository.GetNextRequestCodeAsync(cancellationToken);
        request.AssignRequestCode(requestCode);

        // 3. Add items if provided
        if (command.Items.Any()) {
            foreach (var itemDto in command.Items) {
                // Add item to request
                var addItemResult = request.AddItem(
                    itemDto.ItemCode,
                    itemDto.ItemName,
                    itemDto.UomId,
                    itemDto.Quantity,
                    itemDto.UnitPrice,
                    request.CreatedBy);  // Use CreatedBy for initial creation

                if (addItemResult.IsFailure) {
                    throw new InvalidOperationException($"Failed to add item '{itemDto.ItemName}': {addItemResult.Error}");
                }

                // Get the newly added item
                var addedItem = request.Items.OrderByDescending(i => i.CreatedAt).First();

                // 4. Add financial distributions to the item if provided
                if (itemDto.FinancialDistributions.Any()) {
                    foreach (var distDto in itemDto.FinancialDistributions) {
                        var addDistResult = addedItem.AddFinancialDistribution(
                            distDto.AmountType,
                            distDto.Year,
                            distDto.Amount);

                        if (addDistResult.IsFailure) {
                            throw new InvalidOperationException(
                                $"Failed to add financial distribution for item '{itemDto.ItemName}': {addDistResult.Error}");
                        }
                    }
                }
            }
        }

        // 5. Save to database
        await _repository.AddAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}
