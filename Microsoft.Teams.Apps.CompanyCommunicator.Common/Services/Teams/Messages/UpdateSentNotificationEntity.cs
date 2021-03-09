// <copyright file="UpdateSentNotificationEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;

    /// <summary>
    /// UpdateSentNotificationEntity.
    /// </summary>
    public class UpdateSentNotificationEntity
    {
        /// <summary>
        /// Gets or Sets Notification Id.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or Sets NotificationDataEntity. 
        /// </summary>
        public NotificationDataEntity NotificationEntity { get; set; }
    }
}
