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
    /// This entity holds the information about a Channel.
    /// </summary>
    public class ChannelDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the id of the notification.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Channel name.
        /// </summary>
        public string ChannelName { get; set; }
    }
}
