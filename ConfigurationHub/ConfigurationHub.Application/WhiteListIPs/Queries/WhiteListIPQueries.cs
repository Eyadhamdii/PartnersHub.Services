using MediatR;
using PartnersHub.ConfigurationHub.Application.Common.DTOs;

namespace PartnersHub.ConfigurationHub.Application.WhiteListIPs.Queries;

public record CheckIfIPIsInWhiteListQuery(string ipAddress) : IRequest<bool>;

public record GetWhiteListIPByIdQuery : IRequest<WhiteListIPDto?> {
    public Guid Id { get; init; }
}

public record GetAllWhiteListIPsQuery : IRequest<IEnumerable<WhiteListIPDto>>;

public record GetActiveWhiteListIPsQuery : IRequest<IEnumerable<WhiteListIPDto>>;

public record IsIPWhitelistedQuery : IRequest<bool> {
    public string IPAddress { get; init; } = string.Empty;
}