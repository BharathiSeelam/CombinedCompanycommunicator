namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    [Route("api/appsettings")]
    public class AppsettingsController : Controller
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppsettingsController"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public AppsettingsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<AppsettingsData>> GetAppsettingsDataAsync()
        {

            this.ViewBag.connectionstring = this.configuration["AuthorizedCreatorUpns"];
            var result = new List<AppsettingsData>();
            var settings = new AppsettingsData
            {
                AuthorizedCreatorUpns = this.ViewBag.connectionstring,
            };
            result.Add(settings);            
            return this.Ok(result);
        }
    }
}
