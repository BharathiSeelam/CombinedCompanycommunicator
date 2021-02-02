// <copyright file="IChannelDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// interface for Channel data Repository.
    /// </summary>
    public interface IChannelDataRepository : IRepository<ChannelDataEntity>
    {
        /// <summary>
        /// Gets table row key generator.
        /// </summary>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <summary>
        /// Get all channel data entities, and sort the result alphabetically by name.
        /// </summary>
        /// <returns>The channel data entities sorted alphabetically by name.</returns>
        public Task<IEnumerable<ChannelDataEntity>> GetAllSortedAlphabeticallyByNameAsync();
    }
}
