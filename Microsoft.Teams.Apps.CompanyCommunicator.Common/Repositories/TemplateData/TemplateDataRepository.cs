// <copyright file="TemplateDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Repository of the tempalte data in the table storage.
    /// </summary>
    public class TemplateDataRepository : BaseRepository<TemplateDataEntity>, ITemplateDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateDataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        /// <param name="tableRowKeyGenerator">Table row key generator service.</param>
        public TemplateDataRepository(
        ILogger<TemplateDataRepository> logger,
        IOptions<RepositoryOptions> repositoryOptions,
        TableRowKeyGenerator tableRowKeyGenerator)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: TemplateDataTableNames.TableName,
                  defaultPartitionKey: TemplateDataTableNames.TemplatePartition,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
            this.TableRowKeyGenerator = tableRowKeyGenerator;
        }

        /// <inheritdoc/>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <inheritdoc/>
        public async Task<IEnumerable<TemplateDataEntity>> GetAllTemplatesAsync()
        {
            var result = await this.GetAllAsync(TemplateDataTableNames.TemplatePartition);

            return result;
        }
    }
}
