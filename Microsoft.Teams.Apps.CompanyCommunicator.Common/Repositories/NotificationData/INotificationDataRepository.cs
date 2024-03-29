﻿// <copyright file="INotificationDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// interface for Notification data Repository.
    /// </summary>
    public interface INotificationDataRepository : IRepository<NotificationDataEntity>
    {
        /// <summary>
        /// Gets table row key generator.
        /// </summary>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <summary>
        /// Get all draft notification entities from the table storage.
        /// </summary>
        /// <returns>All draft notification entities.</returns>
        public Task<IEnumerable<NotificationDataEntity>> GetAllDraftNotificationsAsync();

        /// <summary>
        /// Get the top 25 most recently sent notification entities from the table storage.
        /// </summary>
        /// <returns>The top 25 most recently sent notification entities.</returns>
        public Task<IEnumerable<NotificationDataEntity>> GetMostRecentSentNotificationsAsync();

        /// <summary>
        /// Get all notification data entities, and filter the result  by name.
        /// </summary>
        /// <param name="filter">The notification entity filter.</param>
        /// <param name="partitionkey">partitionkey.</param>
        /// <returns>The channel data entities of filter condition.</returns>
        public Task<IEnumerable<NotificationDataEntity>> GetFilterAsync(string filter, string partitionkey);

        /// <summary>
        /// Move a draft notification from draft to sent partition.
        /// </summary>
        /// <param name="draftNotificationEntity">The draft notification instance to be moved to the sent partition.</param>
        /// <returns>The new SentNotification ID.</returns>
        public Task<string> MoveDraftToSentPartitionAsync(NotificationDataEntity draftNotificationEntity);

        /// <summary>
        /// Update Sent Notification.
        /// </summary>
        /// <param name="updateSentNotificationDataEntity">The draft notification instance to be moved to the sent partition.</param>
        /// <param name="id">Id.</param>
        /// <returns>The new SentNotification ID.</returns>
        public Task<string> UpdateSentNotificationAsync(NotificationDataEntity updateSentNotificationDataEntity, string id);

        /// <summary>
        /// Duplicate an existing draft notification.
        /// </summary>
        /// <param name="notificationEntity">The notification entity to be duplicated.</param>
        /// <param name="createdBy">Created by.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public Task DuplicateDraftNotificationAsync(
            NotificationDataEntity notificationEntity,
            string createdBy);

        /// <summary>
        /// Updates notification status.
        /// </summary>
        /// <param name="notificationId">Notificaion Id.</param>
        /// <param name="status">Status.</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation.</returns>
        public Task UpdateNotificationStatusAsync(string notificationId, NotificationStatus status);

        /// <summary>
        /// Updates notification PublishOn.
        /// </summary>
        /// <param name="notificationId">Notificaion Id.</param>
        /// <param name="pubblishOn">PublishOn.</param>
        /// <returns><see cref="Task"/> representing the asynchronous operation.</returns>
        public Task UpdateNotificationSendDateAsync(string notificationId, string pubblishOn);

        /// <summary>
        /// Save exception error message in a notification data entity.
        /// </summary>
        /// <param name="notificationDataEntityId">Notification data entity id.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task SaveExceptionInNotificationDataEntityAsync(
            string notificationDataEntityId,
            string errorMessage);

        /// <summary>
        /// Save warning message in a notification data entity.
        /// </summary>
        /// <param name="notificationDataEntityId">Notification data entity id.</param>
        /// <param name="warningMessage">Warning message to be saved.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task SaveWarningInNotificationDataEntityAsync(
            string notificationDataEntityId,
            string warningMessage);
    }
}
