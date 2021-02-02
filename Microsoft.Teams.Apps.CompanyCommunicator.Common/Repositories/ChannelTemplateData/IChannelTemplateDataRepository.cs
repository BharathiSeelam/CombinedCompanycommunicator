// <copyright file="IChannelTemplateDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelTemplateData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// interface for Channel template data Repository.
    /// </summary>
    public interface IChannelTemplateDataRepository : IRepository<ChannelTemplateDataEntity>
    {
        /// <summary>
        /// Gets table row key generator.
        /// </summary>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <summary>
        /// Get all channel template entities from the table storage.
        /// </summary>
        /// <returns>All channel template entities.</returns>
        public Task<IEnumerable<ChannelTemplateDataEntity>> GetAllChannelTemplatesAsync();
    }
}
