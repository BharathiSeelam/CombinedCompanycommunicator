// <copyright file="ChannelDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;

    /// <summary>
    /// Channel data entity class.
    /// This entity type holds the data for channels
    /// It holds the data for the content of the channel.
    /// </summary>
    public class ChannelDataEntity : TableEntity
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
    }
}
