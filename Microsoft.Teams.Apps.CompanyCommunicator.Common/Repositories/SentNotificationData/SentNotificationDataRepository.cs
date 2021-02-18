// <copyright file="SentNotificationDataRepository.cs" company="Microsoft">
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
    public class SentNotificationDataRepository : BaseRepository<SentNotificationDataEntity>, ISentNotificationDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentNotificationDataRepository"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">The notification service.</param>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public SentNotificationDataRepository(
             INotificationDataRepository notificationDataRepository,
             ILogger<SentNotificationDataRepository> logger,
             IOptions<RepositoryOptions> repositoryOptions)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: SentNotificationDataTableNames.TableName,
                  defaultPartitionKey: SentNotificationDataTableNames.DefaultPartition,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
                // this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
               // this.messageService = messageService;
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

        public async Task<SentNotificationDataEntity> GetActivityIDAsync(string notificationID)
        {
            SentNotificationDataEntity sentNotificationDatResult = null;
            var sentNotificationDataEntites = await this.GetAllAsync(notificationID);
            if (sentNotificationDataEntites != null)
            {
                foreach (var sentNotificationDataEntity in sentNotificationDataEntites)
                {
                    sentNotificationDatResult = sentNotificationDataEntity;
                }
            }

            return sentNotificationDatResult;
        }
    }
}
