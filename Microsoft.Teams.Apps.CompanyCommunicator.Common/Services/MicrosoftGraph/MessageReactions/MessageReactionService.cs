// <copyright file="MessageReactionService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    /// <summary>
    /// Message reaction service.
    /// </summary>
    public class MessageReactionService : IMessageReactionService
    {
        private readonly IGraphServiceClient graphServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReactionService"/> class.
        /// </summary>
        /// <param name="graphServiceClient">graph service client.</param>
        public MessageReactionService(IGraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient ?? throw new ArgumentNullException(nameof(graphServiceClient));
        }

        /// <summary>
        /// Get the Team channel message.
        /// </summary>
        /// <param name="teamsID">teams id oaram.</param>
        /// <param name="channelID">channel id param.</param>
        /// <param name="messageID">message id param.</param>
        /// <returns>chat message.</returns>
        public async Task<ChatMessage> GetMessagesAsync(string teamsID, string channelID, string messageID)
        {
            var response = new ChatMessage();
            try
            {
                response = await this.graphServiceClient
                                                    .Teams[teamsID]
                                                    .Channels[channelID]
                                                    .Messages[messageID]
                                                    .Request()
                                                    .WithMaxRetry(GraphConstants.MaxRetry)
                                                    .GetAsync();
            }
            catch (Exception)
            {
            }

            return response;
        }

        /// <summary>
        /// Get the Loggedinuser Details.
        /// </summary>
        /// <returns>chat message.</returns>
        public async Task<User> GetLoggedinUserDetails()
        {
            var response = new User();
            try
            {
                response = await this.graphServiceClient
                                                 .Me
                                                 .Request()
                                                 .GetAsync();
            }
            catch (Exception)
            {
            }
            return response;
        }
    }
}
