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
        this.channelDataRepository = channelDataRepository;
        }

        /// <summary>
             /// Get data for all Channels.
             /// </summary>
             /// <returns>A list of channel data.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllChannelDataAsync()
        {
            var entities = await this.channelDataRepository.GetAllAsync();
            var result = new List<ChannelData>();
            foreach (var entity in entities)
            {
                var channel = new ChannelData
                {
                    Id = entity.Id,
                    ChannelName = entity.ChannelName,
                };
                result.Add(channel);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Get a Channel Name by Id.
        /// </summary>
        /// <param name="id">Channel Id.</param>
        /// <returns>It returns the channel name with the passed in id.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in id is invalid, it returns 404 not found error.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChannelNameByIdAsync(string id)
        {
            var channelDataEntity = await this.channelDataRepository.GetAsync("Default", id);
            if (channelDataEntity == null)
            {
                return this.NotFound();
            }

            var result = new ChannelData
            {
                Id = channelDataEntity.Id,
                ChannelName = channelDataEntity.ChannelName,
            };

            return this.Ok(result);
        }
    }
}
