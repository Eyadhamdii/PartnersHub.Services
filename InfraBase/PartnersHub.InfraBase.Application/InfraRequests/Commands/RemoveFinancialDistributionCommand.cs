using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to remove financial distribution
/// </summary>
public record RemoveFinancialDistributionCommand : IRequest<bool>
{
    public Guid RequestId { get; init; }
    public Guid ItemId { get; init; }
    public Guid DistributionId { get; init; }
}
