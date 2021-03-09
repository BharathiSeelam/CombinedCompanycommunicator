// <copyright file="BotOptions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.CommonBot
{
    /// <summary>
    /// Options used for holding metadata for the bot.
    /// </summary>
    public class BotOptions
    {
        /// <summary>
        /// Gets or sets the Microsoft app ID for the user bot.
        /// </summary>
        public string UserAppId { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft app password for the user bot.
        /// </summary>
        public string UserAppPassword { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft app ID for the author bot.
        /// </summary>
        public string AuthorAppId { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft app password for the author bot.
        /// </summary>
        public string AuthorAppPassword { get; set; }

        /// <summary>
        /// Gets or sets the GrantType.
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// Gets or sets the Scope.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the Tenant ID.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets Storage Account.
        /// </summary>
        public string StorageAccount { get; set; }

        /// <summary>
        /// Gets or sets Blob Container Name.
        /// </summary>
        public string BlobContainerName { get; set; }

        /// <summary>
        /// Gets or sets Send Function App Base URL.
        /// </summary>
        public string SendFunctionAppBaseURL { get; set; }
    }
}
