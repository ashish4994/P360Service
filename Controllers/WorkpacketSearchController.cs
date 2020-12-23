using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class WorkpacketSearchController : Controller
    {
        private readonly IP360AuthenticationService _p360AuthenticationService;
        private readonly IWorkpacketSearchService _workpacketSearchService;
        private readonly IOptionsMonitor<P360LoginData> _p360LoginData;

        public WorkpacketSearchController(
            IP360AuthenticationService p360AuthenticationService,
            IWorkpacketSearchService workpacketSearchService,
            IOptionsMonitor<P360LoginData> p360LoginData
            )
        {
            this._p360AuthenticationService = p360AuthenticationService;
            this._p360LoginData = p360LoginData;
            this._workpacketSearchService = workpacketSearchService;
        }

        /// <summary>
        /// Search worklists and archive
        /// </summary>
        /// <param name="workpacketId"></param>
        /// <returns></returns>
        [HttpGet("all/{workpacketId}")]
        public async Task<ActionResult<WorkpacketSearchResponse>> SearchAll(string workpacketId)
        {
            WorkpacketSearchResponse result = null;

            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            try
            {

                result = await _workpacketSearchService.SearchForClient(p360loginResponse.SessionTokenHeader, workpacketId);
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }

            return result;
        }

        [HttpGet("worklist/{workpacketId}")]
        public async Task<ActionResult<Workpacket>> Search(string workpacketId)
        {
            Workpacket result = null;

            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            try
            {
                result = await _workpacketSearchService.SearchAsync(p360loginResponse.SessionTokenHeader, workpacketId);
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }
            return result;
        }

        [HttpGet("archive/{workpacketId}")]
        public async Task<ActionResult<Workpacket>> SearchArchive(string workpacketId)
        {
            Workpacket result = null;

            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            try
            {
                result = await _workpacketSearchService.SearchArchiveAsync(p360loginResponse.SessionTokenHeader, ("NAME_CHANGE", workpacketId));
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }

            return result;
        }

        [HttpGet("by-account/{creditAccountId}")]
        public async Task<ActionResult<List<Workpacket>>> SearchWorkpacketBy(long creditAccountId)
        {
            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            List<Workpacket> result;
            try
            {
                result = await _workpacketSearchService.SearchByAccountAsync(p360loginResponse.SessionTokenHeader, creditAccountId);
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }
            return result;
        }

        [HttpGet("archive-by-account/{creditAccountId}")]
        public async Task<ActionResult<Workpacket>> SearchArchiveBy(long creditAccountId)
        {
            Workpacket result = null;

            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            try
            {
                result = await _workpacketSearchService.SearchArchiveByAccountAsync(p360loginResponse.SessionTokenHeader, creditAccountId);
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }

            return result;
        }
    }
}
