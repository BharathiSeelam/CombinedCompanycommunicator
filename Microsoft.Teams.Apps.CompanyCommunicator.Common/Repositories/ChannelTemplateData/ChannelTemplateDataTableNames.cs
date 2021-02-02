// <copyright file="ChannelTemplateDataTableNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelTemplateData
{
    /// <summary>
    /// ChannelTemplates table names.
    /// </summary>
    public static class ChannelTemplateDataTableNames
    {
        /// <summary>
        /// Table name for the channel template data table.
        /// </summary>
        public static readonly string TableName = "ChannelTemplates";

        /// <summary>
        /// Channel template partition key name.
        /// </summary>
        public static readonly string ChannelTemplatePartition = "Default";
    }
}
