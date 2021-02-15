// <copyright file="MessageService.cs" company="Microsoft">
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
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.AdaptiveCard;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.CommonBot;
    using Polly;
    using Polly.Contrib.WaitAndRetry;
    using Polly.Retry;

    /// <summary>
    /// Teams message service.
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly string microsoftAppId;
        private readonly BotFrameworkHttpAdapter botAdapter;
        private readonly AdaptiveCardCreator adaptiveCardCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        /// <param name="botOptions">The bot options.</param>
        /// <param name="adaptiveCardCreator">The adaptiveCardCreator.</param>
        /// <param name="botAdapter">The bot adapter.</param>
        public MessageService(
            IOptions<BotOptions> botOptions,
            AdaptiveCardCreator adaptiveCardCreator,
            BotFrameworkHttpAdapter botAdapter)
        {
            this.microsoftAppId = botOptions?.Value?.UserAppId ?? throw new ArgumentNullException(nameof(botOptions));

            // this.adaptiveCardCreator = adaptiveCardCreator ?? throw new ArgumentNullException(nameof(adaptiveCardCreator));
            this.botAdapter = botAdapter ?? throw new ArgumentNullException(nameof(botAdapter));
        }

        /// <inheritdoc/>
        public async Task<SendMessageResponse> SendMessageAsync(
            IMessageActivity message,
            string conversationId,
            string serviceUrl,
            int maxAttempts,
            ILogger log)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(conversationId))
            {
                throw new ArgumentException($"'{nameof(conversationId)}' cannot be null or empty", nameof(conversationId));
            }

            if (string.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentException($"'{nameof(serviceUrl)}' cannot be null or empty", nameof(serviceUrl));
            }

            if (log is null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            // Set the service URL in the trusted list to ensure the SDK includes the token in the request.
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);

            var conversationReference = new ConversationReference
            {
                ServiceUrl = serviceUrl,
                Conversation = new ConversationAccount
                {
                    Id = conversationId,
                },
            };

            var response = new SendMessageResponse
            {
                TotalNumberOfSendThrottles = 0,
                AllSendStatusCodes = string.Empty,
            };

            await this.botAdapter.ContinueConversationAsync(
                botAppId: this.microsoftAppId,
                reference: conversationReference,
                callback: async (turnContext, cancellationToken) =>
                {
                    var policy = this.GetRetryPolicy(maxAttempts, log);
                    try
                    {
                        // Send message.
                        var messageResponse = await policy.ExecuteAsync(async () => await turnContext.SendActivityAsync(message));

                        // Success.
                        response.ResultType = SendMessageResult.Succeeded;
                        response.StatusCode = (int)HttpStatusCode.Created;
                        response.AllSendStatusCodes += $"{(int)HttpStatusCode.Created},";
                        response.ActivityId = messageResponse.Id;
                    }
                    catch (ErrorResponseException e)
                    {
                        var errorMessage = $"{e.GetType()}: {e.Message}";
                        log.LogError(e, $"Failed to send message. Exception message: {errorMessage}");

                        response.StatusCode = (int)e.Response.StatusCode;
                        response.AllSendStatusCodes += $"{(int)e.Response.StatusCode},";
                        response.ErrorMessage = e.Response.Content;
                        switch (e.Response.StatusCode)
                        {
                            case HttpStatusCode.TooManyRequests:
                                response.ResultType = SendMessageResult.Throttled;
                                response.TotalNumberOfSendThrottles = maxAttempts;
                                break;

                            case HttpStatusCode.NotFound:
                                response.ResultType = SendMessageResult.RecipientNotFound;
                                break;

                            default:
                                response.ResultType = SendMessageResult.Failed;
                                break;
                        }
                    }
                },
                cancellationToken: CancellationToken.None);

            return response;
        }

        /// <inheritdoc/>
        public async Task DeleteSentNotification(
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
                      // Delete message.
                      await turnContext.DeleteActivityAsync(conversationReference);
                  }
                  catch (ErrorResponseException e)
                  {
                      var errorMessage = $"{e.GetType()}: {e.Message}";
                  }
              },
              cancellationToken: CancellationToken.None);

            // return response;
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
                      var reply = this.CreateReply(notificationDataEntity);
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

        private IMessageActivity CreateReply(NotificationDataEntity notificationDataEntity)
        {
            var adaptiveCard = this.adaptiveCardCreator.CreateAdaptiveCard(
                notificationDataEntity.Title,
                notificationDataEntity.ImageLink,
                notificationDataEntity.Summary,
                notificationDataEntity.Author,
                notificationDataEntity.ButtonTitle,
                notificationDataEntity.ButtonLink);
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

        private AsyncRetryPolicy GetRetryPolicy(int maxAttempts, ILogger log)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: maxAttempts);
            return Policy
                .Handle<ErrorResponseException>(e =>
                {
                    var errorMessage = $"{e.GetType()}: {e.Message}";
                    log.LogError(e, $"Exception thrown: {errorMessage}");

                    // Handle throttling and internal server errors.
                    var statusCode = e.Response.StatusCode;
                    return statusCode == HttpStatusCode.TooManyRequests || ((int)statusCode >= 500 && (int)statusCode < 600);
                })
                .WaitAndRetryAsync(delay);
        }
    }
}
