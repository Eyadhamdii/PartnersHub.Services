using MediatR;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;


namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public class ArchiveChallengeRequestCommandHandler : IRequestHandler<ArchiveChallengeRequestCommand, bool>
{
    private readonly IChallengeRequestRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveChallengeRequestCommandHandler(
        IChallengeRequestRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(ArchiveChallengeRequestCommand archiveRequest, CancellationToken cancellationToken)
    {
        var request = await _repository.GetById(archiveRequest.RequestId, cancellationToken);
        
        if (request == null)
            throw new InvalidOperationException("Challenge request not found");

        var result = request.Archive(_currentUserService.UserId);
        
        if (result.IsFailure)
            throw new InvalidOperationException(result.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
