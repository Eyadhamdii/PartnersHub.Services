using MediatR;
using PartnersHub.InnovationHub.Application.Common.Interfaces;
using PartnersHub.InnovationHub.Application.Common.Interfaces.Persistence;
using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;

namespace PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;

public class DeleteChallengeRequestDraftCommandHandler : IRequestHandler<DeleteChallengeRequestDraftCommand, Result<bool>>
{
    private readonly IChallengeRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteChallengeRequestDraftCommandHandler(
        IChallengeRequestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteChallengeRequestDraftCommand draftRequest, CancellationToken cancellationToken)
    {
        var request = await _repository.GetById(draftRequest.RequestId, cancellationToken);
        
        if (request == null)
            throw new InvalidOperationException("Challenge request not found");

        if (request.ChallengeStatus != ChallengeStatus.Draft)
            return Result<bool>.Failure("Only draft challenges can be deleted");

        await _repository.Delete(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
