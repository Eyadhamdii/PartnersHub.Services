using MediatR;
using PartnersHub.Synergy.Application.SuccessStories.DTOs;
using PartnersHub.Synergy.Domain.Common;
using PartnersHub.Synergy.Domain.Events;


namespace PartnersHub.Synergy.Application.SuccessStories.Queries
{
    public class GetSuccessStoryByIdQuery : IRequest<Result<SuccessStoryResponseDto>>
    {
        public Guid Id { get; set; }

    }
    public class SearchSuccessStoriesQuery : IRequest<Result<SuccessStorySearchResponseDto>>
    {
        public Guid? CompanyId { get; set; }
        public int? CollaborationType { get; set; }
        public List<Guid>? SectorIds { get; set; }
        public SuccessStoryStatus? Status { get; set; } // Pending, Published, Returned
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? PartnerCompanyName { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } // Title, SubmissionDate, Status
        public bool SortDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
