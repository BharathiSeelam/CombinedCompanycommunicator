﻿// <copyright file="SentNotificationSummary.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;

    /// <summary>
    /// Sent notification summary model class.
    /// </summary>
    public class SentNotificationSummary
    {
        /// <summary>
        /// Gets or sets Notification Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Title value.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the image link of the notification's content.
        /// </summary>
        public string ImageLink { get; set; }

        /// <summary>
        /// Gets or sets the summary text of the notification's content.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets Account.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets Created DateTime value.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets Sent DateTime value.
        /// </summary>
        public DateTime? SentDate { get; set; }

        /// <summary>
        /// Gets or sets Edited DateTime value.
        /// </summary>
        public DateTime? Edited { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who have received the notification successfully.
        /// </summary>
        public int Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who failed in receiving the notification.
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients whose delivery status is unknown.
        /// </summary>
        public int? Unknown { get; set; }

        /// <summary>
        /// Gets or sets the total number of messages to be sent.
        /// </summary>
        public int TotalMessageCount { get; set; }

        /// <summary>
        /// Gets or sets the sending started date time.
        /// </summary>
        public DateTime? SendingStartedDate { get; set; }

        /// <summary>
        /// Gets or sets notification status. <see cref="NotificationStatus"/> for possible values.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Likes Count. <see cref="NotificationStatus"/> for possible values.
        /// </summary>
        public string Likes { get; set; }
    }
}
