// <copyright file="SendPreviewFunction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Send.Func
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function App triggered by communicator app for preview
    /// Used for sending preview messages from the bot.
    /// </summary>
    public class SendPreviewFunction
    {

        private readonly IMessageService messageService;
        private readonly string botAppId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPreviewFunction"/> class.
        /// </summary>
        /// <param name="messageService">message service param.</param>
        public SendPreviewFunction(IMessageService messageService)
        {
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        }

        /// <summary>
        /// Funtion app for Send preview card.
        /// </summary>
        /// <param name="req">req param.</param>
        /// <param name="log">logger log param.</param>
        /// <returns>Http status response.</returns>
        [FunctionName("SendPreviewFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            PreviewDataEntity previewDataEntity = JsonConvert.DeserializeObject<PreviewDataEntity>(content);
            var response = await this.messageService.SendPreviewMessageAsync(previewDataEntity, 100, log);

            string responseMessage = (response.StatusCode == StatusCodes.Status201Created)
                ? "Preview Created."
                : $"Failed to create preview. Please try again.";

            return new OkObjectResult(responseMessage);
        }
    }
}
