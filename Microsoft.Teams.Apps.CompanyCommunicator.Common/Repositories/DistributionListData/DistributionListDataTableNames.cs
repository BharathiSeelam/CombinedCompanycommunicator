// <copyright file="DistributionListDataTableNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData
{
    /// <summary>
    /// DistributionLists table names.
    /// </summary>
    public static class DistributionListDataTableNames
    {
        /// <summary>
        /// Table name for the Distribution List data table.
        /// </summary>
        public static readonly string TableName = "DistributionLists";

        /// <summary>
        /// DistributionList partition key name.
        /// </summary>
        public static readonly string DistributionListPartition = "Default";
    }
}
