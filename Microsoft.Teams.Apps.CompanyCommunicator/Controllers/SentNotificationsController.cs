// <copyright file="SentNotificationsController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Extensions;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.DataQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.PrepareToSendQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.SendQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;
    using Microsoft.Teams.Apps.CompanyCommunicator.Controllers.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Controller for the sent notification data.
    /// </summary>
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/sentNotifications")]
    public class SentNotificationsController : ControllerBase
    {
        private readonly IChannelDataRepository channelDataRepository;
        private readonly INotificationDataRepository notificationDataRepository;
        private readonly ISentUpdateandDeleteNotificationDataRepository sentNotificationDataRepository;
        private readonly ISentUpdateDataRepository sentNotificationUpdateDataRepository;
        private readonly ISentNotificationDataRepository sentNotificationDataRepstry;
        private readonly ITeamDataRepository teamDataRepository;
        private readonly IDistributionListDataRepository distributionListDataRepository;
        private readonly IPrepareToSendQueue prepareToSendQueue;
        private readonly ISendQueue sendQueue;
        private readonly IDataQueue dataQueue;
        private readonly double forceCompleteMessageDelayInSeconds;
        private readonly IGroupsService groupsService;
        private readonly ITeamMembersService memberService;
        private readonly IMessageReactionService reactionService;
        private readonly IExportDataRepository exportDataRepository;
        private readonly IAppCatalogService appCatalogService;
        private readonly IAppSettingsService appSettingsService;
        private readonly UserAppOptions userAppOptions;
        private readonly ILogger<SentNotificationsController> logger;
        private readonly IStringLocalizer<Strings> localizer;
        private string account;
        private string loggedinuser;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentNotificationsController"/> class.
        /// </summary>
        /// <param name="channelDataRepository">Channel data repository service that deals with the table storage in azure.</param>
        /// <param name="notificationDataRepository">Notification data repository service that deals with the table storage in azure.</param>
        /// <param name="sentNotificationDataRepository">Sent notification data repository.</param>
        /// <param name="sentNotificationUpdateDataRepository">Sent update notification data repository.</param>
        /// <param name="sentNotificationDataRepstry">Sent notification data repository to Get Likes.</param>
        /// <param name="teamDataRepository">Team data repository instance.</param>
        /// <param name="distributionListDataRepository">DistributionList data repository instance.</param>
        /// <param name="prepareToSendQueue">The service bus queue for preparing to send notifications.</param>
        /// <param name="sendQueue">The service bus queue for the send queue.</param>
        /// <param name="dataQueue">The service bus queue for the data queue.</param>
        /// <param name="dataQueueMessageOptions">The options for the data queue messages.</param>
        /// <param name="groupsService">The groups service.</param>
        /// <param name="memberService">The meber info service.</param>
        /// <param name="reactionService">The reaction of message service.</param>
        /// <param name="exportDataRepository">The Export data repository instance.</param>
        /// <param name="appCatalogService">App catalog service.</param>
        /// <param name="appSettingsService">App settings service.</param>
        /// <param name="userAppOptions">User app options.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The Configuration.</param>
        /// <param name="botOptions">bot options.</param>
        public SentNotificationsController(
            IChannelDataRepository channelDataRepository,
            INotificationDataRepository notificationDataRepository,
            ISentUpdateandDeleteNotificationDataRepository sentNotificationDataRepository,
            ISentUpdateDataRepository sentNotificationUpdateDataRepository,
            ISentNotificationDataRepository sentNotificationDataRepstry,
            ITeamDataRepository teamDataRepository,
            IDistributionListDataRepository distributionListDataRepository,
            IPrepareToSendQueue prepareToSendQueue,
            ISendQueue sendQueue,
            IDataQueue dataQueue,
            IOptions<DataQueueMessageOptions> dataQueueMessageOptions,
            IGroupsService groupsService,
            IMessageReactionService reactionService,
            ITeamMembersService memberService,
            IExportDataRepository exportDataRepository,
            IAppCatalogService appCatalogService,
            IAppSettingsService appSettingsService,
            IOptions<UserAppOptions> userAppOptions,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IOptions<BotOptions> botOptions)
        {
            if (dataQueueMessageOptions is null)
            {
                throw new ArgumentNullException(nameof(dataQueueMessageOptions));
            }

            var options = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
            this.channelDataRepository = channelDataRepository ?? throw new ArgumentNullException(nameof(channelDataRepository));
            this.notificationDataRepository = notificationDataRepository ?? throw new ArgumentNullException(nameof(notificationDataRepository));
            this.sentNotificationDataRepository = sentNotificationDataRepository ?? throw new ArgumentNullException(nameof(sentNotificationDataRepository));
            this.sentNotificationUpdateDataRepository = sentNotificationUpdateDataRepository ?? throw new ArgumentException(nameof(sentNotificationUpdateDataRepository));
            this.sentNotificationDataRepstry = sentNotificationDataRepstry ?? throw new ArgumentNullException(nameof(sentNotificationDataRepstry));
            this.teamDataRepository = teamDataRepository ?? throw new ArgumentNullException(nameof(teamDataRepository));
            this.distributionListDataRepository = distributionListDataRepository ?? throw new ArgumentNullException(nameof(distributionListDataRepository));
            this.prepareToSendQueue = prepareToSendQueue ?? throw new ArgumentNullException(nameof(prepareToSendQueue));
            this.sendQueue = sendQueue ?? throw new ArgumentNullException(nameof(sendQueue));
            this.dataQueue = dataQueue ?? throw new ArgumentNullException(nameof(dataQueue));
            this.forceCompleteMessageDelayInSeconds = dataQueueMessageOptions.Value.ForceCompleteMessageDelayInSeconds;
            this.groupsService = groupsService ?? throw new ArgumentNullException(nameof(groupsService));
            this.reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));
            this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this.exportDataRepository = exportDataRepository ?? throw new ArgumentNullException(nameof(exportDataRepository));
            this.appCatalogService = appCatalogService ?? throw new ArgumentNullException(nameof(appCatalogService));
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
            this.userAppOptions = userAppOptions?.Value ?? throw new ArgumentNullException(nameof(userAppOptions));
            this.logger = loggerFactory?.CreateLogger<SentNotificationsController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.account = string.Empty;
            this.configuration = configuration;
        }

        /// <summary>
        /// Send a notification, which turns a draft to be a sent notification.
        /// </summary>
        /// <param name="draftNotification">An instance of <see cref="DraftNotification"/> class.</param>
        /// <returns>The result of an action method.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSentNotificationAsync(
            [FromBody] DraftNotification draftNotification)
        {
            var draftNotificationDataEntity = await this.notificationDataRepository.GetAsync(
                NotificationDataTableNames.DraftNotificationsPartition,
                draftNotification.Id);
            if (draftNotificationDataEntity == null)
            {
                return this.NotFound($"Draft notification, Id: {draftNotification.Id}, could not be found.");
            }

            var newSentNotificationId =
                await this.notificationDataRepository.MoveDraftToSentPartitionAsync(draftNotificationDataEntity);

            // Ensure the data table needed by the Azure Functions to send the notifications exist in Azure storage.
            await this.sentNotificationDataRepository.EnsureSentNotificationDataTableExistsAsync();

            // Update user app id if proactive installation is enabled.
            await this.UpdateUserAppIdAsync();

            var prepareToSendQueueMessageContent = new PrepareToSendQueueMessageContent
            {
                NotificationId = newSentNotificationId,
            };
            await this.prepareToSendQueue.SendAsync(prepareToSendQueueMessageContent);

            // Send a "force complete" message to the data queue with a delay to ensure that
            // the notification will be marked as complete no matter the counts
            var forceCompleteDataQueueMessageContent = new DataQueueMessageContent
            {
                NotificationId = newSentNotificationId,
                ForceMessageComplete = true,
            };
            await this.dataQueue.SendDelayedAsync(
                forceCompleteDataQueueMessageContent,
                this.forceCompleteMessageDelayInSeconds);

            return this.Ok();
        }

        /// <summary>
        /// Update an existing sent notification.
        /// </summary>
        /// <param name="notification">An existing Draft Notification to be updated.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateSentNotificationAsync([FromBody] DraftNotification notification)
        {
            var containsHiddenMembership = await this.groupsService.ContainsHiddenMembershipAsync(notification.Groups);
            if (containsHiddenMembership)
            {
                return this.Forbid();
            }

            if (!notification.Validate(this.localizer, out string errorMessage))
            {
                return this.BadRequest(errorMessage);
            }

            var senttNotificationDataEntity = await this.notificationDataRepository.GetAsync(
                NotificationDataTableNames.SentNotificationsPartition,
                notification.Id);

            var updateSentNotificationDataEntity = new NotificationDataEntity
            {
                PartitionKey = NotificationDataTableNames.SentNotificationsPartition,
                RowKey = notification.Id,
                Id = notification.Id,
                Channel = notification.Channel,
                TemplateID = notification.TemplateID,
                Title = notification.Title,
                ImageLink = notification.ImageLink,
                Summary = notification.Summary,
                Author = notification.Author,
                ButtonTitle = notification.ButtonTitle,
                ButtonLink = notification.ButtonLink,
                CreatedBy = this.HttpContext.User?.Identity?.Name,
                CreatedDate = DateTime.UtcNow,
                SentDate = DateTime.UtcNow,
                Edited = DateTime.UtcNow,
                IsDraft = false,
                Teams = notification.Teams,
                Rosters = notification.Rosters,
                Groups = notification.Groups,
                AllUsers = notification.AllUsers,
                MessageVersion = senttNotificationDataEntity.MessageVersion,
                Succeeded = senttNotificationDataEntity.Succeeded,
                Failed = 0,
                Throttled = 0,
                TotalMessageCount = senttNotificationDataEntity.TotalMessageCount,
                SendingStartedDate = DateTime.UtcNow,
                Status = NotificationStatus.Sent.ToString(),
            };
            var id = updateSentNotificationDataEntity.Id;
            var sentNotificationId = await this.notificationDataRepository.UpdateSentNotificationAsync(updateSentNotificationDataEntity, id);
            await this.sentNotificationUpdateDataRepository.EnsureSentNotificationDataTableExistsAsync();

            var sendQueueMessageContent = new SendQueueMessageContent
            {
                NotificationId = sentNotificationId,
                ActivtiyId = null,
                RecipientData = null,
                NotificationUpdatePreviewEntity = new NotificationUpdatePreviewEntity
                {
                    ActionType = "EditNotification",
                    NotificationDataEntity = updateSentNotificationDataEntity,
                    ConversationReferance = null,
                    MessageActivity = null,
                    ServiceUrl = null,
                    AppID = null,
                },
            };
            await this.sendQueue.SendAsync(sendQueueMessageContent);
            return this.Ok();
        }

        /// <summary>
        /// Get most recently sent notification summaries.
        /// </summary>
        /// <returns>A list of <see cref="SentNotificationSummary"/> instances.</returns>
        [HttpGet]
        public async Task<IEnumerable<SentNotificationSummary>> GetSentNotificationsAsync()
        {

            var appsettingsadmin = this.configuration["AuthorizedCreatorUpns"];
            string[] adminsarr = appsettingsadmin.Split(",");
            this.loggedinuser = this.HttpContext.User?.Identity?.Name;
            var sloggedin = string.Empty + this.loggedinuser;
            this.loggedinuser = sloggedin.ToLower();
            var result = new List<SentNotificationSummary>();
            if (adminsarr.Contains(this.loggedinuser))
            {
                var notificationEntities = await this.notificationDataRepository.GetMostRecentSentNotificationsAsync();
                foreach (var notificationEntity in notificationEntities)
                {
                    string likes = await this.GetActivityIDandLikes(notificationEntity);
                    var summary = new SentNotificationSummary
                    {
                        Id = notificationEntity.Id,
                        Title = notificationEntity.Title,
                        Account = this.account,
                        CreatedDateTime = notificationEntity.CreatedDate,
                        SentDate = notificationEntity.SentDate,
                        Succeeded = notificationEntity.Succeeded,
                        Edited = notificationEntity.Edited,
                        Failed = notificationEntity.Failed,
                        Unknown = this.GetUnknownCount(notificationEntity),
                        TotalMessageCount = notificationEntity.TotalMessageCount,
                        SendingStartedDate = notificationEntity.SendingStartedDate,
                        Status = notificationEntity.GetStatus(),
                        Likes = likes,
                    };

                    result.Add(summary);
                }

                return result;
            }
            else
            {
                this.loggedinuser = sloggedin;
                var notificationEntities = await this.notificationDataRepository.GetWithFilterAsync("CreatedBy eq '" + this.loggedinuser + "'", "SentNotifications");
                foreach (var notificationEntity in notificationEntities)
                {
                    string likes = await this.GetActivityIDandLikes(notificationEntity);
                    var summary = new SentNotificationSummary
                    {
                        Id = notificationEntity.Id,
                        Title = notificationEntity.Title,
                        Account = this.account,
                        CreatedDateTime = notificationEntity.CreatedDate,
                        SentDate = notificationEntity.SentDate,
                        Succeeded = notificationEntity.Succeeded,
                        Edited = notificationEntity.Edited,
                        Failed = notificationEntity.Failed,
                        Unknown = this.GetUnknownCount(notificationEntity),
                        TotalMessageCount = notificationEntity.TotalMessageCount,
                        SendingStartedDate = notificationEntity.SendingStartedDate,
                        Status = notificationEntity.GetStatus(),
                        Likes = likes,
                    };

                    result.Add(summary);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a sent notification by Id.
        /// </summary>
        /// <param name="id">Id of the requested sent notification.</param>
        /// <returns>Required sent notification.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSentNotificationByIdAsync(string id)
        {
            int likes = 0;
            int likeCount = 0;
            var notificationEntity = await this.notificationDataRepository.GetAsync(
                NotificationDataTableNames.SentNotificationsPartition,
                id);

            if (notificationEntity == null)
            {
                return this.NotFound();
            }

            var sentNotificationEntities = await this.sentNotificationDataRepstry.GetActivityIDAsync(id);
            foreach (var sentNotificationEntity in sentNotificationEntities)
            {
                if (sentNotificationEntity != null && !string.IsNullOrEmpty(sentNotificationEntity.ConversationId))
                {
                    var teamsDataEntity = await this.teamDataRepository.GetWithFilterAsync("RowKey eq '" + sentNotificationEntity.ConversationId + "'");
                    if (teamsDataEntity != null && teamsDataEntity.ToArray().Length > 0)
                    {
                        foreach (var teamsData in teamsDataEntity)
                        {
                            string teamsID = await this.groupsService.SearchTeamsGroupAsync("displayname eq '" + teamsData.Name + "'");

                            if (!string.IsNullOrEmpty(teamsID))
                            {
                                var messageResponse = await this.reactionService.GetMessagesAsync(teamsID, sentNotificationEntity.ConversationId, sentNotificationEntity.ActivtyId);

                                if (messageResponse != null && messageResponse.Reactions != null && messageResponse.Reactions.ToArray().Length > 0)
                                {
                                    foreach (var reaction in messageResponse.Reactions)
                                    {
                                        if (reaction.ReactionType.ToString() == "like")
                                        {
                                            likeCount++;
                                        }
                                    }

                                    likes = likeCount;
                                }
                            }
                        }
                    }
                }
            }

            // var groupNames = await this.groupsService.
            // GetByIdsAsync(notificationEntity.Groups).
            //   Select(x => x.DisplayName).
            // ToListAsync();
            var userId = this.HttpContext.User.FindFirstValue(Common.Constants.ClaimTypeUserId);
            var userNotificationDownload = await this.exportDataRepository.GetAsync(userId, id);

            var result = new SentNotification
            {
                Id = notificationEntity.Id,
                Channel = notificationEntity.Channel,
                TemplateID = notificationEntity.TemplateID,
                Title = notificationEntity.Title,
                ImageLink = notificationEntity.ImageLink,
                Summary = notificationEntity.Summary,
                Author = notificationEntity.Author,
                ButtonTitle = notificationEntity.ButtonTitle,
                ButtonLink = notificationEntity.ButtonLink,
                CreatedDateTime = notificationEntity.CreatedDate,
                SentDate = notificationEntity.SentDate,
                Succeeded = notificationEntity.Succeeded,
                Failed = notificationEntity.Failed,
                Unknown = this.GetUnknownCount(notificationEntity),
                TeamNames = await this.teamDataRepository.GetTeamNamesByIdsAsync(notificationEntity.Teams),
                RosterNames = await this.teamDataRepository.GetTeamNamesByIdsAsync(notificationEntity.Rosters),
                GroupNames = await this.distributionListDataRepository.GetDLsByIdsAsync(notificationEntity.Groups),
                AllUsers = notificationEntity.AllUsers,
                SendingStartedDate = notificationEntity.SendingStartedDate,
                ErrorMessage = notificationEntity.ErrorMessage,
                WarningMessage = notificationEntity.WarningMessage,
                CanDownload = userNotificationDownload == null,
                SendingCompleted = notificationEntity.IsCompleted(),
                Likes = likes,
            };

            return this.Ok(result);
        }

        /// <summary>
        /// Get a sent notification by Id for Edit.
        /// </summary>
        /// <param name="id">Id of the requested sent notification to edit.</param>
        /// <returns>Required edit sent notification.</returns>
        [HttpGet("sent/{id}")]
        public async Task<ActionResult<DraftNotification>> GetDraftSentNotificationByIdAsync(string id)
        {
            var notificationEntity = await this.notificationDataRepository.GetAsync(
                NotificationDataTableNames.SentNotificationsPartition,
                id);
            if (notificationEntity == null)
            {
                return this.NotFound();
            }

            var result = new DraftNotification
            {
                Id = notificationEntity.Id,
                Channel = notificationEntity.Channel,
                TemplateID = notificationEntity.TemplateID,
                Title = notificationEntity.Title,
                ImageLink = notificationEntity.ImageLink,
                Summary = notificationEntity.Summary,
                Author = notificationEntity.Author,
                ButtonTitle = notificationEntity.ButtonTitle,
                ButtonLink = notificationEntity.ButtonLink,
                CreatedDateTime = notificationEntity.CreatedDate,
                Teams = notificationEntity.Teams,
                Rosters = notificationEntity.Rosters,
                Groups = notificationEntity.Groups,
                AllUsers = notificationEntity.AllUsers,
            };

            return this.Ok(result);
        }

        /// <summary>
        /// Delete an existing sent notification.
        /// </summary>
        /// <param name="id">The id of the sent notification to be deleted.</param>
        /// <returns>If the passed in Id is invalid, it returns 404 not found error. Otherwise, it returns 200 OK.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSentNotificationAsync(string id)
        {
            var notificationEntity = await this.notificationDataRepository.GetAsync(
               NotificationDataTableNames.SentNotificationsPartition,
               id);
            if (notificationEntity == null)
            {
                return this.NotFound();
            }

            await this.notificationDataRepository.DeleteAsync(notificationEntity);
            var deleteQueueMessageContent = new SendQueueMessageContent
            {
                NotificationId = id,
                ActivtiyId = null,
                RecipientData = null,
                NotificationUpdatePreviewEntity = new NotificationUpdatePreviewEntity
                {
                    ActionType = "DeleteNotification",
                    NotificationDataEntity = null,
                    ConversationReferance = null,
                    MessageActivity = null,
                    ServiceUrl = null,
                    AppID = null,
                },
            };
            await this.sendQueue.SendAsync(deleteQueueMessageContent);
            return this.Ok();
        }

        private async Task<string> GetActivityIDandLikes(NotificationDataEntity notificationEntity)
        {
            // var tenantId = this.HttpContext.User.FindFirstValue(Common.Constants.ClaimTypeTenantId);
            // var serviceUrl = await this.appSettingsService.GetServiceUrlAsync();
            int likes = 0;
            int likeCount = 0;
            try
            {
                var channelDataEntity = await this.channelDataRepository.GetFilterAsync("RowKey eq '" + notificationEntity.Channel + "'", null);
                foreach (ChannelDataEntity channelData in channelDataEntity)
                {
                    this.account = channelData.ChannelName;
                }

                var sentNotificationEntities = await this.sentNotificationDataRepstry.GetActivityIDAsync(notificationEntity.RowKey);
                foreach (var sentNotificationEntity in sentNotificationEntities)
                {

                    if (sentNotificationEntity != null && !string.IsNullOrEmpty(sentNotificationEntity.ConversationId))
                    {
                        var teamsDataEntity = await this.teamDataRepository.GetWithFilterAsync("RowKey eq '" + sentNotificationEntity.ConversationId + "'");
                        if (teamsDataEntity != null && teamsDataEntity.ToArray().Length > 0)
                        {
                            foreach (var teamsData in teamsDataEntity)
                            {
                                string teamsID = await this.groupsService.SearchTeamsGroupAsync("displayname eq '" + teamsData.Name + "'");

                                if (!string.IsNullOrEmpty(teamsID))
                                {
                                    var messageResponse = await this.reactionService.GetMessagesAsync(teamsID, sentNotificationEntity.ConversationId, sentNotificationEntity.ActivtyId);

                                    if (messageResponse != null && messageResponse.Reactions != null && messageResponse.Reactions.ToArray().Length > 0)
                                    {
                                        foreach (var reaction in messageResponse.Reactions)
                                        {
                                            if (reaction.ReactionType.ToString() == "like")
                                            {
                                                likeCount++;
                                            }
                                        }

                                        likes = likeCount;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return likes.ToString();
        }

        private int? GetUnknownCount(NotificationDataEntity notificationEntity)
        {
            var unknown = notificationEntity.Unknown;

            // In CC v2, the number of throttled recipients are counted and saved in NotificationDataEntity.Unknown property.
            // However, CC v1 saved the number of throttled recipients in NotificationDataEntity.Throttled property.
            // In order to make it backward compatible, we add the throttled number to the unknown variable.
            var throttled = notificationEntity.Throttled;
            if (throttled > 0)
            {
                unknown += throttled;
            }

            return unknown > 0 ? unknown : (int?)null;
        }

        /// <summary>
        /// Updates user app id if its not already synced.
        /// </summary>
        private async Task UpdateUserAppIdAsync()
        {
            // check if proactive installation is enabled.
            if (!this.userAppOptions.ProactivelyInstallUserApp)
            {
                return;
            }

            // check if we have already synced app id.
            var appId = await this.appSettingsService.GetUserAppIdAsync();
            if (!string.IsNullOrWhiteSpace(appId))
            {
                return;
            }

            try
            {
                // Fetch and store user app id in App Catalog.
                appId = await this.appCatalogService.GetTeamsAppIdAsync(this.userAppOptions.UserAppExternalId);

                // Graph SDK returns empty id if the app is not found.
                if (string.IsNullOrEmpty(appId))
                {
                    this.logger.LogError($"Failed to find an app in AppCatalog with external Id: {this.userAppOptions.UserAppExternalId}");
                    return;
                }

                await this.appSettingsService.SetUserAppIdAsync(appId);
            }
            catch (ServiceException exception)
            {
                // Failed to fetch app id.
                this.logger.LogError(exception, $"Failed to get catalog app id. Error message: {exception.Message}.");
            }
        }
    }
}