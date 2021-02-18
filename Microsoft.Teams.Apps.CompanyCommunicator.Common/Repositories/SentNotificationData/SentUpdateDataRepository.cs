// <copyright file="SentUpdateDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;

    /// <summary>
    /// Repository of the notification data in the table storage.
    /// </summary>
    public class SentUpdateDataRepository : BaseRepository<SentNotificationDataEntity>, ISentUpdateDataRepository
    {
        private readonly IUpdateMessageService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentUpdateDataRepository"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">The notification service.</param>
        /// <param name="logger">The logging service.</param>
        /// <param name="messageService">The messaging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public SentUpdateDataRepository(
             INotificationDataRepository notificationDataRepository,
             ILogger<SentNotificationDataRepository> logger,
             IUpdateMessageService messageService,
             IOptions<RepositoryOptions> repositoryOptions)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: SentNotificationDataTableNames.TableName,
                  defaultPartitionKey: SentNotificationDataTableNames.DefaultPartition,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        }

        /// <inheritdoc/>
        public async Task EnsureSentNotificationDataTableExistsAsync()
        {
            var exists = await this.Table.ExistsAsync();
            if (!exists)
            {
                await this.Table.CreateAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateFromPostAsync(string notificationId, NotificationDataEntity isentNotificationDataEntity)
        {
            var sentNotificationDataEntites = await this.GetWithFilterAsync("PartitionKey eq '" + notificationId + "' ", notificationId);

            if (sentNotificationDataEntites != null)
            {
                // var notificationDataEntites = await this.GetWithFilterAsync("PartitionKey eq '" + notificationId + "' ", notificationId);
                foreach (var sentNotificationDataEntity in sentNotificationDataEntites)
                {
                    await this.messageService.UpdatePostSentNotification(
                    notificationDataEntity: isentNotificationDataEntity,
                    notificationId: sentNotificationDataEntity.ConversationId,
                    recipientId: sentNotificationDataEntity.RecipientId,
                    serviceUrl: sentNotificationDataEntity.ServiceUrl,
                    tenantId: sentNotificationDataEntity.TenantId,
                    activityId: sentNotificationDataEntity.ActivtyId);
                }
            }
        }
    }
}
