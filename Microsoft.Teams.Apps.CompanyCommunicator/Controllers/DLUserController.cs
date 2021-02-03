// <copyright file="DLUserController.cs" company="Microsoft">
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
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.DLUserData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;
    using Microsoft.Teams.Apps.CompanyCommunicator.Repositories.Extensions;

    /// <summary>
    /// Controller for the dluser data.
    /// </summary>
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/dlUsers")]
    public class DLUserController : ControllerBase
    {
        private readonly IDLUserDataRepository dlUserDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DLUserController"/> class.
        /// </summary>
        /// <param name="dlUserDataRepository">dlUser data repository instance.</param>
        public DLUserController(
            IDLUserDataRepository dlUserDataRepository)
        {
            this.dlUserDataRepository = dlUserDataRepository ?? throw new ArgumentNullException(nameof(dlUserDataRepository));
        }

        /// <summary>
        /// Get dlusers.
        /// </summary>
        /// <returns>A list of <see cref="DLUser"/> instances.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DLUser>>> GetAllDLUsersAsync()
        {
            var dlUserEntities = await this.dlUserDataRepository.GetAllDLUsersAsync();

            var result = new List<DLUser>();
            foreach (var dlUserEntity in dlUserEntities)
            {
                var dlUsers = new DLUser
                {
                    UserID = dlUserEntity.UserID,
                    DLName = dlUserEntity.DLName,
                    TeamsID = dlUserEntity.TeamsID,
                    UserEmail = dlUserEntity.UserEmail,
                    UserName = dlUserEntity.UserName,
                };

                result.Add(dlUsers);
            }

            return result;
        }

        /// <summary>
        /// Get a dluser by Id.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>It returns the dluser with the passed in userName.
        /// The returning value is wrapped in a ActionResult object.
        /// If the passed in userName is invalid, it returns 404 not found error.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DLUser>> GetDLUserByIdAsync(string id)
        {
            var dlUserEntity = await this.dlUserDataRepository.GetAsync(
                DLUserDataTableNames.DLUserPartition,
                id);
            if (dlUserEntity == null)
            {
                return this.NotFound();
            }

            var result = new DLUser
            {
                UserID = dlUserEntity.UserID,
                DLName = dlUserEntity.DLName,
                TeamsID = dlUserEntity.TeamsID,
                UserEmail = dlUserEntity.UserEmail,
                UserName = dlUserEntity.UserName,
            };

            return this.Ok(result);
        }
    }
}