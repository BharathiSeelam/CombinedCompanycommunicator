// <copyright file="NotificationDetailsExport.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model
{
    using System;
    /// <summary>
    /// Sent notification summary model class.
    /// </summary>
    public class NotificationDetailsExport
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
        /// Gets or sets the sending started date time.
        /// </summary>
        public DateTime? SendingStartedDate { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string RecipientType { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user principal name.
        /// </summary>
        public string Upn { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the team id value.
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// Gets or sets the team id value.
        /// </summary>
        public string TeamName { get; set; }
        /// <summary>
        /// Gets or sets the delivery status value.
        /// </summary>
        public string DeliveryStatus { get; set; }

        /// <summary>
        /// Gets or sets the status reason value.
        /// </summary>
        public string StatusReason { get; set; }
    }
}
