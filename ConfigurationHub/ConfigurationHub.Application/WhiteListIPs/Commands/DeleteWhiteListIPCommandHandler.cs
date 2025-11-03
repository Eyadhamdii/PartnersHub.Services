using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Application.WhiteListIPs.Commands;

public class DeleteWhiteListIPCommandHandler : IRequestHandler<DeleteWhiteListIPCommand, Result<bool>> {
    private readonly IWhiteListIPRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWhiteListIPCommandHandler(IWhiteListIPRepository repository, IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteWhiteListIPCommand request, CancellationToken cancellationToken) {
        var whitelist = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (whitelist == null)
            return Result<bool>.Failure("WhiteListIP not found");

        _repository.Delete(whitelist);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}