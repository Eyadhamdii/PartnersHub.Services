using MediatR;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Domain.Common;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public class CreateChallengeRequestCommandHandler : IRequestHandler<CreateChallengeRequestCommand, Result<Guid>>
{
    private readonly IChallengeRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssociatedProviderRepository _associatedProviderRepository;
    private readonly IAssociatedSectorRepository _associatedSectorRepository;

    public CreateChallengeRequestCommandHandler(
        IChallengeRequestRepository repository,
        IUnitOfWork unitOfWork,
        IAssociatedProviderRepository associatedProviderRepository,
        IAssociatedSectorRepository associatedSectorRepository)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _associatedProviderRepository = associatedProviderRepository;
        _associatedSectorRepository = associatedSectorRepository;
    }

    public async Task<Result<Guid>> Handle(CreateChallengeRequestCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate name
        if (await _repository.ExistsByNameAsync(request.Name, cancellationToken))
            return Result<Guid>.Failure("A challenge with this name already exists");

        var existingProvider = await _associatedProviderRepository.GetById(request.SourceCompany.id, cancellationToken);
        var existingSector = await _associatedSectorRepository.GetById(request.AssociatedSector.id, cancellationToken);

        var sourceCompanyId = existingProvider?.Id ?? request.SourceCompany.id;
        var sourceCompanyName = existingProvider?.Name ?? request.SourceCompany.name;

        var associatedSectorId = existingSector?.Id ?? request.AssociatedSector.id;
        var associatedSectorName = existingSector?.Name ?? request.AssociatedSector.name;

        // Create the challenge request using factory method
        var createResult = Domain.Aggregates.ChallengeRequest.ChallengeRequest.Create(
            request.UserId,
            request.Name,
            request.Description,
            sourceCompanyId,
            sourceCompanyName,
            associatedSectorId,
            associatedSectorName,
            request.SubmitterName,
            (int)request.PriorityLevel,
            request.IsDraft);

        // Return early if creation failed
        if (createResult.IsFailure)
            return Result<Guid>.Failure(createResult.Error!);

        // Persist to database
        await _repository.AddAsync(createResult.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(createResult.Value!.Id);
    }
}
