// <copyright file="IDistributionListDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// interface for Distribution list data Repository.
    /// </summary>
    public interface IDistributionListDataRepository : IRepository<DistributionListDataEntity>
    {
        /// <summary>
        /// Gets table row key generator.
        /// </summary>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }


        /// <summary>
        /// Get DLs by Ids.
        /// </summary>
        /// <param name="ids">DL ids.</param>
        /// <returns>Names of the DLs matching incoming ids.</returns>
        public Task<IEnumerable<string>> GetDLsByIdsAsync(IEnumerable<string> ids);

        /// <summary>
        /// Get all distribution list entities from the table storage.
        /// </summary>
        /// <returns>All distribution list entities.</returns>
        public Task<IEnumerable<DistributionListDataEntity>> GetAllDistributionListsAsync();

    }
}
