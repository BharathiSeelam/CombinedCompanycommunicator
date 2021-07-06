// <copyright file="DistributionListController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    /// <summary>
    /// Controller for the distribution list data.
    /// </summary>
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/distributionLists")]
    public class DistributionListController : ControllerBase
    {
        private readonly IDistributionListDataRepository distributionListDataRepository;
        private readonly INotificationDataRepository notificationDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionListController"/> class.
        /// </summary>
        /// <param name="distributionListDataRepository">DistributionList data repository instance.</param>
        /// <param name="notificationDataRepository">Notification data repository instance.</param>
        public DistributionListController(
            INotificationDataRepository notificationDataRepository,
            IDistributionListDataRepository distributionListDataRepository)
        {
            this.distributionListDataRepository = distributionListDataRepository ?? throw new ArgumentNullException(nameof(distributionListDataRepository));
            this.notificationDataRepository = notificationDataRepository ?? throw new ArgumentNullException(nameof(notificationDataRepository));
        }

        /// <summary>
        /// Get distribution lists.
        /// </summary>
        /// <returns>A list of <see cref="DistributionList"/> instances.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistributionList>>> GetAllDistributionListsAsync()
        {
            var distributionListEntities = await this.distributionListDataRepository.GetAllDistributionListsAsync();

            var result = new List<DistributionList>();
            foreach (var distributionListEntity in distributionListEntities)
            {
                var distributionLists = new DistributionList
                {
                    DLID = distributionListEntity.DLID,
                    DLName = distributionListEntity.DLName,
                    DLMail = distributionListEntity.DLMail,
                    DLMemberCount = distributionListEntity.DLMemberCount,
                    GroupType = distributionListEntity.GroupType,
                };

                result.Add(distributionLists);
            }

            return result;
        }

        /// <summary>
        /// Get a DL name by name.
        /// </summary>
        /// <param name="dLName">DL name.</param>
        /// <returns>It returns the DL details with the passed in name.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in id is invalid, it returns 404 not found error.</returns>
        [HttpGet("{DLName}")]
        public async Task<ActionResult<IEnumerable<DistributionList>>> GetDLByNameAsync(string dLName)
        {
            var distributionListEntities = await this.distributionListDataRepository.GetWithFilterAsync("DLMail eq '" + dLName.ToLower() + "'", "Default");

            var result = new List<DistributionList>();
            foreach (var distributionListEntity in distributionListEntities)
            {
                var distributionLists = new DistributionList
                {
                    DLID = distributionListEntity.DLID,
                    DLName = distributionListEntity.DLName,
                    DLMail = distributionListEntity.DLMail,
                    DLMemberCount = distributionListEntity.DLMemberCount,
                    GroupType = distributionListEntity.GroupType,
                };

                result.Add(distributionLists);
            }

            return result;
        }

        /// <summary>
        /// Get a DL name by name.
        /// </summary>
        /// <param name="dLID">DL name.</param>
        /// <returns>It returns the DL details with the passed in name.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in id is invalid, it returns 404 not found error.</returns>
        [HttpGet("draft/{dLID}")]
        public async Task<ActionResult<DistributionList>> GetDLByIDAsync(string dLID)
        {
            var distributionListEntities = await this.distributionListDataRepository.GetWithFilterAsync("DLID eq '" + dLID + "'", "Default");

            var result = new List<DistributionList>();
            foreach (var distributionListEntity in distributionListEntities)
            {
                var distributionLists = new DistributionList
                {
                    DLID = distributionListEntity.DLID,
                    DLName = distributionListEntity.DLName,
                    DLMail = distributionListEntity.DLMail,
                    DLMemberCount = distributionListEntity.DLMemberCount,
                    GroupType = distributionListEntity.GroupType,
                };

                result.Add(distributionLists);
            }

            return this.Ok(result);
        }
    }
}
