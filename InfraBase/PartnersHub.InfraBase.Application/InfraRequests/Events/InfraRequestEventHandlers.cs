using MediatR;
using Microsoft.Extensions.Logging;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Events;

namespace PartnersHub.InfraBase.Application.InfraRequests.Events;

/// <summary>
/// Handler for RequestSubmittedEvent
/// Notifies PC Admins about new request for review
/// </summary>
public class RequestSubmittedEventHandler : INotificationHandler<RequestSubmittedEvent>
{
    private readonly ILogger<RequestSubmittedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IInfrabaseRequestRepository _repository;

    public RequestSubmittedEventHandler(
        ILogger<RequestSubmittedEventHandler> logger,
        INotificationService notificationService,
        IInfrabaseRequestRepository repository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _repository = repository;
    }

    public async Task Handle(RequestSubmittedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Request {RequestId} with code {RequestCode} was submitted with status {Status} by user {UserId}",
            notification.RequestId,
            notification.RequestCode,
            notification.Status,
            notification.SubmittedBy);

        try
        {
            var request = await _repository.GetByIdAsync(notification.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found for notification", notification.RequestId);
                return;
            }

            // Notify submitter (confirmation)
            await _notificationService.CreateInAppNotificationAsync(
                userId: notification.SubmittedBy,
                title: "Request Submitted Successfully",
                message: $"Your request {notification.RequestCode} has been submitted and is now pending PC Admin review.",
                link: $"/requests/{notification.RequestId}",
                notificationType: "Success",
                cancellationToken);

            // TODO: Get PC Admin user IDs from configuration or user service
            // For now, just log that we would notify them
            _logger.LogInformation("PC Admins should be notified about request {RequestCode} for review", notification.RequestCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for request submission {RequestId}", notification.RequestId);
        }
    }
}

/// <summary>
/// Handler for RequestAcceptedByPcAdminEvent
/// Notifies requester and forwards to Infrabase Admin
/// </summary>
public class RequestAcceptedByPcAdminEventHandler : INotificationHandler<RequestAcceptedByPcAdminEvent>
{
    private readonly ILogger<RequestAcceptedByPcAdminEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IInfrabaseRequestRepository _repository;

    public RequestAcceptedByPcAdminEventHandler(
        ILogger<RequestAcceptedByPcAdminEventHandler> logger,
        INotificationService notificationService,
        IInfrabaseRequestRepository repository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _repository = repository;
    }

    public async Task Handle(RequestAcceptedByPcAdminEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Request {RequestId} with code {RequestCode} was accepted by PC Admin {UserId}",
            notification.RequestId,
            notification.RequestCode,
            notification.AcceptedBy);

        try
        {
            var request = await _repository.GetByIdAsync(notification.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found for notification", notification.RequestId);
                return;
            }

            // Notify requester
            await _notificationService.CreateInAppNotificationAsync(
                userId: request.CreatedBy,
                title: "Request Accepted by PC Admin",
                message: $"Your request {notification.RequestCode} has been accepted by the PC Admin and forwarded to Infrabase Admin for final review.",
                link: $"/requests/{notification.RequestId}",
                notificationType: "Success",
                cancellationToken);

            // TODO: Notify Infrabase Admins about new request pending their review
            _logger.LogInformation("Infrabase Admins should be notified about request {RequestCode} for review", notification.RequestCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for PC Admin acceptance {RequestId}", notification.RequestId);
        }
    }
}

/// <summary>
/// Handler for RequestRejectedByPcAdminEvent
/// Notifies requester with rejection reason and guidance
/// </summary>
public class RequestRejectedByPcAdminEventHandler : INotificationHandler<RequestRejectedByPcAdminEvent>
{
    private readonly ILogger<RequestRejectedByPcAdminEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IInfrabaseRequestRepository _repository;

    public RequestRejectedByPcAdminEventHandler(
        ILogger<RequestRejectedByPcAdminEventHandler> logger,
        INotificationService notificationService,
        IInfrabaseRequestRepository repository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _repository = repository;
    }

    public async Task Handle(RequestRejectedByPcAdminEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Request {RequestId} with code {RequestCode} was rejected by PC Admin {UserId}. Reason: {Reason}",
            notification.RequestId,
            notification.RequestCode,
            notification.RejectedBy,
            notification.RejectionReason);

        try
        {
            var request = await _repository.GetByIdAsync(notification.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found for notification", notification.RequestId);
                return;
            }

            // Notify requester with rejection reason
            await _notificationService.CreateInAppNotificationAsync(
                userId: request.CreatedBy,
                title: "Request Rejected - Action Required",
                message: $"Request {notification.RequestCode} was rejected by PC Admin. Reason: {notification.RejectionReason}. Please review and resubmit.",
                link: $"/requests/{notification.RequestId}/edit",
                notificationType: "Warning",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for PC Admin rejection {RequestId}", notification.RequestId);
        }
    }
}

/// <summary>
/// Handler for RequestApprovedByInfrabaseAdminEvent
/// Notifies all stakeholders about final approval
/// </summary>
public class RequestApprovedByInfrabaseAdminEventHandler : INotificationHandler<RequestApprovedByInfrabaseAdminEvent>
{
    private readonly ILogger<RequestApprovedByInfrabaseAdminEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IInfrabaseRequestRepository _repository;

    public RequestApprovedByInfrabaseAdminEventHandler(
        ILogger<RequestApprovedByInfrabaseAdminEventHandler> logger,
        INotificationService notificationService,
        IInfrabaseRequestRepository repository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _repository = repository;
    }

    public async Task Handle(RequestApprovedByInfrabaseAdminEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Request {RequestId} with code {RequestCode} received final approval from Infrabase Admin {UserId}",
            notification.RequestId,
            notification.RequestCode,
            notification.ApprovedBy);

        try
        {
            var request = await _repository.GetByIdAsync(notification.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found for notification", notification.RequestId);
                return;
            }

            // Notify requester
            await _notificationService.CreateInAppNotificationAsync(
                userId: request.CreatedBy,
                title: "Request Approved!",
                message: $"Congratulations! Your request {notification.RequestCode} has received final approval.",
                link: $"/requests/{notification.RequestId}",
                notificationType: "Success",
                cancellationToken);

            // Notify PC Admin who initially approved
            if (request.ApprovedBy.HasValue && request.ApprovedBy.Value != notification.ApprovedBy)
            {
                await _notificationService.CreateInAppNotificationAsync(
                    userId: request.ApprovedBy.Value,
                    title: "Request Finalized",
                    message: $"Request {notification.RequestCode} has been given final approval by Infrabase Admin.",
                    link: $"/requests/{notification.RequestId}",
                    notificationType: "Info",
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for Infrabase Admin approval {RequestId}", notification.RequestId);
        }
    }
}

/// <summary>
/// Handler for RequestRejectedByInfrabaseAdminEvent
/// Notifies requester and stakeholders about rejection
/// </summary>
public class RequestRejectedByInfrabaseAdminEventHandler : INotificationHandler<RequestRejectedByInfrabaseAdminEvent>
{
    private readonly ILogger<RequestRejectedByInfrabaseAdminEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IInfrabaseRequestRepository _repository;

    public RequestRejectedByInfrabaseAdminEventHandler(
        ILogger<RequestRejectedByInfrabaseAdminEventHandler> logger,
        INotificationService notificationService,
        IInfrabaseRequestRepository repository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _repository = repository;
    }

    public async Task Handle(RequestRejectedByInfrabaseAdminEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Request {RequestId} with code {RequestCode} was rejected by Infrabase Admin {UserId}. Reason: {Reason}",
            notification.RequestId,
            notification.RequestCode,
            notification.RejectedBy,
            notification.RejectionReason);

        try
        {
            var request = await _repository.GetByIdAsync(notification.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found for notification", notification.RequestId);
                return;
            }

            // Notify requester
            await _notificationService.CreateInAppNotificationAsync(
                userId: request.CreatedBy,
                title: "Request Rejected by Infrabase Admin",
                message: $"Request {notification.RequestCode} was rejected. Reason: {notification.RejectionReason}. You may edit and resubmit your request.",
                link: $"/requests/{notification.RequestId}/edit",
                notificationType: "Error",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for Infrabase Admin rejection {RequestId}", notification.RequestId);
        }
    }
}

/// <summary>
/// Handler for RequestChangesRequestedByInfrabaseAdminEvent
/// Notifies requester with change instructions
/// </summary>
public class RequestChangesRequestedEventHandler : INotificationHandler<RequestChangesRequestedByInfrabaseAdminEvent>
{
    private readonly ILogger<RequestChangesRequestedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IInfrabaseRequestRepository _repository;

    public RequestChangesRequestedEventHandler(
        ILogger<RequestChangesRequestedEventHandler> logger,
        INotificationService notificationService,
        IInfrabaseRequestRepository repository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _repository = repository;
    }

    public async Task Handle(RequestChangesRequestedByInfrabaseAdminEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Changes requested for request {RequestId} with code {RequestCode} by {UserId}",
            notification.RequestId,
            notification.RequestCode,
            notification.RequestedBy);

        try
        {
            var request = await _repository.GetByIdAsync(notification.RequestId, cancellationToken);
            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found for notification", notification.RequestId);
                return;
            }

            // Notify requester with change instructions
            await _notificationService.CreateInAppNotificationAsync(
                userId: request.CreatedBy,
                title: "Changes Requested",
                message: $"Infrabase Admin requested changes to {notification.RequestCode}. Description: {notification.ChangeDescription}",
                link: $"/requests/{notification.RequestId}/edit",
                notificationType: "Warning",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for change request {RequestId}", notification.RequestId);
        }
    }
}

/// <summary>
/// Handler for RequestSavedAsDraftEvent
/// Simple confirmation notification
/// </summary>
public class RequestSavedAsDraftEventHandler : INotificationHandler<RequestSavedAsDraftEvent>
{
    private readonly ILogger<RequestSavedAsDraftEventHandler> _logger;

    public RequestSavedAsDraftEventHandler(ILogger<RequestSavedAsDraftEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(RequestSavedAsDraftEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Request {RequestId} for project {ProjectName} was saved as draft by user {UserId}",
            notification.RequestId,
            notification.ProjectName,
            notification.SavedBy);

        // No notification needed for draft save (just logging)
        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler for RequestItemAddedEvent
/// Logs item addition for audit trail
/// </summary>
public class RequestItemAddedEventHandler : INotificationHandler<RequestItemAddedEvent>
{
    private readonly ILogger<RequestItemAddedEventHandler> _logger;

    public RequestItemAddedEventHandler(ILogger<RequestItemAddedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(RequestItemAddedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Item {ItemId} '{ItemName}' with amount {Amount:C} was added to request {RequestId}",
            notification.ItemId,
            notification.ItemName,
            notification.TotalAmount,
            notification.RequestId);

        // No notification needed for item addition (just logging)
        await Task.CompletedTask;
    }
}
