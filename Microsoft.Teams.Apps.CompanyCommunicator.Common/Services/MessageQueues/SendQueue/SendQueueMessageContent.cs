// <copyright file="SendQueueMessageContent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.SendQueue
{
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;

    /// <summary>
    /// Azure service bus send queue message content class.
    /// </summary>
    public class SendQueueMessageContent
    {
        /// <summary>
        /// Gets or sets the notification id value.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the Activity id value.
        /// </summary>
        public string ActivtiyId { get; set; }

        /// <summary>
        /// Gets or sets the information about the recipient. This
        /// holds enough information for the Azure Function to send this
        /// recipient a notification.
        /// </summary>
        public RecipientData RecipientData { get; set; }

        /// <summary>
        /// Gets or sets the information about NotificationUpdatePreviewEntity.
        /// This holds information for Preview, Edit and Delete of Notifications.
        /// </summary>
        public NotificationUpdatePreviewEntity NotificationUpdatePreviewEntity { get; set; }
    }
}
