using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Controllers
{
    [ApiController]
    [Route("api/delete")]
    public class WorkpacketDeletionController : Controller
    {
        private readonly IWorkpacketSearchService _workpacketSearchService;
        private readonly IP360AuthenticationService _p360AuthenticationService;
        private readonly IOptionsMonitor<P360LoginData> _p360LoginData;
        private readonly IWorkpacketDeletionService _workpacketDeletionService;

        public WorkpacketDeletionController(IWorkpacketDeletionService workpacketDeletionService,
            IWorkpacketSearchService workpacketSearchService,
            IP360AuthenticationService p360AuthenticationService,
            IOptionsMonitor<P360LoginData> p360LoginData         
            )
        {
            this._workpacketDeletionService = workpacketDeletionService;
            this._workpacketSearchService = workpacketSearchService;
            this._p360AuthenticationService = p360AuthenticationService;
            this._p360LoginData = p360LoginData;
        }


        [HttpDelete("{workpacketId}")]
        public async Task<bool> DeleteWorkPacket(string workpacketId)
        {
            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            bool response = false;
            try
            {
                var workpacketResult = await _workpacketSearchService.SearchAsync(p360loginResponse.SessionTokenHeader, workpacketId);
                response = await _workpacketDeletionService.DeleteWorkPacketAsync(p360loginResponse.SessionTokenHeader, workpacketResult.Id);
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader,_p360LoginData.CurrentValue);
            }

            return response;
        }

    }

}
