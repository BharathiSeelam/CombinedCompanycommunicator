// <copyright file="Template.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System;

    /// <summary>
    /// Template model class.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets TemplateId.
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// Gets or sets TemplateName value.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the TemplateJSON value.
        /// </summary>
        public string TemplateJSON { get; set; }
    }
}
