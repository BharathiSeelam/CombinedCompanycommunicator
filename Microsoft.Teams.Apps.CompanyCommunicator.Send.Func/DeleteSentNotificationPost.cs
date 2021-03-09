// <copyright file="DeleteSentNotificationPost.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Microsoft.Teams.Apps.CompanyCommunicator.Send.Func
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams.Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function App triggered by messages from a http trigger
    /// Used for sending messages from the bot.
    /// </summary>
    public class DeleteSentNotificationPost
    {
        private readonly ISentUpdateandDeleteNotificationDataRepository sentNotificationDataRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSentNotificationPost"/> class.
        /// </summary>
        /// <param name="sentNotificationDataRepository">Notification data repository.</param>
        public DeleteSentNotificationPost(
            ISentUpdateandDeleteNotificationDataRepository sentNotificationDataRepository)
        {
            this.sentNotificationDataRepository = sentNotificationDataRepository ?? throw new ArgumentNullException(nameof(sentNotificationDataRepository));
        }

        /// <summary>
        /// Azure Function App triggered by delete Sent messages request is recieved.
        /// </summary>
        /// <returns>null</returns>
        /// <param name="req">Request</param>
        [FunctionName("DeleteSentNotificationPost")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            UpdateSentNotificationEntity notificationEntity = JsonConvert.DeserializeObject<UpdateSentNotificationEntity>(content);
            var resMsg = string.Empty;
            try
            {
                await this.sentNotificationDataRepository.DeleteFromPostAsync(notificationEntity.NotificationId);
            }
            catch (Exception ex)
            {
                resMsg = ex.Message;

                // throw;
            }

            string responseMessage = string.IsNullOrEmpty(resMsg)
                ? "This HTTP triggered function executed successfully."
                : "Error occurred: Error Details -" + resMsg;

            return new OkObjectResult(responseMessage); 
        }
    }
}
