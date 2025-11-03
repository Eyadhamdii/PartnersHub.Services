namespace PartnersHub.InfraBase.Application.Common.Interfaces;

/// <summary>
/// Service for sending notifications to users
/// Implementation will be provided by middleware integration
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends email notification
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body (supports HTML)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendEmailAsync(
        string to, 
        string subject, 
        string body, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends email notification to multiple recipients
    /// </summary>
    /// <param name="recipients">List of recipient email addresses</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body (supports HTML)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendEmailToMultipleAsync(
        IEnumerable<string> recipients,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends push notification to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendPushNotificationAsync(
        Guid userId, 
        string title, 
        string message, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends SMS notification
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="message">SMS message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendSmsAsync(
        string phoneNumber, 
        string message, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates in-app notification for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="link">Optional link to related resource</param>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CreateInAppNotificationAsync(
        Guid userId, 
        string title, 
        string message, 
        string? link = null,
        string notificationType = "Info",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates in-app notification for multiple users
    /// </summary>
    /// <param name="userIds">List of user IDs</param>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="link">Optional link to related resource</param>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CreateBulkInAppNotificationAsync(
        IEnumerable<Guid> userIds,
        string title,
        string message,
        string? link = null,
        string notificationType = "Info",
        CancellationToken cancellationToken = default);
}
