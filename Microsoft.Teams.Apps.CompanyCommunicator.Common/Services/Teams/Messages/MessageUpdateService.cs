// <copyright file="MessageUpdateService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.AdaptiveCard;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.CommonBot;
    using Polly;
    using Polly.Contrib.WaitAndRetry;
    using Polly.Retry;

    /// <summary>
    /// Teams message service.
    /// </summary>
    public class MessageUpdateService : IUpdateMessageService
    {
        private readonly string microsoftAppId;
        private readonly BotFrameworkHttpAdapter botAdapter;
        private readonly AdaptiveCardCreator adaptiveCardCreator;
        private readonly ITemplateDataRepository templateDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageUpdateService"/> class.
        /// </summary>
        /// <param name="botOptions">The bot options.</param>
        /// <param name="adaptiveCardCreator">The adaptiveCardCreator.</param>
        /// <param name="templateDataRepository">The template data repository.</param>
        /// <param name="botAdapter">The bot adapter.</param>
        public MessageUpdateService(
            IOptions<BotOptions> botOptions,
            AdaptiveCardCreator adaptiveCardCreator,
            ITemplateDataRepository templateDataRepository,
            BotFrameworkHttpAdapter botAdapter)
        {
            this.microsoftAppId = botOptions?.Value?.UserAppId ?? throw new ArgumentNullException(nameof(botOptions));
            this.adaptiveCardCreator = adaptiveCardCreator ?? throw new ArgumentNullException(nameof(adaptiveCardCreator));
            this.templateDataRepository = templateDataRepository ?? throw new ArgumentNullException(nameof(templateDataRepository));
            this.botAdapter = botAdapter ?? throw new ArgumentNullException(nameof(botAdapter));
        }

        /// <inheritdoc/>
        public async Task UpdatePostSentNotification(
            NotificationDataEntity notificationDataEntity,
            string conversationId,
            string recipientId,
            string serviceUrl,
            string tenantId,
            string activityId)
        {
            // Set the service URL in the trusted list to ensure the SDK includes the token in the request.
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
            var conversationReference = new ConversationReference
            {
                ServiceUrl = serviceUrl,
                ActivityId = activityId,
                Conversation = new ConversationAccount
                {
                    TenantId = tenantId,
                    Id = conversationId,
                },
            };
            await this.botAdapter.ContinueConversationAsync(
              botAppId: this.microsoftAppId,
              reference: conversationReference,
              callback: async (turnContext, cancellationToken) =>
              {
                  try
                  {
                      // Update message.
                      var templateDataEntityResult = await this.templateDataRepository.GetAsync("Default", notificationDataEntity.TemplateID);
                      var reply = this.CreateReply(notificationDataEntity, templateDataEntityResult.TemplateJSON);
                     // var reply = this.CreateReply(notificationDataEntity);
                      var attachments = reply.Attachments[0];
                      var updateCardActivity = new Activity(ActivityTypes.Message)
                      {
                          Id = activityId,
                          Conversation = turnContext.Activity.Conversation,
                          Attachments = new List<Attachment> { attachments },
                      };
                      await turnContext.UpdateActivityAsync(updateCardActivity, cancellationToken);
                  }
                  catch (ErrorResponseException e)
                  {
                      var errorMessage = $"{e.GetType()}: {e.Message}";
                  }
              },
              cancellationToken: CancellationToken.None);

            // return response;
        }

        private IMessageActivity CreateReply(NotificationDataEntity notificationDataEntity , string templateJson)
        {

            var adaptiveCard = this.adaptiveCardCreator.CreateAdaptiveCardWithoutHeader(
                notificationDataEntity.Title,
                notificationDataEntity.ImageLink,
                notificationDataEntity.Summary,
                notificationDataEntity.Author,
                notificationDataEntity.ButtonTitle,
                notificationDataEntity.ButtonLink,
                templateJson);
            /*var adaptiveCard = this.adaptiveCardCreator.CreateAdaptiveCard(
                   "Testing3",
                   "https://www.cloudsavvyit.com/thumbcache/600/340/dc6262cc4d1f985b23e2bca456d9a611/p/uploads/2020/09/8b1648fb.png",
                   "Summary Test",
                   "Test",
                   "Click here",
                   "https://clickhere.com"
                   );*/

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
