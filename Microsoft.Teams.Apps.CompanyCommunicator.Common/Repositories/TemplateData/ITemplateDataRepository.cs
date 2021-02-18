// <copyright file="ITemplateDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// interface for  template data Repository.
    /// </summary>
    public interface ITemplateDataRepository : IRepository<TemplateDataEntity>
    {
        /// <summary>
        /// Gets table row key generator.
        /// </summary>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <summary>
        /// Get all  template entities from the table storage.
        /// </summary>
        /// <returns>All  template entities.</returns>
        public Task<IEnumerable<TemplateDataEntity>> GetAllTemplatesAsync();
    }
}
