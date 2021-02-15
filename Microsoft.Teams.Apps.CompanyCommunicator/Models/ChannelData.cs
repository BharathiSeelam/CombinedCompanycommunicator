// <copyright file="ChannelData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    /// <summary>
    /// Channel data model class.
    /// </summary>
    public class ChannelData
    {
        /// <summary>
        /// Gets or sets ChannelId.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets ChannelName value.
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// Gets or sets the ChannelDescription value.
        /// </summary>
        public string ChannelDescription { get; set; }

        /// <summary>
        /// Gets or sets the ChannelTemplate value.
        /// </summary>
        public string ChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the ChannelAdmins value.
        /// </summary>
        public string ChannelAdmins { get; set; }

        /// <summary>
        /// Gets or sets the ChannelAdminDLs value.
        /// </summary>
        public string ChannelAdminDLs { get; set; }

        /// <summary>
        /// Gets or sets the ChannelAdminEmail value.
        /// </summary>
        public string ChannelAdminEmail { get; set; }

        /// <summary>
        /// Gets or sets the TemplateJson value.
        /// </summary>
        public string TemplateJson { get; set; }
    }
}
