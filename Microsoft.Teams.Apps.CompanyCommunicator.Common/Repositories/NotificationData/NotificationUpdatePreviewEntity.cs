// <copyright file="NotificationUpdatePreviewEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData
{
    using Microsoft.Bot.Schema;

    /// <summary>
    /// NotificationUpdatePreviewEntity.
    /// This entity holds the information required for preview, delete and edit.
    /// </summary>
    public class NotificationUpdatePreviewEntity
    {
        /// <summary>
        /// Gets or sets the ActionType value.
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// Gets or Sets NotificationDataEntity.
        /// </summary>
        public NotificationDataEntity NotificationDataEntity { get; set; }

        /// <summary>
        /// Gets or sets Conversationpreference.
        /// </summary>
        public ConversationReference ConversationReferance { get; set; }

        /// <summary>
        /// Gets or sets MessageActivity.
        /// </summary>
        public Activity MessageActivity { get; set; }

        /// <summary>
        /// Gets or sets ServiceUrl.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets AppID.
        /// </summary>
        public string AppID { get; set; }
    }
}
