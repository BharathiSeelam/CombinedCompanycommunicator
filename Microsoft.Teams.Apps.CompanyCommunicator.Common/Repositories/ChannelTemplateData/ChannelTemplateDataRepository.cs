// <copyright file="ChannelTemplateDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelTemplateData
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the channel tempalte data in the table storage.
    /// </summary>
    public class ChannelTemplateDataRepository : BaseRepository<ChannelTemplateDataEntity>, IChannelTemplateDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelTemplateDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        public ChannelTemplateDataRepository(
        ILogger<ChannelTemplateDataRepository> logger,
        IOptions<RepositoryOptions> repositoryOptions,
        TableRowKeyGenerator tableRowKeyGenerator)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: ChannelTemplateDataTableNames.TableName,
                  defaultPartitionKey: ChannelTemplateDataTableNames.ChannelTemplatePartition,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
            this.TableRowKeyGenerator = tableRowKeyGenerator;
        }

        /// <inheritdoc/>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <inheritdoc/>
        public async Task<IEnumerable<ChannelTemplateDataEntity>> GetAllChannelTemplatesAsync()
        {
            var result = await this.GetAllAsync(ChannelTemplateDataTableNames.ChannelTemplatePartition);

            return result;
        }
    }
}
