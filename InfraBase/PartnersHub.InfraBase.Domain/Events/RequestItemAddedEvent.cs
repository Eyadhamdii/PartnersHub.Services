using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.Events;

public class RequestItemAddedEvent : DomainEvent {
    public Guid RequestId { get; }
    public Guid ItemId { get; }
    public string ItemName { get; }
    public decimal TotalAmount { get; }

    public RequestItemAddedEvent(Guid requestId, Guid itemId, string itemName, decimal totalAmount) {
        RequestId = requestId;
        ItemId = itemId;
        ItemName = itemName;
        TotalAmount = totalAmount;
    }
}