using MediatR;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Application.WhiteListIPs.Commands;

/// <summary>
/// Command to create a new WhiteListIP
/// </summary>
public record CreateWhiteListIPCommand : IRequest<Result<Guid>> {
    public string IPAddress { get; init; } = string.Empty;
    public DateTime ExpiryDate { get; init; }
    public string? Description { get; init; }
    public Guid CreatedBy { get; init; }
}