/// <summary>
/// Function to edit posted notifications
/// </summary>
namespace Microsoft.Teams.Apps.CompanyCommunicator.Send.Func
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Extensions;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.SendQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams.Messages;
    using Microsoft.Teams.Apps.CompanyCommunicator.Send.Func.Services;
    using Newtonsoft.Json;

    /// <summary>
    /// Function to edit posted notifications
    /// </summary>
    public class SentUpdatedNotificationFunction
    {
        /// <summary>
        /// This is set to 10 because the default maximum delivery count from the service bus
        /// message queue before the service bus will automatically put the message in the Dead Letter
        /// Queue is 10.
        /// </summary>
        private readonly int maxNumberOfAttempts;
        private readonly double sendRetryDelayNumberOfSeconds;
        private readonly INotificationService notificationService;
        private readonly ISendingNotificationDataRepository notificationRepo;
        private readonly IMessageService messageService;
        private readonly ISendQueue sendQueue;
        private readonly IStringLocalizer<Strings> localizer;
        private readonly ISentUpdateDataRepository sentNotificationUpdateDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentUpdatedNotificationFunction"/> class.
        /// </summary>
        /// <param name="options">Send function options.</param>
        /// <param name="notificationService">The service to precheck and determine if the queue message should be processed.</param>
        /// <param name="messageService">Message service.</param>
        /// <param name="notificationRepo">Notification repository.</param>
        /// <param name="sendQueue">The send queue.</param>
        /// <param name="localizer">Localization service.</param>
        /// <param name="sentNotificationUpdateDataRepository">sentNotificationUpdateDataRepository</param>
        public SentUpdatedNotificationFunction(
            IOptions<SendFunctionOptions> options,
            INotificationService notificationService,
            IMessageService messageService,
            ISendingNotificationDataRepository notificationRepo,
            ISendQueue sendQueue,
            IStringLocalizer<Strings> localizer,
            ISentUpdateDataRepository sentNotificationUpdateDataRepository)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.maxNumberOfAttempts = options.Value.MaxNumberOfAttempts;
            this.sendRetryDelayNumberOfSeconds = options.Value.SendRetryDelayNumberOfSeconds;

            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            this.notificationRepo = notificationRepo ?? throw new ArgumentNullException(nameof(notificationRepo));
            this.sendQueue = sendQueue ?? throw new ArgumentNullException(nameof(sendQueue));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.sentNotificationUpdateDataRepository = sentNotificationUpdateDataRepository ?? throw new ArgumentNullException(nameof(sentNotificationUpdateDataRepository));
        }

        /// <summary>
        /// SentUpdatedNotificationFunction.
        /// </summary>
        /// <param name="req"> reg. </param>
        /// <param name="log"> log. </param>
        /// <returns> status. </returns>
        [FunctionName("SentUpdatedNotificationFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var content = await new StreamReader(req.Body).ReadToEndAsync();
            UpdateSentNotificationEntity notificationEntity = JsonConvert.DeserializeObject<UpdateSentNotificationEntity>(content);
            var resMsg = string.Empty;
            try
            {
                await this.sentNotificationUpdateDataRepository.UpdateFromPostAsync(notificationEntity.NotificationId, notificationEntity.NotificationEntity);
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
