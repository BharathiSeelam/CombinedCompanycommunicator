// <copyright file="IMessageService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;

    /// <summary>
    /// Teams message service.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Send message to a conversation.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="conversationId">Conversation Id.</param>
        /// <param name="serviceUrl">The service URL to use for sending the notification.</param>
        /// <param name="maxAttempts">Max attempts to send the message.</param>
        /// <param name="logger">Logger.</param>
        /// <returns>Send message response.</returns>
        public Task<SendMessageResponse> SendMessageAsync(
            IMessageActivity message,
            string conversationId,
            string serviceUrl,
            int maxAttempts,
            ILogger logger);

        /// <summary>
        /// Delete message to a conversation.
        /// </summary>
        /// <param name="notificationId">Message.</param>
        /// <param name="recipientId">Recipient Id.</param>
        /// <param name="serviceUrl">Service url.</param>
        /// <param name="tenantId">Tenant Id.</param>
        /// <param name="activityId">AadObject Id.</param>
        /// <returns>Send message response.</returns>
        public Task DeleteSentNotification(
           string notificationId,
           string recipientId,
           string serviceUrl,
           string tenantId,
           string activityId);

        /// <summary>
        /// Update message to a conversation.
        /// </summary>
        /// <param name="notificationDataEntity">sentNotificationEntity.</param>
        /// <param name="notificationId">Message.</param>
        /// <param name="recipientId">Recipient Id.</param>
        /// <param name="serviceUrl">Service url.</param>
        /// <param name="tenantId">Tenant Id.</param>
        /// /// <param name="activityId">AadObject Id.</param>
        /// <returns>Send message response.</returns>
        public Task UpdatePostSentNotification(
           NotificationDataEntity notificationDataEntity,
           string notificationId,
           string recipientId,
           string serviceUrl,
           string tenantId,
           string activityId);
    }
}
