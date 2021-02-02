// <copyright file="DLUserDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DLUserData
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;

    /// <summary>
    /// DLUser data entity class.
    /// This entity type holds the data for DLUser
    /// It holds the data for the content of the DLUser.
    /// </summary>
    public class DLUserDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets UserID.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets DLName value.
        /// </summary>
        public string DLName { get; set; }

        /// <summary>
        /// Gets or sets the TeamsID value.
        /// </summary>
        public string TeamsID { get; set; }

        /// <summary>
        /// Gets or sets the UserEmail value.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the UserName value.
        /// </summary>
        public string UserName { get; set; }
    }
}
