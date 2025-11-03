using MediatR;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public class UnarchiveChallengeRequestCommandHandler : IRequestHandler<UnarchiveChallengeRequestCommand, bool>
{
    private readonly IChallengeRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UnarchiveChallengeRequestCommandHandler(
        IChallengeRequestRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(UnarchiveChallengeRequestCommand archiveRequest, CancellationToken cancellationToken)
    {
        var request = await _repository.GetById(archiveRequest.RequestId, cancellationToken);

        if (request == null)
            throw new InvalidOperationException("Challenge request not found");

        var result = request.Unarchive(_currentUserService.UserId);

        if (result.IsFailure)
            throw new InvalidOperationException(result.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
