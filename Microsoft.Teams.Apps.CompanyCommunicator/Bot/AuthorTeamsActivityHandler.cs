// <copyright file="AuthorTeamsActivityHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.AdaptiveCard;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.User;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Company Communicator Author Bot.
    /// Captures author data, file upload.
    /// </summary>
    public class AuthorTeamsActivityHandler : TeamsActivityHandler
    {
        private const string ChannelType = "channel";
        private readonly TeamsFileUpload teamsFileUpload;
        private readonly IUserDataService userDataService;
        private readonly IAppSettingsService appSettingsService;
        private readonly IStringLocalizer<Strings> localizer;
        private readonly INotificationDataRepository notificationDataRepository;
        private readonly IChannelDataRepository channelDataRepository;
        private readonly AdaptiveCardCreator adaptiveCardCreator;
        private readonly ITemplateDataRepository templateDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorTeamsActivityHandler"/> class.
        /// </summary>
        /// <param name="teamsFileUpload">File upload service.</param>
        /// <param name="userDataService">User data service.</param>
        /// <param name="appSettingsService">App Settings service.</param>
        /// <param name="notificationDataRepository">Notification data repository service that deals with the table storage in azure.</param>
        /// <param name="channelDataRepository">ChannelDataRepository.</param>
        /// <param name="adaptiveCardCreator">adaptiveCardCreator .</param>
        /// <param name="templateDataRepository">templateD ataRepository.</param>
        /// <param name="localizer">Localization service.</param>
        public AuthorTeamsActivityHandler(
            TeamsFileUpload teamsFileUpload,
            IUserDataService userDataService,
            IAppSettingsService appSettingsService,
            INotificationDataRepository notificationDataRepository,
            IChannelDataRepository channelDataRepository,
            AdaptiveCardCreator adaptiveCardCreator,
            ITemplateDataRepository templateDataRepository,
            IStringLocalizer<Strings> localizer)
        {
            this.userDataService = userDataService ?? throw new ArgumentNullException(nameof(userDataService));
            this.teamsFileUpload = teamsFileUpload ?? throw new ArgumentNullException(nameof(teamsFileUpload));
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
            this.notificationDataRepository = notificationDataRepository ?? throw new ArgumentNullException(nameof(notificationDataRepository));
            this.adaptiveCardCreator = adaptiveCardCreator ?? throw new ArgumentNullException(nameof(adaptiveCardCreator));
            this.templateDataRepository = templateDataRepository ?? throw new ArgumentNullException(nameof(templateDataRepository));
            this.channelDataRepository = channelDataRepository ?? throw new ArgumentNullException(nameof(channelDataRepository));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        /// <summary>
        /// Invoked when a conversation update activity is received from the channel.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected override async Task OnConversationUpdateActivityAsync(
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            // base.OnConversationUpdateActivityAsync is useful when it comes to responding to users being added to or removed from the conversation.
            // For example, a bot could respond to a user being added by greeting the user.
            // By default, base.OnConversationUpdateActivityAsync will call <see cref="OnMembersAddedAsync(IList{ChannelAccount}, ITurnContext{IConversationUpdateActivity}, CancellationToken)"/>
            // if any users have been added or <see cref="OnMembersRemovedAsync(IList{ChannelAccount}, ITurnContext{IConversationUpdateActivity}, CancellationToken)"/>
            // if any users have been removed. base.OnConversationUpdateActivityAsync checks the member ID so that it only responds to updates regarding members other than the bot itself.
            await base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);

            var activity = turnContext.Activity;

            // Take action if the event includes the bot being added.
            var membersAdded = activity.MembersAdded;
            if (membersAdded != null && membersAdded.Any(p => p.Id == activity.Recipient.Id))
            {
                if (activity.Conversation.ConversationType.Equals(ChannelType))
                {
                    await this.userDataService.SaveAuthorDataAsync(activity);
                }
            }

            if (activity.MembersRemoved != null)
            {
                await this.userDataService.RemoveAuthorDataAsync(activity);
            }

            // Update service url app setting.
            await this.UpdateServiceUrl(activity.ServiceUrl);
        }

        /// <summary>
        /// Invoke when a file upload accept consent activitiy is received from the channel.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="fileConsentCardResponse">The accepted response object of File Card.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task reprsenting asynchronous operation.</returns>
        protected override async Task OnTeamsFileConsentAcceptAsync(
            ITurnContext<IInvokeActivity> turnContext,
            FileConsentCardResponse fileConsentCardResponse,
            CancellationToken cancellationToken)
        {
            var (fileName, notificationId) = this.teamsFileUpload.ExtractInformation(fileConsentCardResponse.Context);
            try
            {
                await this.teamsFileUpload.UploadToOneDrive(
                    fileName,
                    fileConsentCardResponse.UploadInfo.UploadUrl,
                    cancellationToken);

                await this.teamsFileUpload.FileUploadCompletedAsync(
                    turnContext,
                    fileConsentCardResponse,
                    fileName,
                    notificationId,
                    cancellationToken);
            }
            catch (Exception e)
            {
                await this.teamsFileUpload.FileUploadFailedAsync(
                    turnContext,
                    notificationId,
                    e.ToString(),
                    cancellationToken);
            }
        }

        /// <summary>
        /// Invoke when a file upload decline consent activitiy is received from the channel.
        /// </summary>
        /// <param name="turnContext">The context object for this turn.</param>
        /// <param name="fileConsentCardResponse">The declined response object of File Card.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task reprsenting asynchronous operation.</returns>
        protected override async Task OnTeamsFileConsentDeclineAsync(ITurnContext<IInvokeActivity> turnContext, FileConsentCardResponse fileConsentCardResponse, CancellationToken cancellationToken)
        {
            var (fileName, notificationId) = this.teamsFileUpload.ExtractInformation(
                fileConsentCardResponse.Context);

            await this.teamsFileUpload.CleanUp(
                turnContext,
                fileName,
                notificationId,
                cancellationToken);

            var reply = MessageFactory.Text(this.localizer.GetString("PermissionDeclinedText"));
            reply.TextFormat = "xml";
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        /// <summary>
        /// Invoked when the user opens the messaging extension or searching any content in it.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="query">Contains messaging extension query keywords.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Messaging extension response object to fill compose extension section.</returns>
        protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
            var text = query?.Parameters?[0]?.Value as string ?? string.Empty;
            var cattachments = new List<MessagingExtensionAttachment>();
            if (text == null || text == string.Empty || text == "true")
            {
                var obj = await this.notificationDataRepository.GetMostRecentSentNotificationsAsync();
                var packages = obj;
                var templateDataEntityResult = await this.templateDataRepository.GetAllTemplatesAsync();

                foreach (var package in packages)
                {
                    var channelEntities = await this.channelDataRepository.GetWithFilterAsync("Id eq '" + package.Channel + "'", null);
                    foreach (var cname in channelEntities)
                    {
                        var channelName = cname.ChannelName;
                        var previewCard = new ThumbnailCard { Title = package.Title, Subtitle = package.Author, Text = channelName, Tap = new CardAction { Type = "invoke", Value = package } };
                        if (!string.IsNullOrEmpty(package.ImageLink))
                        {
                            previewCard.Images = new List<CardImage>() { new CardImage(package.ImageLink, "Icon") };
                        }

                        var attachment = new MessagingExtensionAttachment
                        {
                            ContentType = HeroCard.ContentType,
                            Content = new HeroCard { Title = package.Title },
                            Preview = previewCard.ToAttachment(),
                        };

                        cattachments.Add(attachment);
                    }
                }
            }
            else
            {
                var obj = await this.notificationDataRepository.GetWithFilterAsync("Title eq '" + text + "'", "SentNotifications");
                var packages = obj;
                var templateDataEntityResult = await this.templateDataRepository.GetAllTemplatesAsync();
                foreach (var package in packages)
                {
                    var channelEntities = await this.channelDataRepository.GetWithFilterAsync("Id eq '" + package.Channel + "'", null);
                    foreach (var cname in channelEntities)
                    {
                        var channelName = cname.ChannelName;
                        var previewCard = new ThumbnailCard { Title = package.Title, Subtitle = package.Author, Text = channelName, Tap = new CardAction { Type = "invoke", Value = package } };
                        if (!string.IsNullOrEmpty(package.ImageLink))
                        {
                            previewCard.Images = new List<CardImage>() { new CardImage(package.ImageLink, "Icon") };
                        }

                        var attachment = new MessagingExtensionAttachment
                        {
                            ContentType = HeroCard.ContentType,
                            Content = new HeroCard { Title = package.Title },
                            Preview = previewCard.ToAttachment(),
                        };

                        cattachments.Add(attachment);
                    }
                }
            }

            // The list of MessagingExtensionAttachments must we wrapped in a MessagingExtensionResult wrapped in a MessagingExtensionResponse.
            return new MessagingExtensionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = cattachments,
                },
            };
        }

        /// <summary>
        /// Invoked when the user opens the messaging extension or select any content in it.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="query">Contains messaging extension query keywords.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Messaging extension response object to fill compose extension section.</returns>
        protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionSelectItemAsync(ITurnContext<IInvokeActivity> turnContext, JObject query, CancellationToken cancellationToken)
        {
            // The Preview card's Tap should have a Value property assigned, this will be returned to the bot in this event update.
            var title = (string)query["Title"];
            var imageUrl = (string)query["ImageLink"];
            var summary = (string)query["Summary"];
            var author = (string)query["Author"];
            var buttonTitle = (string)query["ButtonTitle"];
            var buttonUrl = (string)query["ButtonLink"];
            var templateId = (string)query["TemplateID"];

            // We take every row of the results and wrap them in cards wrapped in in MessagingExtensionAttachment objects.
            // The Preview is optional, if it includes a Tap, that will trigger the OnTeamsMessagingExtensionSelectItemAsync event back on this bot.
            var templateDataEntityResult = await this.templateDataRepository.GetAsync("Default", templateId);
            NotificationDataEntity notificationDataEntity = new NotificationDataEntity();
            notificationDataEntity.Title = title;
            notificationDataEntity.ImageLink = imageUrl;
            notificationDataEntity.Summary = summary;
            notificationDataEntity.Author = author;
            notificationDataEntity.ButtonTitle = buttonTitle;
            notificationDataEntity.ButtonLink = buttonUrl;

            var reply = this.CreateReply(notificationDataEntity, templateDataEntityResult.TemplateJSON);
            var attachments = reply.Attachments[0];

            var previewCard = new ThumbnailCard { Title = $"{title}, {author}" };
            if (!string.IsNullOrEmpty(imageUrl))
            {
                previewCard.Images = new List<CardImage>() { new CardImage(imageUrl, "Icon") };
            }

            var attachment = new MessagingExtensionAttachment
            {
                ContentType = attachments.ContentType,
                Content = JsonConvert.DeserializeObject((string)attachments.Content),
                Preview = previewCard.ToAttachment(),
            };

           /* var adaptiveCard = this.adaptiveCardCreator.CreateAdaptiveCard(
                title,
                imageUrl,
                summary,
                author,
                buttonTitle,
                buttonUrl
                );
            var _previewCard = new ThumbnailCard { Title = $"{title}, {author}" };
            if (!string.IsNullOrEmpty(imageUrl))
            {
                _previewCard.Images = new List<CardImage>() { new CardImage(imageUrl, "Icon") };
            }
            var attachment = new MessagingExtensionAttachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard,
                Preview = _previewCard.ToAttachment(),
            };*/
            return await Task.FromResult(new MessagingExtensionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = new List<MessagingExtensionAttachment> { attachment },
                },
            });
        }

        private async Task UpdateServiceUrl(string serviceUrl)
        {
            // Check if service url is already synced.
            var cachedUrl = await this.appSettingsService.GetServiceUrlAsync();
            if (!string.IsNullOrWhiteSpace(cachedUrl))
            {
                return;
            }

            // Update service url.
            await this.appSettingsService.SetServiceUrlAsync(serviceUrl);
        }

        private IMessageActivity CreateReply(NotificationDataEntity notificationDataEntity, string templateJson)
        {
            var adaptiveCard = this.adaptiveCardCreator.CreateAdaptiveCardWithoutHeader(
                notificationDataEntity.Title,
                notificationDataEntity.ImageLink,
                notificationDataEntity.Summary,
                notificationDataEntity.Author,
                notificationDataEntity.ButtonTitle,
                notificationDataEntity.ButtonLink,
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
