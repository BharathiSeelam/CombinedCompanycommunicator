// <copyright file="IDLUserDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DLUserData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// interface for DLUser data Repository.
    /// </summary>
    public interface IDLUserDataRepository : IRepository<DLUserDataEntity>
    {
        /// <summary>
        /// Gets table row key generator.
        /// </summary>
        public TableRowKeyGenerator TableRowKeyGenerator { get; }

        /// <summary>
        /// Get all DLUser entities from the table storage.
        /// </summary>
        /// <returns>All DLUser entities.</returns>
        public Task<IEnumerable<DLUserDataEntity>> GetAllDLUsersAsync();
    }
}
