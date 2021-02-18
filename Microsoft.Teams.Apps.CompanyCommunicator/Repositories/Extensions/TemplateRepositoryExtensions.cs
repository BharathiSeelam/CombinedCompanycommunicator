// <copyright file="TemplateRepositoryExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    /// <summary>
    /// Extensions for the repository of the template data.
    /// </summary>
    public static class TemplateRepositoryExtensions
    {
        /// <summary>
        /// Create a new channel.
        /// </summary>
        /// <param name="templateRepository">The repository.</param>
        /// <param name="template"> model class instance passed in from Web API.</param>
        /// <returns>The newly created templates's id.</returns>
        public static async Task<string> CreateTemplateAsync(
            this ITemplateDataRepository templateRepository,
            Template template)
        {
            var newId = templateRepository.TableRowKeyGenerator.CreateNewKeyOrderingOldestToMostRecent();

            var templateEntity = new TemplateDataEntity
            {
                PartitionKey = TemplateDataTableNames.TemplatePartition,
                RowKey = newId,
                TemplateID = newId,
                TemplateName = template.TemplateName,
                TemplateJSON = template.TemplateJSON,
            };

            await templateRepository.CreateOrUpdateAsync(templateEntity);

            return newId;
        }
    }
}
