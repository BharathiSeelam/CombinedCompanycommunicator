// <copyright file="ImageUploaderController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.CommonBot;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// ImageUploaderController.
    /// </summary>
    [Route("api/uploadImage")]
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    public class ImageUploaderController : ControllerBase
    {
        private readonly BotFrameworkHttpAdapter botAdapter;
        private readonly string storageAccount;
        private readonly string blobContainerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageUploaderController"/> class.
        /// </summary>
        /// <param name="botAdapter">Bot Adapter client.</param>
        /// <param name="botOptions">BOT options.</param>
        public ImageUploaderController(BotFrameworkHttpAdapter botAdapter,
            IOptions<BotOptions> botOptions)
        {
            this.botAdapter = botAdapter ?? throw new ArgumentNullException(nameof(botAdapter));
            this.storageAccount = botOptions?.Value?.StorageAccount ?? throw new ArgumentNullException(nameof(botOptions));
            this.blobContainerName = botOptions?.Value?.BlobContainerName ?? throw new ArgumentNullException(nameof(botOptions));
        }

        /// <summary>
        /// Create a new draft notification.
        /// </summary>
        /// <param name="imageValues">New Image deatils to be uploaded to BLOB storage.</param>
        /// <returns>The created document URL.</returns>
        [HttpPost]
        public async Task<string> UploadImagetoBlob([FromBody] UploadImageEntity imageValues)
        {
            var uploadSuccess = false;
            string uploadedUri = null;
            try
            {

                (uploadSuccess, uploadedUri) = await this.UploadToBlob(imageValues.Name, imageValues.File);
                return uploadedUri;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Method for Uploading Image to Blob storage.
        /// </summary>
        /// <param name="filename">Name of the image.</param>
        /// <param name="stream">image stream.</param>
        /// <returns>return 2 values ; status and uploaded image uri.</returns>
        private async Task<(bool, string)> UploadToBlob(string filename, string stream = null)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(this.storageAccount, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    cloudBlobContainer = cloudBlobClient.GetContainerReference(this.blobContainerName);
                    bool isExist = await cloudBlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

                    // Create a container based on configuration value and append a GUID value to it to make the name unique.

                    //if (!isExist)
                    //{
                    //    cloudBlobContainer = cloudBlobClient.GetContainerReference(this.blobContainerName + Guid.NewGuid().ToString());
                    //    await cloudBlobContainer.CreateAsync();
                    //}

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob,
                    };

                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(DateTime.Now.ToString("yyyyMMddHHmmss") + filename);

                    var bytes = Convert.FromBase64String(stream.Replace("data:image/jpeg;base64,", string.Empty)); // without data:image/jpeg;base64 prefix, just base64 string.

                    using (var fileStream = new MemoryStream(bytes))
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                    }

                    return (true, cloudBlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());
                }
                catch (StorageException ex)
                {
                    return (false, null);
                }
                finally
                {
                    // OPTIONAL: Clean up resources, e.g. blob container
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                }
            }
            else
            {
                return (false, null);
            }

        }

    }
}
