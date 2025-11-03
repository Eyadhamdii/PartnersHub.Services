using Microsoft.Extensions.Logging;
using PartnersHub.InfraBase.Application.Common.Interfaces;

namespace PartnersHub.InfraBase.Infrastructure.Services;

/// <summary>
/// Placeholder notification service implementation
/// Will be replaced with actual middleware integration
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to, 
        string subject, 
        string body, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending via middleware
        _logger.LogInformation(
            "Email notification (not sent - placeholder): To={To}, Subject={Subject}",
            to, subject);
        
        await Task.CompletedTask;
    }

    public async Task SendEmailToMultipleAsync(
        IEnumerable<string> recipients,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual bulk email sending via middleware
        _logger.LogInformation(
            "Bulk email notification (not sent - placeholder): Recipients={Count}, Subject={Subject}",
            recipients.Count(), subject);
        
        await Task.CompletedTask;
    }

    public async Task SendPushNotificationAsync(
        Guid userId, 
        string title, 
        string message, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual push notification via middleware
        _logger.LogInformation(
            "Push notification (not sent - placeholder): UserId={UserId}, Title={Title}",
            userId, title);
        
        await Task.CompletedTask;
    }

    public async Task SendSmsAsync(
        string phoneNumber, 
        string message, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual SMS sending via middleware
        _logger.LogInformation(
            "SMS notification (not sent - placeholder): Phone={Phone}, Message={Message}",
            phoneNumber, message);
        
        await Task.CompletedTask;
    }

    public async Task CreateInAppNotificationAsync(
        Guid userId, 
        string title, 
        string message, 
        string? link = null,
        string notificationType = "Info",
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual in-app notification via middleware
        _logger.LogInformation(
            "In-app notification (not sent - placeholder): UserId={UserId}, Title={Title}, Type={Type}, Link={Link}",
            userId, title, notificationType, link ?? "none");
        
        await Task.CompletedTask;
    }

    public async Task CreateBulkInAppNotificationAsync(
        IEnumerable<Guid> userIds,
        string title,
        string message,
        string? link = null,
        string notificationType = "Info",
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual bulk in-app notification via middleware
        _logger.LogInformation(
            "Bulk in-app notification (not sent - placeholder): UserCount={Count}, Title={Title}, Type={Type}",
            userIds.Count(), title, notificationType);
        
        await Task.CompletedTask;
    }
}
