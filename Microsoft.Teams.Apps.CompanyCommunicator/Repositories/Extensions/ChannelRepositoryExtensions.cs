// <copyright file="ChannelRepositoryExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    /// <summary>
    /// Extensions for the repository of the channel data.
    /// </summary>
    public static class ChannelRepositoryExtensions
    {
        /// <summary>
        /// Create a new channel.
        /// </summary>
        /// <param name="channelRepository">The channel repository.</param>
        /// <param name="channel">Channel model class instance passed in from Web API.</param>
        /// <returns>The newly created channel's id.</returns>
        public static async Task<string> CreateChannelAsync(
            this IChannelDataRepository channelRepository,
            ChannelData channel)
        {
            var newId = channelRepository.TableRowKeyGenerator.CreateNewKeyOrderingOldestToMostRecent();

            var channelEntity = new ChannelDataEntity
            {
                PartitionKey = ChannelDataTableName.ChannelDataPartition,
                RowKey = newId,
                Id = newId,
                ChannelName = channel.ChannelName,
                ChannelDescription = channel.ChannelDescription,
                ChannelAdmins = channel.ChannelAdmins,
                ChannelAdminDLs = channel.ChannelAdminDLs,
                ChannelAdminEmail = channel.ChannelAdminEmail,
            };

            await channelRepository.CreateOrUpdateAsync(channelEntity);

            return newId;
        }
    }
}
