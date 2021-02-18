namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;


    public interface ITeamsChannelInfo
    {
        public Task<string> GetTeamsChannelInfoAsync(string teamId, string tenantId, string serviceUrl,string teamsName);
    }
}
