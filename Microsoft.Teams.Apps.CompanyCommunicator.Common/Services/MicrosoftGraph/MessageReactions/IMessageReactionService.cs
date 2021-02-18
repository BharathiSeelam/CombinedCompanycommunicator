namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    public interface IMessageReactionService
    {
       public Task<ChatMessage> GetMessagesAsync(string teamsID, string channelID, string messageID);
    }
}
