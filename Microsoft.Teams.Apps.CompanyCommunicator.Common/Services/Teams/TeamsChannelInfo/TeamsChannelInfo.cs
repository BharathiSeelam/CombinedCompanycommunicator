namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Options;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.CommonBot;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class TeamsChannelInfo : ITeamsChannelInfo
    {
        private readonly BotFrameworkHttpAdapter botAdapter;
        private readonly string userAppId;
        private readonly string authorAppId;
        private readonly string authorSecret;
        private readonly string grantType;
        private readonly string scope;
        private readonly string tenantID;
        public TeamsChannelInfo(
            BotFrameworkHttpAdapter botAdapter,
            IOptions<BotOptions> botOptions)
        {
            this.botAdapter = botAdapter ?? throw new ArgumentNullException(nameof(botAdapter));
            this.userAppId = botOptions?.Value?.UserAppId ?? throw new ArgumentNullException(nameof(botOptions));
            this.authorAppId = botOptions?.Value?.AuthorAppId ?? throw new ArgumentNullException(nameof(botOptions));
            this.authorSecret = botOptions?.Value?.AuthorAppPassword ?? throw new ArgumentNullException(nameof(botOptions));
            this.grantType = botOptions?.Value?.GrantType ?? throw new ArgumentNullException(nameof(botOptions));
            this.scope = botOptions?.Value?.Scope ?? throw new ArgumentNullException(nameof(botOptions));
            this.tenantID = botOptions?.Value?.TenantId ?? throw new ArgumentNullException(nameof(botOptions));
        }
        public async Task<string> GetTeamsChannelInfoAsync(string teamId, string tenantId, string serviceUrl, string teamsName)
        {
            TokenGeneratorExtension tokenHelper = new TokenGeneratorExtension();
            GraphServiceClient graphClient = tokenHelper.GenerateGraphClient(this.authorAppId, this.scope, this.authorSecret, this.grantType, this.TenantId);
            graphClient.BaseUrl = graphClient.BaseUrl.Replace("v1.0", "beta");
            var teams = graphClient.Groups.Request().Filter("resourceProvisioningOptions/Any(x:x eq 'Team') and displayname eq '" + teamsName + "'").GetAsync().Result;
            var grp = new List<Group>(teams as IEnumerable<Group>);
            var g = grp.Find(s => s.DisplayName.Equals(teamsName));
            return g.Id;
            //return await GetTeamsChannelsDetailsAsync(teamId, tenantId, serviceUrl, this.authorAppId,this.authorSecret).ConfigureAwait(false);
        }

        private async Task<TeamChannelDetails> GetTeamsChannelsDetailsAsync(string teamId, string tenantId, string serviceUrl, string appId, string password)
        {
            //// Set the service URL in the trusted list to ensure the SDK includes the token in the request.

            //var teamsCredentials = new MicrosoftAppCredentials(this.authorAppId, this.authorSecret);
            //MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.MaxValue);
            //var connector = new ConnectorClient(new Uri(serviceUrl),teamsCredentials);


            //var members = await connector.Conversations.GetConversationMembersAsync(teamId).ConfigureAwait(false);


            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.MaxValue);

            var conversationReference = new ConversationReference
            {
                ServiceUrl = serviceUrl,
                Conversation = new ConversationAccount
                {
                    Id = teamId,
                },
            };

            TeamChannelDetails userDataEntitiesResult = null;

            await this.botAdapter.ContinueConversationAsync(
                appId,
                conversationReference,
                async (turnContext, cancellationToken) =>
                {

                    var members = await this.GetTeamChannelDetails(turnContext, cancellationToken, teamId, tenantId, serviceUrl).ConfigureAwait(false);
                    userDataEntitiesResult = members;
                },
                CancellationToken.None);

            return userDataEntitiesResult;
        }

        private async Task<TeamChannelDetails> GetTeamChannelDetails(ITurnContext turnContext, CancellationToken cancellationToken, string teamId, string tenantId, string serviceUrl)
        {
            TeamChannelDetails objDetails = new TeamChannelDetails();
            try
            {
                do
                {
                    TeamDetails teamDetails = await TeamsInfo.GetTeamDetailsAsync(turnContext, teamId, cancellationToken).ConfigureAwait(false);
                    objDetails.TeamDetails = teamDetails;
                    objDetails.AadGroupId = teamDetails.AadGroupId;
                    objDetails.TeamName = teamDetails.Name;
                    objDetails.TeamId = turnContext.Activity.TeamsGetTeamInfo().Id; // teamDetails.Id



                    var channelDetails = await TeamsInfo.GetTeamChannelsAsync(turnContext, teamId, cancellationToken).ConfigureAwait(false);
                    objDetails.Channels = channelDetails;
                    objDetails.ChannelId = turnContext.Activity.TeamsGetChannelId();
                    objDetails.ChannelName = channelDetails.Where(channel => channel.Id == objDetails.ChannelId).FirstOrDefault().Name ?? "General";
                    //objDetails.TeamMembers = TeamsInfo.GetMembersAsync(turnContext, cancellationToken);
                }
                while (!cancellationToken.IsCancellationRequested);
                return objDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
