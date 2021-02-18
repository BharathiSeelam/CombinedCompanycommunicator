// <copyright file="TemplateController.cs" company="Microsoft">
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
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;
    using Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions;

    /// <summary>
    /// Controller for the template data.
    /// </summary>
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/template")]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateDataRepository templateDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateController"/> class.
        /// </summary>
        /// <param name="templateDataRepository">template data repository instance.</param>
        public TemplateController(
            ITemplateDataRepository templateDataRepository)
        {
            this.templateDataRepository = templateDataRepository ?? throw new ArgumentNullException(nameof(templateDataRepository));
        }

        /// <summary>
        /// Get templates.
        /// </summary>
        /// <returns>A list of <see cref="Template"/> instances.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Template>>> GetAllTemplatesAsync()
        {
            var templateEntities = await this.templateDataRepository.GetAllTemplatesAsync();

            var result = new List<Template>();
            foreach (var templateEntity in templateEntities)
            {
                var templates = new Template
                {
                    TemplateID = templateEntity.TemplateID,
                    TemplateName = templateEntity.TemplateName,
                    TemplateJSON = templateEntity.TemplateJSON,
                };

                result.Add(templates);
            }

            return result;
        }

        /// <summary>
        /// Get a template by Id.
        /// </summary>
        /// <param name="id">Template Id.</param>
        /// <returns>It returns the template with the passed in id.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in id is invalid, it returns 404 not found error.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Template>> GetTemplateByIdAsync(string id)
        {
            var templateEntity = await this.templateDataRepository.GetAsync(
                TemplateDataTableNames.TemplatePartition,
                id);
            if (templateEntity == null)
            {
                return this.NotFound();
            }

            var result = new Template
            {
                TemplateID = templateEntity.TemplateID,
                TemplateName = templateEntity.TemplateName,
                TemplateJSON = templateEntity.TemplateJSON,
            };

            return this.Ok(result);
        }

        /// <summary>
        /// Create a new template.
        /// </summary>
        /// <param name="template">A new Template to be created.</param>
        /// <returns>The created template's id.</returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateTemplateAsync([FromBody] Template template)
        {
            var templateId = await this.templateDataRepository.CreateTemplateAsync(
                template);
            return this.Ok(templateId);
        }

        /// <summary>
        /// Update an existing template.
        /// </summary>
        /// <param name="template">An existing Template to be updated.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplateAsync([FromBody] Template template)
        {
            var templateEntity = new TemplateDataEntity
            {
                PartitionKey = TemplateDataTableNames.TemplatePartition,
                RowKey = template.TemplateID,
                TemplateID = template.TemplateID,
                TemplateName = template.TemplateName,
                TemplateJSON = template.TemplateJSON,
            };

            await this.templateDataRepository.CreateOrUpdateAsync(templateEntity);
            return this.Ok();
        }

        /// <summary>
        /// Delete an existing template.
        /// </summary>
        /// <param name="id">The id of the template to be deleted.</param>
        /// <returns>If the passed in Id is invalid, it returns 404 not found error. Otherwise, it returns 200 OK.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplateAsync(string id)
        {
            var templateEntity = await this.templateDataRepository.GetAsync(
                TemplateDataTableNames.TemplatePartition,
                id);
            if (templateEntity == null)
            {
                return this.NotFound();
            }

            await this.templateDataRepository.DeleteAsync(templateEntity);
            return this.Ok();
        }
    }
}
