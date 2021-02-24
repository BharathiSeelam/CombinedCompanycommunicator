namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph
{
    using Microsoft.Graph;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class MessageReactionService : IMessageReactionService
    {
        private readonly IGraphServiceClient graphServiceClient;
        public MessageReactionService(IGraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient ?? throw new ArgumentNullException(nameof(graphServiceClient));
        }

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
    }
}
