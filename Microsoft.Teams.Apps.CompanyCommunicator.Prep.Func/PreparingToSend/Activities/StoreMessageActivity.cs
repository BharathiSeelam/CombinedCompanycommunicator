// <copyright file="StoreMessageActivity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.PreparingToSend
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TemplateData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.AdaptiveCard;

    /// <summary>
    /// Stores the message in sending notification data table.
    /// </summary>
    public class StoreMessageActivity
    {
        private readonly ISendingNotificationDataRepository sendingNotificationDataRepository;
        private readonly ITemplateDataRepository templateDataRepository;
        private readonly AdaptiveCardCreator adaptiveCardCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreMessageActivity"/> class.
        /// </summary>
        /// <param name="notificationRepo">Sending notification data repository.</param>
        /// <param name="templateDataRepository">Channel data repository.</param>
        /// <param name="cardCreator">The adaptive card creator.</param>
        public StoreMessageActivity(
            ISendingNotificationDataRepository notificationRepo,
            ITemplateDataRepository templateDataRepository,
            AdaptiveCardCreator cardCreator)
        {
            this.sendingNotificationDataRepository = notificationRepo ?? throw new ArgumentNullException(nameof(notificationRepo));
            this.templateDataRepository = templateDataRepository ?? throw new ArgumentNullException(nameof(templateDataRepository));
            this.adaptiveCardCreator = cardCreator ?? throw new ArgumentNullException(nameof(cardCreator));
        }

        /// <summary>
        /// Stores the message in sending notification data table.
        /// </summary>
        /// <param name="notification">A notification to be sent to recipients.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        [FunctionName(FunctionNames.StoreMessageActivity)]
        public async Task RunAsync(
            [ActivityTrigger] NotificationDataEntity notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var templateID = notification.TemplateID;
            var templateDataforcard = await this.templateDataRepository.GetFilterAsync("TemplateID eq '" + templateID + "'", "Default");
            var serializedContent = string.Empty;
            foreach (var templateData in templateDataforcard)
            {
                        serializedContent = this.adaptiveCardCreator.CreateAdaptiveCardWithoutHeader(notification, templateData.TemplateJSON);
                        break;
            }

            var sendingNotification = new SendingNotificationDataEntity
            {
                PartitionKey = NotificationDataTableNames.SendingNotificationsPartition,
                RowKey = notification.RowKey,
                NotificationId = notification.Id,
                Channel = notification.Channel,
                TemplateID = notification.TemplateID,
                Content = serializedContent,
            };

            await this.sendingNotificationDataRepository.CreateOrUpdateAsync(sendingNotification);
        }
    }
}
