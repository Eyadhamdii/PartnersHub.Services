using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public class AddRequestItemCommandHandler : IRequestHandler<AddRequestItemCommand, Guid> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddRequestItemCommandHandler(
        IInfrabaseRequestRepository repository,
        IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddRequestItemCommand command, CancellationToken cancellationToken) {
        // Get request with items
        var request = await _repository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null) {
            throw new InvalidOperationException("Request not found");
        }

        // UOM validation is handled by ConfigurationHub
        // Frontend provides valid UomId from ConfigurationHub API

        // Use provided item code (required from frontend)
        var itemCode = command.ItemCode;

        // Add item to request
        var result = request.AddItem(
            itemCode,
            command.ItemName,
            command.UomId,
            command.Quantity,
            command.UnitPrice,
            command.UserId);

        if (result.IsFailure) {
            throw new InvalidOperationException(result.Error);
        }

        // Get the added item (last in the collection)
        var addedItem = request.Items.Last();

        // No need to call _repository.Update() - entity is already tracked
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return the ID of the newly added item
        return addedItem.Id;
    }
}
