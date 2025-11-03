using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to update financial distribution amount
/// </summary>
public record UpdateFinancialDistributionCommand : IRequest<bool>
{
    public Guid RequestId { get; init; }
    public Guid ItemId { get; init; }
    public Guid DistributionId { get; init; }
    public decimal Amount { get; init; }
}
