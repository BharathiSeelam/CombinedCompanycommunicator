// <copyright file="DistributionListDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;

    /// <summary>
    /// Repository of the DistributionList data in the table storage.
    /// </summary>
    public class DistributionListDataRepository : BaseRepository<DistributionListDataEntity>, IDistributionListDataRepository
    {
        private readonly IDistributionListDataRepository distributionListDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionListDataRepository"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">The notification service.</param>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        public DistributionListDataRepository(
        INotificationDataRepository notificationDataRepository,
        ILogger<DistributionListDataRepository> logger,
        IOptions<RepositoryOptions> repositoryOptions,
        TableRowKeyGenerator tableRowKeyGenerator)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: DistributionListDataTableNames.TableName,
                  defaultPartitionKey: DistributionListDataTableNames.DistributionListPartition,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
            this.TableRowKeyGenerator = tableRowKeyGenerator;
        }

        /// <inheritdoc/>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <inheritdoc/>
        public async Task<IEnumerable<DistributionListDataEntity>> GetAllDistributionListsAsync()
        {
            var result = await this.GetAllAsync(DistributionListDataTableNames.DistributionListPartition);

            return result;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetDLsByIdsAsync(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new List<string>();
            }

            var rowKeysFilter = this.GetRowKeysFilter(ids);
            var teamDataEntities = await this.GetWithFilterAsync(rowKeysFilter);

            return teamDataEntities.Select(p => p.DLName).OrderBy(p => p);
        }
    }
}
