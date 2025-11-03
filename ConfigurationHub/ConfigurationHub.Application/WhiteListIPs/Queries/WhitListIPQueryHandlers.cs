using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;
using PartnersHub.ConfigurationHub.Application.Common.Interfaces.Repositories;

namespace PartnersHub.ConfigurationHub.Application.WhiteListIPs.Queries;

public class GetWhiteListIPByIdQueryHandler : IRequestHandler<GetWhiteListIPByIdQuery, WhiteListIPDto?> {
    private readonly IWhiteListIPRepository _repository;

    public GetWhiteListIPByIdQueryHandler(IWhiteListIPRepository repository) {
        _repository = repository;
    }

    public async Task<WhiteListIPDto?> Handle(GetWhiteListIPByIdQuery request, CancellationToken cancellationToken) {
        var whitelist = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (whitelist == null)
            return null;

        return new WhiteListIPDto {
            Id = whitelist.Id,
            IPAddress = whitelist.IPAddress.Value,
            ExpiryDate = whitelist.ExpiryDate,
            IsActive = whitelist.IsActive,
            Description = whitelist.Description,
            IsExpired = whitelist.IsExpired(),
            IsValid = whitelist.IsValid(),
            CreatedBy = whitelist.CreatedBy,
            CreatedAt = whitelist.CreatedAt,
            UpdatedBy = whitelist.UpdatedBy,
            UpdatedAt = whitelist.UpdatedAt
        };
    }
}

public class GetAllWhiteListIPsQueryHandler : IRequestHandler<GetAllWhiteListIPsQuery, IEnumerable<WhiteListIPDto>> {
    private readonly IWhiteListIPRepository _repository;

    public GetAllWhiteListIPsQueryHandler(IWhiteListIPRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<WhiteListIPDto>> Handle(GetAllWhiteListIPsQuery request, CancellationToken cancellationToken) {
        var whitelists = await _repository.GetAllAsync(cancellationToken);

        return whitelists.Select(w => new WhiteListIPDto {
            Id = w.Id,
            IPAddress = w.IPAddress.Value,
            ExpiryDate = w.ExpiryDate,
            IsActive = w.IsActive,
            Description = w.Description,
            IsExpired = w.IsExpired(),
            IsValid = w.IsValid(),
            CreatedBy = w.CreatedBy,
            CreatedAt = w.CreatedAt,
            UpdatedBy = w.UpdatedBy,
            UpdatedAt = w.UpdatedAt
        });
    }
}

public class GetActiveWhiteListIPsQueryHandler : IRequestHandler<GetActiveWhiteListIPsQuery, IEnumerable<WhiteListIPDto>> {
    private readonly IWhiteListIPRepository _repository;

    public GetActiveWhiteListIPsQueryHandler(IWhiteListIPRepository repository) {
        _repository = repository;
    }

    public async Task<IEnumerable<WhiteListIPDto>> Handle(GetActiveWhiteListIPsQuery request, CancellationToken cancellationToken) {
        var whitelists = await _repository.GetActiveAsync(cancellationToken);

        return whitelists.Select(w => new WhiteListIPDto {
            Id = w.Id,
            IPAddress = w.IPAddress.Value,
            ExpiryDate = w.ExpiryDate,
            IsActive = w.IsActive,
            Description = w.Description,
            IsExpired = w.IsExpired(),
            IsValid = w.IsValid(),
            CreatedBy = w.CreatedBy,
            CreatedAt = w.CreatedAt,
            UpdatedBy = w.UpdatedBy,
            UpdatedAt = w.UpdatedAt
        });
    }
}

public class IsIPWhitelistedQueryHandler : IRequestHandler<IsIPWhitelistedQuery, bool> {
    private readonly IWhiteListIPRepository _repository;

    public IsIPWhitelistedQueryHandler(IWhiteListIPRepository repository) {
        _repository = repository;
    }

    public async Task<bool> Handle(IsIPWhitelistedQuery request, CancellationToken cancellationToken) {
        return await _repository.IsIPWhitelistedAsync(request.IPAddress, cancellationToken);
    }
}