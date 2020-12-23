using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Controllers
{
    [ApiController]
    [Route("api/manage")]
    public class WorkpacketManagerController : Controller
    {
        private readonly IP360AuthenticationService _p360AuthenticationService;
        private readonly IWorkpacketManageService _workpacketManageService;
        private readonly IWorkpacketSearchService _workpacketSearchService;
        private readonly IOptionsMonitor<P360LoginData> _p360LoginData;

        public WorkpacketManagerController(
            IP360AuthenticationService p360AuthenticationService,
            IWorkpacketManageService workpacketManageService,
            IWorkpacketSearchService workpacketSearchService,
            IOptionsMonitor<P360LoginData> p360LoginData)
        {
            this._p360AuthenticationService = p360AuthenticationService;
            this._workpacketManageService = workpacketManageService;
            this._workpacketSearchService = workpacketSearchService;
            this._p360LoginData = p360LoginData;
        }

        //
        // MANAGE
        //


        [HttpGet("workpacketid-from-webid/{webId}")]
        public async Task<ActionResult<string>> GetWorkpacketId(string webId)
        {
            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            Workpacket workpacket;

            try
            {
                workpacket = await _workpacketSearchService.SearchAllAsync(p360loginResponse.SessionTokenHeader, (null, webId));
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }
            return workpacket?.Id;
        }
    }
}
