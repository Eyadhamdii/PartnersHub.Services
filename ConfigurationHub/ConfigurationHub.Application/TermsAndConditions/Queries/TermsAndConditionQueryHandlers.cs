using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

namespace PartnersHub.ConfigurationHub.Application.TermsAndConditions.Queries;

public class GetTermsByIdQueryHandler : IRequestHandler<GetTermsByIdQuery, TermsAndConditionDto?> {
    private readonly ITermsAndConditionRepository _repository;

    public GetTermsByIdQueryHandler(ITermsAndConditionRepository repository) {
        _repository = repository;
    }

    public async Task<TermsAndConditionDto?> Handle(GetTermsByIdQuery request, CancellationToken cancellationToken) {
        var terms = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (terms == null)
            return null;

        return new TermsAndConditionDto {
            Id = terms.Id,
            Version = terms.Version,
            Type = terms.Type.ToString(),
            TitleAr = terms.TitleAr,
            TitleEn = terms.TitleEn,
            ContentAr = terms.ContentAr,
            ContentEn = terms.ContentEn,
            Status = terms.Status.ToString(),
            EffectiveDate = terms.EffectiveDate,
            ExpiryDate = terms.ExpiryDate,
            RequiresAcceptance = terms.RequiresAcceptance,
            IsActive = terms.IsActive(),
            CreatedBy = terms.CreatedBy,
            CreatedAt = terms.CreatedAt,
            UpdatedBy = terms.UpdatedBy,
            UpdatedAt = terms.UpdatedAt
        };
    }
}

public class GetAllTermsQueryHandler : IRequestHandler<GetAllTermsQuery, IEnumerable<TermsAndConditionDto>> {
    private readonly ITermsAndConditionRepository _repository;

    public GetAllTermsQueryHandler(ITermsAndConditionRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<TermsAndConditionDto>> Handle(GetAllTermsQuery request, CancellationToken cancellationToken) {
        var termsList = await _repository.GetAllAsync(cancellationToken);

        return termsList.Select(t => new TermsAndConditionDto {
            Id = t.Id,
            Version = t.Version,
            Type = t.Type.ToString(),
            TitleAr = t.TitleAr,
            TitleEn = t.TitleEn,
            ContentAr = t.ContentAr,
            ContentEn = t.ContentEn,
            Status = t.Status.ToString(),
            EffectiveDate = t.EffectiveDate,
            ExpiryDate = t.ExpiryDate,
            RequiresAcceptance = t.RequiresAcceptance,
            IsActive = t.IsActive(),
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            UpdatedBy = t.UpdatedBy,
            UpdatedAt = t.UpdatedAt
        });
    }
}

public class GetActiveTermsByTypeQueryHandler : IRequestHandler<GetActiveTermsByTypeQuery, TermsAndConditionDto?> {
    private readonly ITermsAndConditionRepository _repository;

    public GetActiveTermsByTypeQueryHandler(ITermsAndConditionRepository repository) {
        _repository = repository;
    }

    public async Task<TermsAndConditionDto?> Handle(GetActiveTermsByTypeQuery request, CancellationToken cancellationToken) {
        var terms = await _repository.GetActiveByTypeAsync(request.Type, cancellationToken);
        if (terms == null)
            return null;

        return new TermsAndConditionDto {
            Id = terms.Id,
            Version = terms.Version,
            Type = terms.Type.ToString(),
            TitleAr = terms.TitleAr,
            TitleEn = terms.TitleEn,
            ContentAr = terms.ContentAr,
            ContentEn = terms.ContentEn,
            Status = terms.Status.ToString(),
            EffectiveDate = terms.EffectiveDate,
            ExpiryDate = terms.ExpiryDate,
            RequiresAcceptance = terms.RequiresAcceptance,
            IsActive = terms.IsActive(),
            CreatedBy = terms.CreatedBy,
            CreatedAt = terms.CreatedAt,
            UpdatedBy = terms.UpdatedBy,
            UpdatedAt = terms.UpdatedAt
        };
    }
}

public class GetTermsByTypeQueryHandler : IRequestHandler<GetTermsByTypeQuery, IEnumerable<TermsAndConditionDto>> {
    private readonly ITermsAndConditionRepository _repository;

    public GetTermsByTypeQueryHandler(ITermsAndConditionRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<TermsAndConditionDto>> Handle(GetTermsByTypeQuery request, CancellationToken cancellationToken) {
        var termsList = await _repository.GetByTypeAsync(request.Type, cancellationToken);

        return termsList.Select(t => new TermsAndConditionDto {
            Id = t.Id,
            Version = t.Version,
            Type = t.Type.ToString(),
            TitleAr = t.TitleAr,
            TitleEn = t.TitleEn,
            ContentAr = t.ContentAr,
            ContentEn = t.ContentEn,
            Status = t.Status.ToString(),
            EffectiveDate = t.EffectiveDate,
            ExpiryDate = t.ExpiryDate,
            RequiresAcceptance = t.RequiresAcceptance,
            IsActive = t.IsActive(),
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            UpdatedBy = t.UpdatedBy,
            UpdatedAt = t.UpdatedAt
        });
    }
}