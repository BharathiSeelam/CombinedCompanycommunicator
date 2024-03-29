﻿// <copyright file="SentUpdateandDeleteDataRepository.cs" company="Microsoft">
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
    public class SentUpdateandDeleteDataRepository : BaseRepository<SentNotificationDataEntity>, ISentUpdateandDeleteNotificationDataRepository
    {
        private readonly IMessageService messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentUpdateandDeleteDataRepository"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">The notification service.</param>
        /// <param name="logger">The logging service.</param>
        /// <param name="messageService">The messaging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public SentUpdateandDeleteDataRepository(
             INotificationDataRepository notificationDataRepository,
             ILogger<SentNotificationDataRepository> logger,
             IMessageService messageService,
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
        public async Task DeleteFromPostAsync(string notificationId)
        {
            var sentNotificationDataEntites = await this.GetWithFilterAsync("DeliveryStatus eq 'Succeeded'", notificationId);
            if (sentNotificationDataEntites != null)
            {
                foreach (var sentNotificationDataEntity in sentNotificationDataEntites)
                {
                    // var indent = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond, d.Kind);
                    // var messageid = Convert.ToInt32(sentNotificationDataEntity.SentDate);
                    await this.messageService.DeleteSentNotification(
                           notificationId: sentNotificationDataEntity.ConversationId,
                           recipientId: sentNotificationDataEntity.RecipientId,
                           serviceUrl: sentNotificationDataEntity.ServiceUrl,
                           tenantId: sentNotificationDataEntity.TenantId,
                           activityId: sentNotificationDataEntity.ActivtyId);

                    // await this.sentNotificationDataRepository.DeleteAsync(notification);
                }
            }
        }
    }
}
