// <copyright file="ChannelTemplate.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System;

    /// <summary>
    /// ChannelTemplate model class.
    /// </summary>
    public class ChannelTemplate
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
