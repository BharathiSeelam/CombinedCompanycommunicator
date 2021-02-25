namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.SchduleMessage
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.DataQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.PrepareToSendQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.PreparingToSend;

    /// <summary>
    /// Function for Queue the Scheduled message.
    /// </summary>
    public class QueueScheduledMessage
    {
        private readonly INotificationDataRepository notificationDataRepository;
        private readonly IPrepareToSendQueue prepareToSendQueue;
        private readonly IDataQueue dataQueue;
        private readonly double forceCompleteMessageDelayInSeconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueScheduledMessage"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">Notification data repository.</param>
        /// <param name="prepareToSendQueue">Prepare to send queue service.</param>
        /// <param name="dataQueue">data queue service.</param>
        /// <param name="dataQueueMessageOptions">data que message option service.</param>
        public QueueScheduledMessage(
            INotificationDataRepository notificationDataRepository, IPrepareToSendQueue prepareToSendQueue, IDataQueue dataQueue, IOptions<DataQueueMessageOptions> dataQueueMessageOptions)
        {
            if (dataQueueMessageOptions is null)
            {
                throw new ArgumentNullException(nameof(dataQueueMessageOptions));
            }

            this.notificationDataRepository = notificationDataRepository ?? throw new ArgumentNullException(nameof(notificationDataRepository));
            this.prepareToSendQueue = prepareToSendQueue ?? throw new ArgumentNullException(nameof(prepareToSendQueue));
            this.dataQueue = dataQueue ?? throw new ArgumentNullException(nameof(dataQueue));
            this.forceCompleteMessageDelayInSeconds = dataQueueMessageOptions.Value.ForceCompleteMessageDelayInSeconds;
        }

        /// <summary>
        /// Azure Function App triggered by timer to check the scheduled messages.
        /// </summary>
        /// <param name="myTimer">timer information.</param>
        /// <param name="log">logger information.</param>
        [FunctionName("QueueScheduledMessage")]
        public async void Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var strFilter = "PublishOn eq '" + this.RoundAndFormatDateTime() + "'";
            var draftNotifications = await this.notificationDataRepository.GetWithFilterAsync(strFilter);
            foreach (var draftEntity in draftNotifications)
            {
                var newSentNotificationId =
                await this.notificationDataRepository.MoveDraftToSentPartitionAsync(draftEntity);

                var prepareToSendQueueMessageContent = new PrepareToSendQueueMessageContent
                {
                    NotificationId = newSentNotificationId,
                };
                await this.prepareToSendQueue.SendAsync(prepareToSendQueueMessageContent);

                var forceCompleteDataQueueMessageContent = new DataQueueMessageContent
                {
                    NotificationId = newSentNotificationId,
                    ForceMessageComplete = true,
                };
                await this.dataQueue.SendDelayedAsync(
                    forceCompleteDataQueueMessageContent,
                    this.forceCompleteMessageDelayInSeconds);
            }

        }

        private string RoundAndFormatDateTime()
        {
            DateTime dt = DateTime.Now;
            try
            {
                return dt.AddMinutes(-dt.Minute % 10).ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
