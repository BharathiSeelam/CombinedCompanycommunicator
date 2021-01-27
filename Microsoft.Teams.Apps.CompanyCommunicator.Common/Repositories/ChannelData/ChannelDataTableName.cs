// <copyright file="ChannelDataTableName.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData
{
    /// <summary>
    /// Channel data table names.
    /// </summary>
    public static class ChannelDataTableName
        {
            /// <summary>
            /// Table name for the team data table.
            /// </summary>
            public static readonly string TableName = "ChannelData";

            /// <summary>
            /// Team data partition key name.
            /// </summary>
            public static readonly string ChannelDataPartition = "Default";
        }
}
