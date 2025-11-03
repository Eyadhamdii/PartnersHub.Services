using PartnersHub.InfraBase.Domain.Common;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

/// <summary>
/// Represents a history entry for an InfraRequest
/// Can only be created through the InfraRequest aggregate root
/// </summary>
public class InfraRequestHistory : Entity
{
    public Guid RequestId { get; private set; }
    public RequestStatus Status { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? Comments { get; private set; }
    public Guid PerformedBy { get; private set; }
    public DateTime PerformedAt { get; private set; }
    public string? FieldsChanged { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }

    private InfraRequestHistory() { }

    internal InfraRequestHistory(Guid requestId, RequestStatus status, string action, Guid performedBy,
        string? comments = null, string? fieldsChanged = null, string? oldValues = null, string? newValues = null)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action is required");
        }

        RequestId = requestId;
        Status = status;
        Action = action;
        PerformedBy = performedBy;
        PerformedAt = DateTime.UtcNow;
        Comments = comments;
        FieldsChanged = fieldsChanged;
        OldValues = oldValues;
        NewValues = newValues;
    }
}