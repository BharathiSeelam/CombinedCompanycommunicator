namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams
{
    using Microsoft.Bot.Schema.Teams;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TeamChannelDetails
    {
        public TeamChannelDetails()
        {

        }

        public string AadGroupId { get; set; }
        public string TeamName { get; set; }
        public string TeamId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelId { get; set; }

        public TeamDetails TeamDetails { get; set; }
        public IList<ChannelInfo> Channels { get; set; }
        public IList<TeamsChannelAccount> TeamMembers { get; set; }


    }
}
