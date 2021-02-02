// <copyright file="DLUserDataTableNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DLUserData
{
    /// <summary>
    /// DLUsers table names.
    /// </summary>
    public static class DLUserDataTableNames
    {
        /// <summary>
        /// Table name for the DLUser data table.
        /// </summary>
        public static readonly string TableName = "DLUsers";

        /// <summary>
        /// DLUser partition key name.
        /// </summary>
        public static readonly string DLUserPartition = "Default";
    }
}
