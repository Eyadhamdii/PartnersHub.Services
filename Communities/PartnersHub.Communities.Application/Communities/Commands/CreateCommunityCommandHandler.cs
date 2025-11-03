using MediatR;
using PartnersHub.Communities.Application.Common.Interfaces;
using PartnersHub.Communities.Application.Common.Interfaces.Rpository;
using PartnersHub.Communities.Domain.Aggregates.Community;

namespace PartnersHub.Communities.Application.Communities.Commands;

public class CreateCommunityCommandHandler : IRequestHandler<CreateCommunityCommand, Guid>
{
    private readonly ICommunitiesRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommunityCommandHandler(ICommunitiesRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Community name is required", nameof(request.Name));

        var community = Community.Create(
            request.Name,
            request.Description,
            request.ImageUrl);

        await _repository.AddAsync(community);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return community.Id;
    }
}