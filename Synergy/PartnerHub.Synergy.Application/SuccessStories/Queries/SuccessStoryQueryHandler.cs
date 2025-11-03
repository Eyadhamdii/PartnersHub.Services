using MediatR;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Application.Opportunities.DTOs;
using PartnersHub.Synergy.Application.Opportunity.Queries;
using PartnersHub.Synergy.Application.SuccessStories.DTOs;
using PartnersHub.Synergy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.SuccessStories.Queries
{

    public class SearchSuccessStoriesQueryHandler : IRequestHandler<SearchSuccessStoriesQuery, Result<SuccessStorySearchResponseDto>>
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ISynergyCompanyRepository _synergyCompanyRepository;
        private readonly ISuccessStoryRepository _successStoryRepository;
        public SearchSuccessStoriesQueryHandler(IOpportunityRepository opportunityRepository, ISynergyCompanyRepository synergyCompanyRepository, ISuccessStoryRepository successStoryRepository)
        {
            _opportunityRepository = opportunityRepository;
            _synergyCompanyRepository = synergyCompanyRepository;
            _successStoryRepository = successStoryRepository;
        }

        public async Task<Result<SuccessStorySearchResponseDto>> Handle(SearchSuccessStoriesQuery request, CancellationToken cancellationToken)
        {
            (List<SuccessStoryResponseDto> successStories, int count) = await _successStoryRepository.Search(pageNumber:request.PageNumber, pageSize:request.PageSize,
                partnerCompanyName:request.PartnerCompanyName, sectorIds:request.SectorIds, startDate:request.StartDate, endDate:request.EndDate, status:request.Status,
                companyId:request.CompanyId, collaborationTypeId:request.CollaborationType, sortBy:request.SortBy, sortDescending:request.SortDescending);




            return Result<SuccessStorySearchResponseDto>.Success(new SuccessStorySearchResponseDto() { SuccessStories = successStories, TotalCount = count});
        }


    }
    public class GetSuccessStoryByIdQueryHandler : IRequestHandler<GetSuccessStoryByIdQuery, Result<SuccessStoryResponseDto>>
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ISynergyCompanyRepository _synergyCompanyRepository;
        private readonly ISuccessStoryRepository _successStoryRepository;
        public GetSuccessStoryByIdQueryHandler(IOpportunityRepository opportunityRepository, ISynergyCompanyRepository synergyCompanyRepository, ISuccessStoryRepository successStoryRepository)
        {
            _opportunityRepository = opportunityRepository;
            _synergyCompanyRepository = synergyCompanyRepository;
            _successStoryRepository = successStoryRepository;
        }

        public async Task<Result<SuccessStoryResponseDto>> Handle(GetSuccessStoryByIdQuery request, CancellationToken cancellationToken)
        {
            SuccessStoryResponseDto successStory = await _successStoryRepository.GetByIdAsync(request.Id);



            if (successStory == null)
            {
                return Result<SuccessStoryResponseDto>.Failure("success story doesn't exist");
            }
            return Result<SuccessStoryResponseDto>.Success(successStory);
        }


    }
}
