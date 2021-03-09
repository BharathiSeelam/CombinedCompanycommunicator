// <copyright file="DraftNotificationPreviewService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.DraftNotificationPreview
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Bot;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.AdaptiveCard;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;
    using Newtonsoft.Json;

    /// <summary>
    /// Draft notification preview service.
    /// </summary>
    public class DraftNotificationPreviewService
    {
        private static readonly string MsTeamsChannelId = "msteams";
        private static readonly string ChannelConversationType = "channel";
        private static readonly string ThrottledErrorResponse = "Throttled";

        private readonly string botAppId;
        private readonly AdaptiveCardCreator adaptiveCardCreator;
        private readonly CompanyCommunicatorBotAdapter companyCommunicatorBotAdapter;
        private readonly ITemplateDataRepository templateDataRepository;
        private readonly string sendFunctionAppBaseURL;

        /// <summary>
        /// Initializes a new instance of the <see cref="DraftNotificationPreviewService"/> class.
        /// </summary>
        /// <param name="botOptions">The bot options.</param>
        /// <param name="adaptiveCardCreator">Adaptive card creator service.</param>
        /// <param name="companyCommunicatorBotAdapter">Bot framework http adapter instance.</param>
        /// <param name="templateDataRepository">The template data repository.</param>
        public DraftNotificationPreviewService(
            IOptions<BotOptions> botOptions,
            AdaptiveCardCreator adaptiveCardCreator,
            CompanyCommunicatorBotAdapter companyCommunicatorBotAdapter,
            ITemplateDataRepository templateDataRepository)
        {
            var options = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.sendFunctionAppBaseURL = options.Value.SendFunctionAppBaseURL;
            this.botAppId = options.Value.AuthorAppId;
            if (string.IsNullOrEmpty(this.botAppId))
            {
                throw new ApplicationException("AuthorAppId setting is missing in the configuration.");
            }

            this.adaptiveCardCreator = adaptiveCardCreator ?? throw new ArgumentNullException(nameof(adaptiveCardCreator));
            this.templateDataRepository = templateDataRepository ?? throw new ArgumentNullException(nameof(templateDataRepository));
            this.companyCommunicatorBotAdapter = companyCommunicatorBotAdapter ?? throw new ArgumentNullException(nameof(companyCommunicatorBotAdapter));
        }

        /// <summary>
        /// Send a preview of a draft notification.
        /// </summary>
        /// <param name="draftNotificationEntity">Draft notification entity.</param>
        /// <param name="teamDataEntity">The team data entity.</param>
        /// <param name="teamsChannelId">The Teams channel id.</param>
        /// <returns>It returns HttpStatusCode.OK, if this method triggers the bot service to send the adaptive card successfully.
        /// It returns HttpStatusCode.TooManyRequests, if the bot service throttled the request to send the adaptive card.</returns>
        public async Task<HttpStatusCode> SendPreview(NotificationDataEntity draftNotificationEntity, TeamDataEntity teamDataEntity, string teamsChannelId)
        {
            if (draftNotificationEntity == null)
            {
                throw new ArgumentException("Null draft notification entity.");
            }

            if (teamDataEntity == null)
            {
                throw new ArgumentException("Null team data entity.");
            }

            if (string.IsNullOrWhiteSpace(teamsChannelId))
            {
                throw new ArgumentException("Null channel id.");
            }

            // Create bot conversation reference.
            var conversationReference = this.PrepareConversationReferenceAsync(teamDataEntity, teamsChannelId);

            // Ensure the bot service URL is trusted.
            if (!MicrosoftAppCredentials.IsTrustedServiceUrl(conversationReference.ServiceUrl))
            {
                MicrosoftAppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
            }

            // Trigger bot to send the adaptive card.
            try
            {
                //await this.companyCommunicatorBotAdapter.ContinueConversationAsync(
                //    this.botAppId,
                //    conversationReference,
                //    async (turnContext, cancellationToken) => await this.SendAdaptiveCardAsync(turnContext, draftNotificationEntity),
                //    CancellationToken.None);

                var previewDataEntity = new PreviewDataEntity
                {
                    ConversationReferance = conversationReference,
                    MessageActivity = await this.GetPreviewMessageActivity(draftNotificationEntity),
                    ServiceUrl = conversationReference.ServiceUrl,
                    AppID = this.botAppId,
                };

                var json = JsonConvert.SerializeObject(previewDataEntity);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = this.sendFunctionAppBaseURL + "SendPreviewFunction";

                using var client = new HttpClient();
                {
                    var response = await client.PostAsync(url, data);
                    string result = response.Content.ReadAsStringAsync().Result;
                }

                return HttpStatusCode.OK;
            }
            catch (ErrorResponseException e)
            {
                var errorResponse = (ErrorResponse)e.Body;
                if (errorResponse != null
                    && errorResponse.Error.Code.Equals(DraftNotificationPreviewService.ThrottledErrorResponse, StringComparison.OrdinalIgnoreCase))
                {
                    return HttpStatusCode.TooManyRequests;
                }

                throw;
            }
        }

        /// <summary>
        /// Get message activity.
        /// </summary>
        /// <param name="draftNotificationEntity">draftNotificationEntity param.</param>
        /// <returns>Message activity.</returns>
        private async Task<Activity> GetPreviewMessageActivity(
           NotificationDataEntity draftNotificationEntity)
        {
            var templateDataEntityResult = await this.templateDataRepository.GetAsync("Default", draftNotificationEntity.TemplateID);
            var reply = this.CreateReply(draftNotificationEntity, templateDataEntityResult.TemplateJSON);
            var attachments = reply.Attachments[0];
            var sendCardActivity = new Activity(ActivityTypes.Message)
            {                
                Attachments = new List<Attachment>
                            {
                              new Attachment()
                                        {
                                            ContentType = attachments.ContentType,
                                            Content = JsonConvert.DeserializeObject((string)attachments.Content),
                                        },
                            },
            };
            return sendCardActivity;
        }

        private ConversationReference PrepareConversationReferenceAsync(TeamDataEntity teamDataEntity, string channelId)
        {
            var channelAccount = new ChannelAccount
            {
                Id = $"28:{this.botAppId}",
            };

            var conversationAccount = new ConversationAccount
            {
                ConversationType = DraftNotificationPreviewService.ChannelConversationType,
                Id = channelId,
                TenantId = teamDataEntity.TenantId,
            };

            var conversationReference = new ConversationReference
            {
                Bot = channelAccount,
                ChannelId = DraftNotificationPreviewService.MsTeamsChannelId,
                Conversation = conversationAccount,
                ServiceUrl = teamDataEntity.ServiceUrl,
            };

            return conversationReference;
        }

        private async Task SendAdaptiveCardAsync(
            ITurnContext turnContext,
            NotificationDataEntity draftNotificationEntity)
        {
            var templateDataEntityResult = await this.templateDataRepository.GetAsync("Default", draftNotificationEntity.TemplateID);
            var reply = this.CreateReply(draftNotificationEntity, templateDataEntityResult.TemplateJSON);
            var attachments = reply.Attachments[0];
            var sendCardActivity = new Activity(ActivityTypes.Message)
            {
                Conversation = turnContext.Activity.Conversation,
                Attachments = new List<Attachment>
                            {
                              new Attachment()
                                        {
                                            ContentType = attachments.ContentType,
                                            Content = JsonConvert.DeserializeObject((string)attachments.Content),
                                        },
                            },
            };
            await turnContext.SendActivityAsync(sendCardActivity);
        }

        private IMessageActivity CreateReply(NotificationDataEntity draftNotificationEntity, string templateJson)
        {
            var adaptiveCard = this.adaptiveCardCreator.CreateAdaptiveCardWithoutHeader(
                draftNotificationEntity.Title,
                draftNotificationEntity.ImageLink,
                draftNotificationEntity.Summary,
                draftNotificationEntity.Author,
                draftNotificationEntity.ButtonTitle,
                draftNotificationEntity.ButtonLink,
                templateJson);

            var attachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard,
            };

            var reply = MessageFactory.Attachment(attachment);

            return reply;
        }
    }
}