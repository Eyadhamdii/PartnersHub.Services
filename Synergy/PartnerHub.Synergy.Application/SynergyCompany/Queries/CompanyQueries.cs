using MediatR;

namespace PartnersHub.Synergy.Application.SynergyCompany.Queries;

public record GetRegisteredCompaniesQuery : IRequest<RegisteredCompaniesResultDto>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? SearchTerm { get; init; }
    public List<Guid>? SectorIds { get; init; }
    public List<string>? Countries { get; init; }
    public List<string>? Cities { get; init; }
}

public class RegisteredCompaniesResultDto
{
    public List<RegisteredCompanyCardDto> Companies { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public List<string> AvailableCountries { get; set; } = new();
    public List<string> AvailableCities { get; set; } = new();
}

public record GetCompanyDetailsQuery(Guid CompanyId) : IRequest<CompanyDetailsDto?>;
