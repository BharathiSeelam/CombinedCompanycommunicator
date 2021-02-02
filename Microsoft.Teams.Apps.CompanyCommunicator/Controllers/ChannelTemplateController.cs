// <copyright file="ChannelTemplateController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelTemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;
    using Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions;

    /// <summary>
    /// Controller for the channel data.
    /// </summary>
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/channelTemplate")]
    public class ChannelTemplateController : ControllerBase
    {
        private readonly IChannelTemplateDataRepository channelTemplateDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelTemplateController"/> class.
        /// </summary>
        /// <param name="channelTemplateDataRepository">channelTemplate data repository instance.</param>
        public ChannelTemplateController(
            IChannelTemplateDataRepository channelTemplateDataRepository)
        {
            this.channelTemplateDataRepository = channelTemplateDataRepository ?? throw new ArgumentNullException(nameof(channelTemplateDataRepository));
        }

        /// <summary>
        /// Get channelTemplates.
        /// </summary>
        /// <returns>A list of <see cref="ChannelTemplate"/> instances.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChannelTemplate>>> GetAllChannelTemplatesAsync()
        {
            var channelTemplateEntities = await this.channelTemplateDataRepository.GetAllChannelTemplatesAsync();

            var result = new List<ChannelTemplate>();
            foreach (var channelTemplateEntity in channelTemplateEntities)
            {
                var channelTemplates = new ChannelTemplate
                {
                    TemplateID = channelTemplateEntity.TemplateID,
                    TemplateName = channelTemplateEntity.TemplateName,
                    TemplateJSON = channelTemplateEntity.TemplateJSON,
                };

                result.Add(channelTemplates);
            }

            return result;
        }

        /// <summary>
        /// Get a channel template by Id.
        /// </summary>
        /// <param name="id">Channel Template Id.</param>
        /// <returns>It returns the channel template with the passed in id.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in id is invalid, it returns 404 not found error.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ChannelTemplate>> GetChannelTemplateByIdAsync(string id)
        {
            var channelTemplateEntity = await this.channelTemplateDataRepository.GetAsync(
                ChannelTemplateDataTableNames.ChannelTemplatePartition,
                id);
            if (channelTemplateEntity == null)
            {
                return this.NotFound();
            }

            var result = new ChannelTemplate
            {
                TemplateID = channelTemplateEntity.TemplateID,
                TemplateName = channelTemplateEntity.TemplateName,
                TemplateJSON = channelTemplateEntity.TemplateJSON,
            };

            return this.Ok(result);
        }

        /// <summary>
        /// Create a new channel template.
        /// </summary>
        /// <param name="channelTemplate">A new Channel Template to be created.</param>
        /// <returns>The created channel template's id.</returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateChannelTemplateAsync([FromBody] ChannelTemplate channelTemplate)
        {
            var channeTemplateId = await this.channelTemplateDataRepository.CreateChannelTemplateAsync(
                channelTemplate);
            return this.Ok(channeTemplateId);
        }

        /// <summary>
        /// Update an existing channel template.
        /// </summary>
        /// <param name="channelTemplate">An existing Channel Template to be updated.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChannelTemplateAsync([FromBody] ChannelTemplate channelTemplate)
        {
            var channelTemplateEntity = new ChannelTemplateDataEntity
            {
                PartitionKey = ChannelTemplateDataTableNames.ChannelTemplatePartition,
                RowKey = channelTemplate.TemplateID,
                TemplateID = channelTemplate.TemplateID,
                TemplateName = channelTemplate.TemplateName,
                TemplateJSON = channelTemplate.TemplateJSON,
            };

            await this.channelTemplateDataRepository.CreateOrUpdateAsync(channelTemplateEntity);
            return this.Ok();
        }

        /// <summary>
        /// Delete an existing channel template.
        /// </summary>
        /// <param name="id">The id of the channel template to be deleted.</param>
        /// <returns>If the passed in Id is invalid, it returns 404 not found error. Otherwise, it returns 200 OK.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChannelTemplateAsync(string id)
        {
            var channelTemplateEntity = await this.channelTemplateDataRepository.GetAsync(
                ChannelTemplateDataTableNames.ChannelTemplatePartition,
                id);
            if (channelTemplateEntity == null)
            {
                return this.NotFound();
            }

            await this.channelTemplateDataRepository.DeleteAsync(channelTemplateEntity);
            return this.Ok();
        }
    }
}
