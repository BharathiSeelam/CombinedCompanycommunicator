// <copyright file="IMessageReactionService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    /// <summary>
    /// Teams message reaction service.
    /// </summary>
    public interface IMessageReactionService
    {
        /// <summary>
        /// get message reaction data of a conversation.
        /// </summary>
        /// <param name="teamsID">teamsID.</param>
        /// <param name="channelID">channelID.</param>
        /// <param name="messageID">messageID.</param>
        /// <returns>Send message reaction.</returns>
        public Task<ChatMessage> GetMessagesAsync(string teamsID, string channelID, string messageID);

        /// <summary>
        /// getLoggedinUser Details.
        /// </summary>
        /// <returns>UserDetails.</returns>
        public Task<User> GetLoggedinUserDetails();
    }
}
