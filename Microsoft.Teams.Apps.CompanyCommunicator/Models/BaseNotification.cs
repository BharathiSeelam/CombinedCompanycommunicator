﻿// <copyright file="BaseNotification.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System;

    /// <summary>
    /// Base notification model class.
    /// </summary>
    public class BaseNotification
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Title value.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Image Link value.
        /// </summary>
        public string ImageLink { get; set; }

        /// <summary>
        /// Gets or sets the Summary value.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the Author value.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the Channel value.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the TemplateID tet of the notification's content.
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// Gets or sets the Button Title value.
        /// </summary>
        public string ButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets the Button Link value.
        /// </summary>
        public string ButtonLink { get; set; }

        /// <summary>
        /// Gets or sets the Created DateTime value.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Partionkey value.(Added by Raja).
        /// </summary>
        public string PartitionKey { get; set; }
    }
}
