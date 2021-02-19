// <copyright file="DistributionList.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System;

    /// <summary>
    /// DistributionList model class.
    /// </summary>
    public class DistributionList
    {
        /// <summary>
        /// Gets or sets DLID.
        /// </summary>
        public string DLID { get; set; }

        /// <summary>
        /// Gets or sets DLName value.
        /// </summary>
        public string DLName { get; set; }

        /// <summary>
        /// Gets or sets the DLMail value.
        /// </summary>
        public string DLMail { get; set; }

        /// <summary>
        /// Gets or sets the DLMemberCount value.
        /// </summary>
        public int DLMemberCount { get; set; }

        /// <summary>
        /// Gets or sets the GroupType value.
        /// </summary>
        public string GroupType { get; set; }
    }
}
