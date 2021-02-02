// <copyright file="ChannelTemplateRepositoryExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelTemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    /// <summary>
    /// Extensions for the repository of the channel template data.
    /// </summary>
    public static class ChannelTemplateRepositoryExtensions
    {
        /// <summary>
        /// Create a new channel.
        /// </summary>
        /// <param name="channelTemplateRepository">The channel repository.</param>
        /// <param name="channelTemplate">Channel model class instance passed in from Web API.</param>
        /// <returns>The newly created channel's id.</returns>
        public static async Task<string> CreateChannelTemplateAsync(
            this IChannelTemplateDataRepository channelTemplateRepository,
            ChannelTemplate channelTemplate)
        {
            var newId = channelTemplateRepository.TableRowKeyGenerator.CreateNewKeyOrderingOldestToMostRecent();

            var channelTemplateEntity = new ChannelTemplateDataEntity
            {
                PartitionKey = ChannelTemplateDataTableNames.ChannelTemplatePartition,
                RowKey = newId,
                TemplateID = newId,
                TemplateName = channelTemplate.TemplateName,
                TemplateJSON = channelTemplate.TemplateJSON,
            };

            await channelTemplateRepository.CreateOrUpdateAsync(channelTemplateEntity);

            return newId;
        }
    }
}
