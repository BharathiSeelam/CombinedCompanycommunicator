// <copyright file="PreviewDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Bot.Schema;

    public class PreviewDataEntity
    {
        /// <summary>
        /// Gets or sets Conversationpreference.
        /// </summary>
        public ConversationReference ConversationReferance { get; set; }

        /// <summary>
        /// Gets or sets MessageActivity.
        /// </summary>
        public Activity MessageActivity { get; set; }

        /// <summary>
        /// Gets or sets ServiceUrl.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets AppID.
        /// </summary>
        public string AppID { get; set; }

    }
}
