// <copyright file="ExportController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.ExportData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.TeamData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.ExportQueue;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    /// <summary>
    /// Coontroller for exporting notification.
    /// </summary>
    [Route("api/exportnotification")]
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    public class ExportController : Controller
    {
        private readonly ISentNotificationDataRepository sentNotificationDataRepository;
        private readonly IExportDataRepository exportDataRepository;
        private readonly IUserDataRepository userDataRepository;
        private readonly IExportQueue exportQueue;
        private readonly ITeamMembersService memberService;
        private readonly ITeamDataRepository teamDataRepository;
        private readonly IAppSettingsService appSettingsService;
        private readonly IConfiguration configuration;
        private string loggedinUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportController"/> class.
        /// </summary>
        /// <param name="sentNotificationDataRepository">SentNotification data repository instance.</param>
        /// <param name="exportDataRepository">Export data repository instance.</param>
        /// <param name="userDataRepository">User data repository instance.</param>
        /// <param name="exportQueue">The service bus queue for the export queue.</param>
        /// <param name="memberService">Teams member service.</param>
        /// <param name="teamDataRepository">Team data reporsitory.</param>
        /// <param name="appSettingsService">App Settings service.</param>
        /// <param name="configuration">configuration.</param>
        public ExportController(
            ISentNotificationDataRepository sentNotificationDataRepository,
            IExportDataRepository exportDataRepository,
            IUserDataRepository userDataRepository,
            IExportQueue exportQueue,
            ITeamMembersService memberService,
            ITeamDataRepository teamDataRepository,
            IAppSettingsService appSettingsService
           // IConfiguration configuration
            )
        {
            this.sentNotificationDataRepository = sentNotificationDataRepository ?? throw new ArgumentNullException(nameof(sentNotificationDataRepository));
            this.exportDataRepository = exportDataRepository ?? throw new ArgumentNullException(nameof(exportDataRepository));
            this.userDataRepository = userDataRepository ?? throw new ArgumentNullException(nameof(userDataRepository));
            this.exportQueue = exportQueue ?? throw new ArgumentNullException(nameof(exportQueue));
            this.memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this.teamDataRepository = teamDataRepository ?? throw new ArgumentNullException(nameof(teamDataRepository));
            this.appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
            //this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Initiate a export of notification.
        /// </summary>
        /// <param name="exportRequest">export request.</param>
        /// <returns>The result of an action method.</returns>
        [HttpPut("export")]
        public async Task<IActionResult> ExportNotificationAsync(
            [FromBody]ExportRequest exportRequest)
        {
            //var appsettingsadmin = this.configuration["AuthorizedCreatorUpns"];
            var authorizedCreatorUpns = "homerun1 @nao365competency.onmicrosoft.com";
             string[] superAdminsArray = authorizedCreatorUpns.Split(",");
            var superAdmins = string.Join(",", superAdminsArray).ToLower();
            this.loggedinUser = this.HttpContext.User?.Identity?.Name;
            var sLoggedin = string.Empty + this.loggedinUser;
            this.loggedinUser = sLoggedin.ToLower();
            //var userType = "admin";
            //if (superAdmins.Contains(this.loggedinUser))
            //{
            //    userType = "superAdmin";
            //}
            var userType = superAdmins.Contains(this.loggedinUser) ? "superAdmin" : "admin";

            var userId = this.HttpContext.User.FindFirstValue(Common.Constants.ClaimTypeUserId);
            var user = await this.userDataRepository.GetAsync(UserDataTableNames.AuthorDataPartition, userId);
            if (user == null)
            {
               // await this.SyncAuthorAsync(exportRequest.TeamId, userId);               
            }

            // Ensure the data tables needed by the Azure Function to export the notification exist in Azure storage.
            await Task.WhenAll(
                this.sentNotificationDataRepository.EnsureSentNotificationDataTableExistsAsync(),
                this.exportDataRepository.EnsureExportDataTableExistsAsync());
            var exportNotification = await this.exportDataRepository.GetAsync(userId, exportRequest.Id);
            if (exportNotification != null)
            {
                return this.Conflict();
            }
            var exportType = "ExportSingleNotifications";
            if (exportRequest.Id == "dummy")
            {
                exportRequest.Id = Guid.NewGuid().ToString();
                exportType = "ExportAllNotifications";
            }

            await this.exportDataRepository.CreateOrUpdateAsync(new ExportDataEntity()
            {
                PartitionKey = userId,
                RowKey = exportRequest.Id,
                SentDate = DateTime.UtcNow,
                Status = ExportStatus.New.ToString(),
                ExportType = exportType,
                UserType = userType,
                RequestedTeamId = exportRequest.TeamId,
            });

            var exportQueueMessageContent = new ExportQueueMessageContent
            {
                NotificationId = exportRequest.Id,
                UserId = userId,
            };
            await this.exportQueue.SendAsync(exportQueueMessageContent);

            return this.Ok();
        }

        private async Task SyncAuthorAsync(string teamId, string userId)
        {
            var tenantId = this.HttpContext.User.FindFirstValue(Common.Constants.ClaimTypeTenantId);
            var serviceUrl = await this.appSettingsService.GetServiceUrlAsync();

            // Sync members.
            var userEntities = await this.memberService.GetAuthorsAsync(
                teamId: teamId,
                tenantId: tenantId,
                serviceUrl: serviceUrl);

            var userData = userEntities.FirstOrDefault(user => user.AadId.Equals(userId));
            if (userData == null)
            {
                throw new ApplicationException("Unable to find user in Team roster");
            }

            userData.PartitionKey = UserDataTableNames.AuthorDataPartition;
            userData.RowKey = userData.AadId;
            await this.userDataRepository.CreateOrUpdateAsync(userData);
        }
    }
}
