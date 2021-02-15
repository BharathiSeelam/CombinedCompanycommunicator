// <copyright file="ISentUpdateandDeleteNotificationDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData
{
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;

    /// <summary>
    /// Interface for Sent Notification data Repository.
    /// </summary>
    public interface ISentUpdateandDeleteNotificationDataRepository : IRepository<SentNotificationDataEntity>
    {
        /// <summary>
        /// This method ensures the SentNotificationData table is created in the storage.
        /// This method should be called before kicking off an Azure function that uses the SentNotificationData table.
        /// Otherwise the app will crash.
        /// By design, Azure functions (in this app) do not create a table if it's absent.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task EnsureSentNotificationDataTableExistsAsync();

        /// <summary>
        /// Save exception error message in a notification data entity.
        /// </summary>
        /// <param name="notificationId">Notification data entity id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        // <returns><see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task DeleteFromPostAsync(
            string notificationId);

        /// <summary>
        /// Save exception error message in a notification data entity.
        /// </summary>
        /// <param name="notificationId">Notification data entity id.</param>
        /// <param name="updateSentNotificationDataEntity">updateSentNotificationDataEntity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        // <returns><see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task UpdateFromPostAsync(
            string notificationId,
            NotificationDataEntity updateSentNotificationDataEntity);
    }
}
