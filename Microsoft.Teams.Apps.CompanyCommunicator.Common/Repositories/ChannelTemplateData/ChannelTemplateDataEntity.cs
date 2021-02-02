// <copyright file="ChannelTemplateDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelTemplateData
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;

    /// <summary>
    /// Channel template data entity class.
    /// This entity type holds the data for channel templates
    /// It holds the data for the content of the channel templates.
    /// </summary>
    public class ChannelTemplateDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets ChannelTemplateId.
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// Gets or sets ChannelTemplateName value.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the ChannelTemplateJSON value.
        /// </summary>
        public string TemplateJSON { get; set; }
    }
}
