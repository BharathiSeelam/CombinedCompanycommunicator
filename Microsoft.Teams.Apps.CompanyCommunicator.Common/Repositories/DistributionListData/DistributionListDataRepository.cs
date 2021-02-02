// <copyright file="DistributionListDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the DistributionList data in the table storage.
    /// </summary>
    public class DistributionListDataRepository : BaseRepository<DistributionListDataEntity>, IDistributionListDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionListDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        public DistributionListDataRepository(
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
    }
}
