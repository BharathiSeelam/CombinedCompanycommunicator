﻿// <copyright file="DataStreamFacade.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Localization;
using Microsoft.Graph;
using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData;
using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;
using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TeamData;
using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph;
using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Extensions;
using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Streams
{
    /// <summary>
    /// Facade to get the data stream.
    /// </summary>
    public class DataStreamFacade : IDataStreamFacade
    {
        private readonly ISentNotificationDataRepository sentNotificationDataRepository;
        private readonly ITeamDataRepository teamDataRepository;
        private readonly IUsersService usersService;
        private readonly IChannelDataRepository channelDataRepository;
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStreamFacade"/> class.
        /// </summary>
        /// <param name="sentNotificationDataRepository">the sent notification data repository.</param>
        /// <param name="teamDataRepository">the team data repository.</param>
        /// <param name="usersService">the users service.</param>
        /// <param name="localizer">Localization service.</param>
        public DataStreamFacade(
            ISentNotificationDataRepository sentNotificationDataRepository,
            ITeamDataRepository teamDataRepository,
            IUsersService usersService,
            IChannelDataRepository channelDataRepository,
            IStringLocalizer<Strings> localizer)
        {
            this.sentNotificationDataRepository = sentNotificationDataRepository ?? throw new ArgumentNullException(nameof(sentNotificationDataRepository));
            this.teamDataRepository = teamDataRepository ?? throw new ArgumentNullException(nameof(teamDataRepository));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            this.channelDataRepository = channelDataRepository ?? throw new ArgumentNullException(nameof(channelDataRepository));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        /// <summary>
        /// get the users data streams.
        /// </summary>
        /// <param name="notificationId">the notification id.</param>
        /// <returns>the streams of user data.</returns>
        public async IAsyncEnumerable<IEnumerable<UserData>> GetUserDataStreamAsync(string notificationId)
        {
            if (notificationId == null)
            {
                throw new ArgumentNullException(nameof(notificationId));
            }

            var sentNotificationDataEntitiesStream = this.sentNotificationDataRepository.GetStreamsAsync(notificationId);
            await foreach (var sentNotifcations in sentNotificationDataEntitiesStream)
            {
                List<User> userList = new List<User>();
                try
                {
                    // filter the recipient not found users.
                    var users = await this.usersService.GetBatchByUserIds(
                        sentNotifcations
                        .Where(sentNotifcation => !sentNotifcation.DeliveryStatus.Equals(SentNotificationDataEntity.RecipientNotFound, StringComparison.CurrentCultureIgnoreCase))
                        .Select(notitification => notitification.RowKey)
                        .ToList()
                        .AsGroups());
                    userList = users.ToList();
                }
                catch (ServiceException serviceException)
                {
                    if (serviceException.StatusCode != HttpStatusCode.Forbidden)
                    {
                        throw serviceException;
                    }
                }

                yield return this.CreateUserData(sentNotifcations, userList);
            }
        }

        /// <summary>
        /// get the team data streams.
        /// </summary>
        /// <param name="notificationId">the notification id.</param>
        /// <returns>the streams of team data.</returns>
        public async IAsyncEnumerable<IEnumerable<TeamData>> GetTeamDataStreamAsync(string notificationId)
        {
            if (notificationId == null)
            {
                throw new ArgumentNullException(nameof(notificationId));
            }

            var sentNotificationDataEntitiesStream = this.sentNotificationDataRepository.GetStreamsAsync(notificationId);
            await foreach (var sentNotificationDataEntities in sentNotificationDataEntitiesStream)
            {
                var teamDataList = new List<TeamData>();
                foreach (var sentNotificationDataEntity in sentNotificationDataEntities)
                {
                    var team = await this.teamDataRepository.GetAsync(TeamDataTableNames.TeamDataPartition, sentNotificationDataEntity.RowKey);
                    var teamData = new TeamData
                    {
                        Id = sentNotificationDataEntity.RowKey,
                        Name = team.Name,
                        DeliveryStatus = this.localizer.GetString(sentNotificationDataEntity.DeliveryStatus),
                        StatusReason = this.GetStatusReason(sentNotificationDataEntity.ErrorMessage, sentNotificationDataEntity.StatusCode.ToString()),
                    };
                    teamDataList.Add(teamData);
                }

                yield return teamDataList;
            }
        }

        /// <summary>
        /// get the team data streams.
        /// </summary>
        /// <param name="notificationDataEntity">notificationDataEntity.</param>
        /// <returns>the streams of team data.</returns>
        public async IAsyncEnumerable<IEnumerable<NotificationDetailsExport>> GetNotificationDetailsStreamAsync (NotificationDataEntity notificationDataEntity)
        {
            if (notificationDataEntity.Id == null)
            {
                throw new ArgumentNullException(nameof(notificationDataEntity.Id));
            }

            var userId = string.Empty;
            var userName = string.Empty;
            var upn = string.Empty;
            var teamId = string.Empty;
            var teamName = string.Empty;
            var channelName = string.Empty;
            var summary = string.Empty;

            var channelDataEntity = await this.channelDataRepository.GetFilterAsync("RowKey eq '" + notificationDataEntity.Channel + "'", null);
            foreach (ChannelDataEntity channelData in channelDataEntity)
            {
                channelName = channelData.ChannelName;
            }

           
            if (!string.IsNullOrWhiteSpace(notificationDataEntity.Summary))
            {
               summary = Regex.Replace(notificationDataEntity.Summary, "<(.|\n)*?>", string.Empty);
             }

            var sentNotificationDataEntitiesStream = this.sentNotificationDataRepository.GetStreamsAsync(notificationDataEntity.Id);
            await foreach (var sentNotificationDataEntities in sentNotificationDataEntitiesStream)
            {
                var notificationDetailsList = new List<NotificationDetailsExport>();
                

                foreach (var sentNotificationDataEntity in sentNotificationDataEntities)
                {

                    if (sentNotificationDataEntity.RecipientType == "Team")
                    {
                        var team = await this.teamDataRepository.GetAsync(TeamDataTableNames.TeamDataPartition, sentNotificationDataEntity.RowKey);
                        teamId = sentNotificationDataEntity.RecipientId;
                        teamName = team.Name;
                        userId = string.Empty;
                        userName = string.Empty;
                        upn = string.Empty;
                    }

                    if (sentNotificationDataEntity.RecipientType == "User")
                    {
                        var users = await this.usersService.GetUserAsync(sentNotificationDataEntity.RecipientId);

                        userId = sentNotificationDataEntity.RecipientId;
                        userName = users.DisplayName;
                        upn = users.UserPrincipalName;
                        teamId = string.Empty;
                        teamName = string.Empty;
                    }

                    var notificationDetails = new NotificationDetailsExport
                    {
                        Id = notificationDataEntity.Id,
                        Title = notificationDataEntity.Title,
                        Summary = summary,
                        Account = channelName,
                        CreatedDateTime = notificationDataEntity.CreatedDate,
                        SentDate = notificationDataEntity.SentDate,
                        Edited = notificationDataEntity.Edited,
                        SendingStartedDate = notificationDataEntity.SendingStartedDate,
                        RecipientType = sentNotificationDataEntity.RecipientType,
                        UserId = userId,
                        Upn = upn,
                        UserName = userName,
                        TeamId = teamId,
                        TeamName = teamName,
                        DeliveryStatus = this.localizer.GetString(sentNotificationDataEntity.DeliveryStatus),
                        StatusReason = this.GetStatusReason(sentNotificationDataEntity.ErrorMessage, sentNotificationDataEntity.StatusCode.ToString()),
                    };
                    notificationDetailsList.Add(notificationDetails);
                }

                yield return notificationDetailsList;
            }
        }

        /// <summary>
        /// Create user data.
        /// </summary>
        /// <param name="sentNotificationDataEntities">the list of sent notification data entities.</param>
        /// <param name="users">the user list.</param>
        /// <returns>list of created user data.</returns>
        private IEnumerable<UserData> CreateUserData(
            IEnumerable<SentNotificationDataEntity> sentNotificationDataEntities,
            IEnumerable<User> users)
        {
            var userdatalist = new List<UserData>();
            foreach (var sentNotification in sentNotificationDataEntities)
            {
                var user = users.
                    FirstOrDefault(user => user != null && user.Id.Equals(sentNotification.RowKey));

                var userData = new UserData
                {
                    Id = sentNotification.RowKey,
                    Name = user == null ? this.localizer.GetString("AdminConsentError") : user.DisplayName,
                    Upn = user == null ? this.localizer.GetString("AdminConsentError") : user.UserPrincipalName,
                    DeliveryStatus = this.localizer.GetString(sentNotification.DeliveryStatus),
                    StatusReason = this.GetStatusReason(sentNotification.ErrorMessage, sentNotification.StatusCode.ToString()),
                };
                userdatalist.Add(userData);
            }

            return userdatalist;
        }

        /// <summary>
        /// adds the status code to error message.
        /// </summary>
        /// <param name="errorMessage">the error message.</param>
        /// <param name="statusCode">the status code.</param>
        /// <returns>status code appended error message.</returns>
        private string GetStatusReason(string errorMessage, string statusCode)
        {
            string result;
            if (string.IsNullOrEmpty(errorMessage))
            {
                result = this.localizer.GetString("OK");
            }
            else if (errorMessage.Contains("error"))
            {
                var rootMessage = JsonConvert.DeserializeObject<RootErrorMessage>(errorMessage);
                result = rootMessage.Error.Message;
            }
            else
            {
                result = errorMessage;
            }

            return $"{statusCode} : {result}";
        }
    }
}
