// <copyright file="DistributionListController.cs" company="Microsoft">
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
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DistributionListData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    /// <summary>
    /// Controller for the distribution list data.
    /// </summary>
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/distributionLists")]
    public class DistributionListController : ControllerBase
    {
        private readonly IDistributionListDataRepository distributionListDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionListController"/> class.
        /// </summary>
        /// <param name="distributionListDataRepository">DistributionList data repository instance.</param>
        public DistributionListController(
            IDistributionListDataRepository distributionListDataRepository)
        {
            this.distributionListDataRepository = distributionListDataRepository ?? throw new ArgumentNullException(nameof(distributionListDataRepository));
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
                    GroupType = distributionListEntity.GroupType,
                };

                result.Add(distributionLists);
            }

            return result;
        }
    }
}
