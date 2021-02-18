// <copyright file="TemplateDataTableNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData
{
    /// <summary>
    /// Templates table names.
    /// </summary>
    public static class TemplateDataTableNames
    {
        /// <summary>
        /// Table name for the template data table.
        /// </summary>
        public static readonly string TableName = "Templates";

        /// <summary>
        ///  template partition key name.
        /// </summary>
        public static readonly string TemplatePartition = "Default";
    }
}
