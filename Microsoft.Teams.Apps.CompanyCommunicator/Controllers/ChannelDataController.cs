// <copyright file="ChannelDataController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;
    using Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions;

    /// <summary>
    /// Controller for the Channel data.
    /// </summary>
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/channelData")]
    public class ChannelDataController : Controller
    {
        private readonly IChannelDataRepository channelDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDataController"/> class.
        /// </summary>
        /// <param name="channelDataRepository">Channel data repository instance.</param>
        public ChannelDataController(
            IChannelDataRepository channelDataRepository)
        {
            this.channelDataRepository = channelDataRepository ?? throw new ArgumentNullException(nameof(channelDataRepository));
        }

        /// <summary>
        /// Get data for all Channels.
        /// </summary>
        /// <returns>A list of <see cref="ChannelData"/> instances.</returns>
        [HttpGet]
        public async Task<ActionResult<ChannelData>> GetAllChannelDataAsync()
        {
            var channelEntities = await this.channelDataRepository.GetAllSortedAlphabeticallyByNameAsync();

            var result = new List<ChannelData>();
            foreach (var channelEntity in channelEntities)
            {
                var channels = new ChannelData
                {
                    Id = channelEntity.Id,
                    ChannelName = channelEntity.ChannelName,
                    ChannelDescription = channelEntity.ChannelDescription,
                    ChannelAdmins = channelEntity.ChannelAdmins,
                    ChannelAdminDLs = channelEntity.ChannelAdminDLs,
                    ChannelAdminEmail = channelEntity.ChannelAdminEmail,
                };

                result.Add(channels);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Get a channel by Id.
        /// </summary>
        /// <param name="id">Channel Id.</param>
        /// <returns>It returns the channel with the passed in id.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in id is invalid, it returns 404 not found error.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChannelByIdAsync(string id)
        {
            var channelEntity = await this.channelDataRepository.GetAsync(
                ChannelDataTableName.ChannelDataPartition,
                id);
            if (channelEntity == null)
            {
                return this.NotFound();
            }

            var result = new ChannelData
            {
                Id = channelEntity.Id,
                ChannelName = channelEntity.ChannelName,
                ChannelDescription = channelEntity.ChannelDescription,
                ChannelAdmins = channelEntity.ChannelAdmins,
                ChannelAdminDLs = channelEntity.ChannelAdminDLs,
                ChannelAdminEmail = channelEntity.ChannelAdminEmail,
            };

            return this.Ok(result);
        }

        /// <summary>
        /// Get a channel by AdminEmail.
        /// </summary>
        /// <param name="channelAdminEmail">Channel AdminEmail.</param>
        /// <param name="id">Id.</param>
        /// <returns>It returns the channel with the passed in AdminEmail.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in Email is invalid, it returns 404 not found error.</returns>
        [HttpGet("channelAdmin/{channelAdminEmail}/{id}")]
        public async Task<ActionResult<ChannelData>> GetChannelByAdminEmailAsync(string channelAdminEmail, string id)
        {
            var channelEntities = await this.channelDataRepository.GetWithFilterAsync("Id eq '" + id + "'", "Default");

            var result = new List<ChannelData>();
            foreach (var channelEntity in channelEntities)
            {
                var loggedinUser = channelEntity.ChannelAdminEmail.Split(",");
                if (loggedinUser.Length >= 0)
                {
                foreach (var loggedin in loggedinUser)
                    {
                    if (loggedin == channelAdminEmail)
                        {
                            var channels = new ChannelData
                            {
                                Id = channelEntity.Id,
                                ChannelName = channelEntity.ChannelName,
                                ChannelDescription = channelEntity.ChannelDescription,
                                ChannelAdmins = channelEntity.ChannelAdmins,
                                ChannelAdminDLs = channelEntity.ChannelAdminDLs,
                                ChannelAdminEmail = channelEntity.ChannelAdminEmail,
                            };
                            result.Add(channels);
                        }
                    }
                }
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Create a new channel.
        /// </summary>
        /// <param name="channel">A new Channel to be created.</param>
        /// <returns>The created channel's id.</returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateChannelAsync([FromBody] ChannelData channel)
        {
            var channeId = await this.channelDataRepository.CreateChannelAsync(
                channel);
            return this.Ok(channeId);
        }

        /// <summary>
        /// Update an existing channel.
        /// </summary>
        /// <param name="channel">An existing Channel to be updated.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChannelAsync([FromBody] ChannelData channel)
        {
            var channelEntity = new ChannelDataEntity
            {
                PartitionKey = ChannelDataTableName.ChannelDataPartition,
                RowKey = channel.Id,
                Id = channel.Id,
                ChannelName = channel.ChannelName,
                ChannelDescription = channel.ChannelDescription,
                ChannelAdmins = channel.ChannelAdmins,
                ChannelAdminDLs = channel.ChannelAdminDLs,
                ChannelAdminEmail = channel.ChannelAdminEmail,
            };

            await this.channelDataRepository.CreateOrUpdateAsync(channelEntity);
            return this.Ok();
        }

        /// <summary>
        /// Delete an existing channel.
        /// </summary>
        /// <param name="id">The id of the channel to be deleted.</param>
        /// <returns>If the passed in Id is invalid, it returns 404 not found error. Otherwise, it returns 200 OK.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChannelAsync(string id)
        {
            var channelEntity = await this.channelDataRepository.GetAsync(
                ChannelDataTableName.ChannelDataPartition,
                id);
            if (channelEntity == null)
            {
                return this.NotFound();
            }

            await this.channelDataRepository.DeleteAsync(channelEntity);
            return this.Ok();
        }
    }
}
