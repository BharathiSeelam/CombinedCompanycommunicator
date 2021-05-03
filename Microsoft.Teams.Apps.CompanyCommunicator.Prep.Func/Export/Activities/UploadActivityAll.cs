// <copyright file="UploadActivity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Activities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using CsvHelper;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.Storage.RetryPolicies;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ChannelData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Mappers;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Streams;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.PreparingToSend;
    using Newtonsoft.Json;

    /// <summary>
    /// Uploads the file to the blob storage.
    /// </summary>
    public class UploadActivityAll
    {
        private readonly string storageConnectionString;
        private readonly IDataStreamFacade userDataStream;
        private readonly IStringLocalizer<Strings> localizer;
        private readonly INotificationDataRepository notificationDataRepository;
        private readonly ISentNotificationDataRepository sentNotificationDataRepstry;
        private readonly IChannelDataRepository channelDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadActivityAll"/> class.
        /// </summary>
        /// <param name="repositoryOptions">the repository options.</param>
        /// <param name="userDataStream">the user data stream.</param>
        /// <param name="localizer">Localization service.</param>
        /// <param name="notificationDataRepository">notificationDataRepository.</param>
        /// <param name="sentNotificationDataRepstry">sentNotificationDataRepstry.</param>
        /// <param name="channelDataRepository">Channel data repository service that deals with the table storage in azure.</param>
        public UploadActivityAll(
            IOptions<RepositoryOptions> repositoryOptions,
            IDataStreamFacade userDataStream,
            IStringLocalizer<Strings> localizer,
            INotificationDataRepository notificationDataRepository,
            ISentNotificationDataRepository sentNotificationDataRepstry,
            IChannelDataRepository channelDataRepository)
        {
            this.storageConnectionString = repositoryOptions.Value.StorageAccountConnectionString;
            this.userDataStream = userDataStream;
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.notificationDataRepository = notificationDataRepository ?? throw new ArgumentNullException(nameof(notificationDataRepository));
            this.sentNotificationDataRepstry = sentNotificationDataRepstry ?? throw new ArgumentNullException(nameof(sentNotificationDataRepstry));
            this.channelDataRepository = channelDataRepository ?? throw new ArgumentNullException(nameof(channelDataRepository));
        }

        private TimeSpan BackOffPeriod { get; set; } = TimeSpan.FromSeconds(3);

        private int MaxRetry { get; set; } = 15;

        /// <summary>
        /// Run the activity.
        /// Upload the notification data to Azure Blob storage.
        /// </summary>
        /// <param name="context">Durable orchestration context.</param>
        /// <param name="uploadData">Tuple containing notification data entity,metadata and filename.</param>
        /// <param name="log">Logging service.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RunAsync(
            IDurableOrchestrationContext context,
            (NotificationDataEntity sentNotificationDataEntity, ExportDataEntity exportDataEntity) uploadData,
            ILogger log)
        {
            await context.CallActivityWithRetryAsync(
              nameof(UploadActivityAll.UploadActivityAllAsync),
              FunctionSettings.DefaultRetryOptions,
              uploadData);
        }

        /// <summary>
        /// Upload the zip file to blob storage.
        /// </summary>
        /// <param name="uploadData">Tuple containing notification data, metadata and filename.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(nameof(UploadActivityAllAsync))]
        public async Task UploadActivityAllAsync(
            [ActivityTrigger](NotificationDataEntity sentNotificationDataEntity, ExportDataEntity exportDataEntity) uploadData)
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(this.storageConnectionString);
            CloudBlobClient client = storage.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(Common.Constants.BlobContainerName);
            await container.CreateIfNotExistsAsync();

            // Set the permissions so the blobs are private.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Off,
            };
            await container.SetPermissionsAsync(permissions);
            CloudBlockBlob blob = container.GetBlockBlobReference(uploadData.exportDataEntity.FileName);
            var blobRequestOptions = new BlobRequestOptions()
            {
                RetryPolicy = new ExponentialRetry(this.BackOffPeriod, this.MaxRetry),
                SingleBlobUploadThresholdInBytes = 1024 * 1024 * 4, // 4Mb.
                ParallelOperationThreadCount = 1, // Advised to keep 1 if upload size is less than 256 Mb.
            };

            using var memorystream = await blob.OpenWriteAsync(new AccessCondition(), blobRequestOptions, new OperationContext());
            using var archive = new ZipArchive(memorystream, ZipArchiveMode.Create);
            string channelIds = string.Empty;
            // message delivery csv creation.
            if (uploadData.exportDataEntity.UserType == "admin")
            {
                var channelDataEntity = await this.channelDataRepository.GetAllAsync();
                foreach (ChannelDataEntity channelData in channelDataEntity)
                {
                    if (channelData.ChannelAdminEmail.ToLower().Contains(uploadData.exportDataEntity.LoggedinUserEmail))
                    {
                        channelIds += channelData.RowKey;
                        channelIds += ",";
                    }
                }
            }

            var messageDeliveryFileName = string.Concat(this.localizer.GetString("FileName_Message_DeliveryDetails"), ".csv");
            var messageDeliveryFile = archive.CreateEntry(messageDeliveryFileName, CompressionLevel.Optimal);
            using (var entryStream = messageDeliveryFile.Open())
            using (var writer = new StreamWriter(entryStream, System.Text.Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var notificationDetailsMap = new NotificationDetailsExportMap(this.localizer);
                csv.Configuration.RegisterClassMap(notificationDetailsMap);
                var notificationEntities = await this.notificationDataRepository.GetMostRecentSentNotificationsAsync();
                foreach (var notificationEntity in notificationEntities)
                {
                    if (uploadData.exportDataEntity.UserType == "superAdmin")
                    {
                        var notificationDetailsStream = this.userDataStream.GetNotificationDetailsStreamAsync(notificationEntity);
                        await foreach (var data in notificationDetailsStream)
                        {
                            await csv.WriteRecordsAsync(data);
                        }
                    }
                    else if (!string.IsNullOrEmpty(channelIds) && uploadData.exportDataEntity.UserType == "admin")
                    {
                        if (channelIds.Contains(notificationEntity.Channel))
                        {
                            var notificationDetailsStream = this.userDataStream.GetNotificationDetailsStreamAsync(notificationEntity);
                            await foreach (var data in notificationDetailsStream)
                            {
                                await csv.WriteRecordsAsync(data);
                            }
                        }
                        else
                        { continue; }
                    }

                }
            }
        }
    }
}
