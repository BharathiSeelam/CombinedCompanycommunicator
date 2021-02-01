// <copyright file="DLUserDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DLUserData
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the DLUser data in the table storage.
    /// </summary>
    public class DLUserDataRepository : BaseRepository<DLUserDataEntity>, IDLUserDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DLUserDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        public DLUserDataRepository(
        ILogger<DLUserDataRepository> logger,
        IOptions<RepositoryOptions> repositoryOptions,
        TableRowKeyGenerator tableRowKeyGenerator)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: DLUserDataTableNames.TableName,
                  defaultPartitionKey: DLUserDataTableNames.DLUserPartition,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
            this.TableRowKeyGenerator = tableRowKeyGenerator;
        }

        /// <inheritdoc/>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <inheritdoc/>
        public async Task<IEnumerable<DLUserDataEntity>> GetAllDLUsersAsync()
        {
            var result = await this.GetAllAsync(DLUserDataTableNames.DLUserPartition);

            return result;
        }
    }
}
