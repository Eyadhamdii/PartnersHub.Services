using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;
using PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Application.WhiteListIPs.Commands;

public class CreateWhiteListIPCommandHandler : IRequestHandler<CreateWhiteListIPCommand, Result<Guid>> {
    private readonly IWhiteListIPRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWhiteListIPCommandHandler(
        IWhiteListIPRepository repository,
        IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateWhiteListIPCommand request, CancellationToken cancellationToken) {
        // Check if IP already exists
        var existing = await _repository.GetByIPAddressAsync(request.IPAddress, cancellationToken);
        if (existing != null)
            return Result<Guid>.Failure("IP address is already whitelisted");

        // Create whitelist entry
        var result = WhiteListIP.Create(
            request.IPAddress,
            request.ExpiryDate,
            request.Description,
            request.CreatedBy);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error!);

        await _repository.AddAsync(result.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value!.Id);
    }
}