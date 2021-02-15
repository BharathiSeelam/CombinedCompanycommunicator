// <copyright file="ChannelDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the channel data in the table storage.
    /// </summary>
    public class ChannelDataRepository : BaseRepository<ChannelDataEntity>, IChannelDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        public ChannelDataRepository(
          ILogger<ChannelDataRepository> logger,
          IOptions<RepositoryOptions> repositoryOptions,
          TableRowKeyGenerator tableRowKeyGenerator)
          : base(
                logger,
                storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                tableName: ChannelDataTableName.TableName,
                defaultPartitionKey: ChannelDataTableName.ChannelDataPartition,
                ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
            this.TableRowKeyGenerator = tableRowKeyGenerator;
        }

        /// <inheritdoc/>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <inheritdoc/>
        public async Task<IEnumerable<ChannelDataEntity>> GetAllSortedAlphabeticallyByNameAsync()
        {
            var channelDataEntities = await this.GetAllAsync();
            var sortedSet = new SortedSet<ChannelDataEntity>(channelDataEntities, new ChannelDataEntityComparer());
            return sortedSet;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ChannelDataEntity>> GetFilterAsync(string filter, string partitionkey)
        {
            var channelDataEntities = await this.GetWithFilterAsync(filter, partitionkey);
            return channelDataEntities;
        }

        private class ChannelDataEntityComparer : IComparer<ChannelDataEntity>
        {
            public int Compare(ChannelDataEntity x, ChannelDataEntity y)
            {
                return x.ChannelName.CompareTo(y.ChannelName);
            }
        }
        }
    }
