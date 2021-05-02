// <copyright file="ExportFunction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.ExportQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Orchestrator;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function App triggered by messages from a Service Bus queue.
    /// This function exports notification in a zip file for the admin.
    /// It prepares the file by reading the notification data, user graph api.
    /// This function stage the file in Blob Storage and send the
    /// file card to the admin using bot framework adapter.
    /// </summary>
    public class ExportFunction
    {
        private readonly INotificationDataRepository notificationDataRepository;
        private readonly IExportDataRepository exportDataRepository;
        private readonly IStringLocalizer<Strings> localizer;
        private readonly ITeamMembersService memberService;
        private readonly IUserDataRepository userDataRepository;
        private readonly ITeamDataRepository teamDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFunction"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">Notification data repository.</param>
        /// <param name="exportDataRepository">Export data repository.</param>
        /// <param name="localizer">Localization service.</param>
        public ExportFunction(
            INotificationDataRepository notificationDataRepository,
            IExportDataRepository exportDataRepository,
            IStringLocalizer<Strings> localizer,
            ITeamMembersService memberService,
            IUserDataRepository userDataRepository,
            ITeamDataRepository teamDataRepository
            )
        {
            this.notificationDataRepository = notificationDataRepository ?? throw new ArgumentNullException(nameof(notificationDataRepository));
            this.exportDataRepository = exportDataRepository ?? throw new ArgumentNullException(nameof(exportDataRepository));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this.userDataRepository = userDataRepository ?? throw new ArgumentNullException(nameof(userDataRepository));
            this.teamDataRepository = teamDataRepository ?? throw new ArgumentNullException(nameof(teamDataRepository));
        }

        /// <summary>
        /// Azure Function App triggered by messages from a Service Bus queue.
        /// It kicks off the durable orchestration for exporting notifications.
        /// </summary>
        /// <param name="myQueueItem">The Service Bus queue item.</param>
        /// <param name="starter">Durable orchestration client.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName("CompanyCommunicatorExportFunction")]
        public async Task Run(
            [ServiceBusTrigger(
             ExportQueue.QueueName,
             Connection = ExportQueue.ServiceBusConnectionConfigurationKey)]
            string myQueueItem,
            [DurableClient]
            IDurableOrchestrationClient starter)
        {
            if (myQueueItem == null)
            {
                throw new ArgumentNullException(nameof(myQueueItem));
            }

            if (starter == null)
            {
                throw new ArgumentNullException(nameof(starter));
            }

            var messageContent = JsonConvert.DeserializeObject<ExportMessageQueueContent>(myQueueItem);
            var notificationId = messageContent.NotificationId;

            var sentNotificationDataEntity = await this.notificationDataRepository.GetAsync(
                partitionKey: NotificationDataTableNames.SentNotificationsPartition,
                rowKey: notificationId);
            var exportDataEntity = await this.exportDataRepository.GetAsync(messageContent.UserId, notificationId);
            if (exportDataEntity.ExportType == "ExportAllNotifications")
            {
                //exportDataEntity.FileName = this.GetFileName("FileName_ExportDetails");
                var fileName = this.localizer.GetString("FileName_ExportDetails") + "_" + exportDataEntity.RowKey  + ".zip";
                exportDataEntity.FileName = fileName;

                var userId = exportDataEntity.PartitionKey;
                var requestedTeamId = exportDataEntity.RequestedTeamId;
                var user = await this.userDataRepository.GetAsync(UserDataTableNames.AuthorDataPartition, userId);
                if (user == null)
                {
                    await this.SyncAuthorAsync(requestedTeamId, userId);
                }
            }
            else
            {
                exportDataEntity.FileName = this.GetFileName("FileName_ExportData");
            }

            var requirement = new ExportDataRequirement(sentNotificationDataEntity, exportDataEntity, messageContent.UserId);

            if (exportDataEntity.ExportType == "ExportAllNotifications")
            {
             string instanceId = await starter.StartNewAsync( nameof(ExportOrchestration.ExportOrchestrationAsync), requirement);
            }
            else if (requirement.IsValid())
            {
             string instanceId = await starter.StartNewAsync(nameof(ExportOrchestration.ExportOrchestrationAsync), requirement);
            }
        }

        private string GetFileName(string resourceKey)
        {
            var guid = Guid.NewGuid().ToString();
            var fileName = this.localizer.GetString(resourceKey); // "FileName_ExportData"
            return $"{fileName}_{guid}.zip";
        }

        private async Task SyncAuthorAsync(string teamId, string userId)
        {           
            var teamData = await this.teamDataRepository.GetAsync("TeamData", teamId);
            var tenantId = teamData.TenantId;
            var serviceUrl = teamData.ServiceUrl;
            // Sync members.
            var userEntities = await this.memberService.GetAuthorsAsync(
                teamId: teamId,
                tenantId: tenantId,
                serviceUrl: serviceUrl);

            var userData = userEntities.FirstOrDefault(user => user.AadId.Equals(userId));
            if (userData == null)
            {
                throw new ApplicationException("Unable to find user in Team roster");
            }

            userData.PartitionKey = UserDataTableNames.AuthorDataPartition;
            userData.RowKey = userData.AadId;
            await this.userDataRepository.CreateOrUpdateAsync(userData);
        }
    }
}
