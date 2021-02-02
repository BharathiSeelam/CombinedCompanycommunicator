// <copyright file="DLUser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System;

    /// <summary>
    /// Channel model class.
    /// </summary>
    public class DLUser
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
