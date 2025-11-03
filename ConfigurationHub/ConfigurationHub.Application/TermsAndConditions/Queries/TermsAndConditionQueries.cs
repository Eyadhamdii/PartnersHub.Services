using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;
using PartnersHub.ConfigurationHub.Domain.Enums;

namespace PartnersHub.ConfigurationHub.Application.TermsAndConditions.Queries;

public record GetTermsByIdQuery : IRequest<TermsAndConditionDto?> {
    public Guid Id { get; init; }
}

public record GetAllTermsQuery : IRequest<IEnumerable<TermsAndConditionDto>>;

public record GetActiveTermsByTypeQuery : IRequest<TermsAndConditionDto?> {
    public TermsType Type { get; init; }
}

public record GetTermsByTypeQuery : IRequest<IEnumerable<TermsAndConditionDto>> {
    public TermsType Type { get; init; }
}