// <copyright file="DistributionListDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;

    /// <summary>
    /// DistributionList data entity class.
    /// This entity type holds the data for DistributionLists
    /// It holds the data for the content of the DistributionList.
    /// </summary>
    public class DistributionListDataEntity : TableEntity
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
        public Int32 DLMemberCount { get; set; }

        /// <summary>
        /// Gets or sets the GroupType value.
        /// </summary>
        public string GroupType { get; set; }
    }
}
