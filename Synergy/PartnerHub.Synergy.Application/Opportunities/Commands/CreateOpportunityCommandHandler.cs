using MediatR;
using PartnersHub.Synergy.Application.Interfaces;
using PartnersHub.Synergy.Application.Interfaces.Common;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;
using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Domain.Aggregates.SynergyCompanyAggregate;
using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Communities.Application.Communities.Commands;

public class CreateOpportunityCommandHandler : IRequestHandler<CreateOpportunityCommand, Result<Guid>>
{
    private readonly IOpportunityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOpportunityTypeRepository _opportunityTypeRepository;
    private readonly IThematicAreaRepository _thematicAreaRepository;
    private readonly ISynergyCompanyRepository _synergyCompanyRepository;
    private readonly IExpectedOutcomesRepository _expectedOutcomesRepository;
    private readonly ICollaborationRequirementRepository _collaborationRequirementRepository;
    private readonly IOpportunityRepository _opportunityRepository;
    private readonly IUserService _userService;

    public CreateOpportunityCommandHandler(IOpportunityRepository repository, IUnitOfWork unitOfWork,
        ISynergyCompanyRepository synergyCompanyRepository,
        IExpectedOutcomesRepository expectedOutcomesRepository,
        IOpportunityRepository opportunityRepository,
        IOpportunityTypeRepository opportunityTypeRepository,
        ICollaborationRequirementRepository collaborationRequirementRepository,
        IThematicAreaRepository thematicAreaRepository,
        IUserService userService


        )
    {
        _unitOfWork = unitOfWork;
        _synergyCompanyRepository = synergyCompanyRepository;
        _expectedOutcomesRepository = expectedOutcomesRepository;
        _opportunityTypeRepository = opportunityTypeRepository;
        _collaborationRequirementRepository = collaborationRequirementRepository;
        _thematicAreaRepository = thematicAreaRepository;
        _opportunityRepository = opportunityRepository;
        _userService = userService;

    }

    public async Task<Result<Guid>> Handle(CreateOpportunityCommand command, CancellationToken cancellationToken)
    {
        var result = await ValidateCommand(command);
        if (result.IsFailure)
        {
            return Result<Guid>.Failure(result.Error);
        }
        List<SynergyCompany> synergyCompanies = await _synergyCompanyRepository.GetByIdsAsync(command.CollaboratedProfiles);
        if (synergyCompanies == null || synergyCompanies.Count <= default(int) || command.CollaboratedProfiles.Count > synergyCompanies.Count)
            return Result<Guid>.Failure("Compaines not found");

        List<ExpectedOutcome> expectedOutcomes = await _expectedOutcomesRepository.GetByIdsAsync(command.ExpectedOutcomes);
        if (expectedOutcomes == null || expectedOutcomes.Count <= default(int)
            || command.ExpectedOutcomes.Count > expectedOutcomes.Count)
            return Result<Guid>.Failure("expectedoutcomes not found");

        List<CollaborationRequirement> collaborationRequirements = await _collaborationRequirementRepository.GetByIdsAsync(command.CollaborationRequirements);
        if (collaborationRequirements == null || collaborationRequirements.Count <= default(int)
            || command.CollaborationRequirements.Count > collaborationRequirements.Count)
            return Result<Guid>.Failure("collaboration Requirements not found");


        ThematicArea thematicArea = await _thematicAreaRepository.GetById(command.ThematicAreaId);


        OpportunityType opportunityType = await _opportunityTypeRepository.GetById(command.TypeId);


        var opportunity = Opportunity.Create(
            command.CompanyId,
            command.Title,
            command.Description,
            command.SectorName,
            command.SectorId,
            command.TypeId,
            command.ThematicAreaId,
            command.RepresentativeEmail,
            command.RepresentativeName,
            command.RepresentativePhone,
            command.RepresentativeTitle,
            command.CollaborationRationale,
            command.StartDate,
            command.EndDate,
            command.TermsAndConditionId,
            _userService.CurrentUserId);

        if (opportunity.IsFailure)
            return Result<Guid>.Failure(opportunity.Error);

        result = opportunity.Value.SetOpportunityType(opportunityType);
        if (result.IsFailure) return Result<Guid>.Failure(result.Error);

        result = opportunity.Value.AddCollaboratedCompanies(command.CollaboratedProfiles);
        if (result.IsFailure) return Result<Guid>.Failure(result.Error);

        result = opportunity.Value.AddCollaborationRequirement(collaborationRequirements, command.CollaborationRequirementOther);
        if (result.IsFailure) return Result<Guid>.Failure(result.Error);

        result = opportunity.Value.SetThematicArea(thematicArea);
        if (result.IsFailure) return Result<Guid>.Failure(result.Error);

        result = opportunity.Value.AddExpectedOutcome(expectedOutcomes, command.ExpectedOutcomeOther);
        if (result.IsFailure) return Result<Guid>.Failure(result.Error);


        result = opportunity.Value.Submit(_userService.CurrentUserId);
        if (result.IsFailure) return Result<Guid>.Failure(result.Error);

        await _opportunityRepository.AddAsync(opportunity.Value);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(opportunity.Value.Id);
    }
    private async Task<Result> ValidateCommand(CreateOpportunityCommand command)
    {

        if (!await _opportunityTypeRepository.ExistsAsync(command.TypeId))
        {
            return Result.Failure("Opportunity Type does not exist");
        }

        if (!await _thematicAreaRepository.ExistsAsync(command.ThematicAreaId))
        {
            return Result.Failure("Thematic Area does not exist");
        }
        if (await _opportunityRepository.IsOpportunityWithTitleAndCompanyExistsAsync(command.Title, command.CompanyId))
        {
            return Result.Failure("Opportunity with the same title exists");

        }

        return Result.Success();
    }
}