using MediatR;
using PartnersHub.Synergy.Application.Interfaces;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Application.SuccessStories.Commands;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;
using PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Domain.Common;

public class CreateSuccessStoryCommandHandler : IRequestHandler<CreateSuccessStoryCommand, Result<Guid>>
{
    private readonly IOpportunityRepository _opportunityRepository;
    private readonly ISynergyCompanyRepository _synergyCompanyRepository;
    private readonly ISuccessStoryRepository _successStoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateSuccessStoryCommandHandler(IOpportunityRepository opportunityRepository, 
        ISynergyCompanyRepository synergyCompanyRepository,
        ISuccessStoryRepository successStoryRepository,
        IUnitOfWork unitOfWork)
    {

        _synergyCompanyRepository = synergyCompanyRepository;
        _opportunityRepository = opportunityRepository;
        _successStoryRepository = successStoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSuccessStoryCommand request, CancellationToken cancellationToken)
    {

        List<SynergyCompany> synergyCompanies = await _synergyCompanyRepository.GetByIdsAsync(request.CollaboratedProfiles);

        if (synergyCompanies.Count <= default(int) || synergyCompanies == null
            || request.CollaboratedProfiles.Count > synergyCompanies.Count)
            return Result<Guid>.Failure("Synergy company doesn't exist");

        var successStoryResult = SuccessStory.Create(request.CompanyId,
            request.Title, request.Description,
            request.SuccessStoryTypeId,
            request.StartDate,
            request.EndDate,
            request.SuccessStoryCollaborationStatusId,
            Guid.NewGuid(),
            Guid.NewGuid());
        if (successStoryResult.IsFailure)
            return Result<Guid>.Failure(successStoryResult.Error);

        var result = successStoryResult.Value.AddCollaboratedCompanies(request.CollaboratedProfiles);
        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error);

        result = successStoryResult.Value.AddAssociatedOpportunities(request.AssociatedOpportunities);
        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error);

        await _successStoryRepository.AddAsync(successStoryResult.Value);
        await _unitOfWork.SaveChangesAsync();



        return Result<Guid>.Success(successStoryResult.Value.Id);

    }
}