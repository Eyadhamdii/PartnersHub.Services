using MediatR;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

public record AddRequestItemCommand : IRequest<Guid> {
    public Guid RequestId { get; init; }
    public Guid UserId { get; init; }  // Added for history tracking
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public Guid UomId { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
