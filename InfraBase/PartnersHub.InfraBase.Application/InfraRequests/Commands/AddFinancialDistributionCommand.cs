using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public record AddFinancialDistributionCommand : IRequest<Guid> {
    public Guid RequestId { get; init; }
    public Guid ItemId { get; init; }
    public AmountType AmountType { get; init; }
    public int Year { get; init; }
    public decimal Amount { get; init; }
}
