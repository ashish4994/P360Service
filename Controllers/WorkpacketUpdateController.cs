using System.Threading.Tasks;
using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CreditOne.P360FormSubmissionService.Controllers
{
    [ApiController]
    [Route("api/update")]
    public sealed class WorkpacketUpdateController : Controller
    {
        private readonly IWorkpacketSearchService _workpacketSearchService;
        private readonly IP360AuthenticationService _p360AuthenticationService;
        private readonly IOptionsMonitor<P360LoginData> _p360LoginData;
        private readonly IWorkpacketUpdateService _workpacketUpdateService;
        private readonly IWorkpacketCreationService _workpacketCreationService;

        public WorkpacketUpdateController(
            IWorkpacketSearchService workpacketSearchService,
            IP360AuthenticationService p360AuthenticationService,
            IOptionsMonitor<P360LoginData> p360LoginData,
            IWorkpacketUpdateService workpacketUpdateService,
            IWorkpacketCreationService workpacketCreationService
            )
        {
            this._workpacketSearchService = workpacketSearchService;
            this._workpacketUpdateService = workpacketUpdateService;
            this._p360AuthenticationService = p360AuthenticationService;
            this._workpacketCreationService = workpacketCreationService;
            this._p360LoginData = p360LoginData;
        }

        [HttpPut("from-workpacket/{workpacketId}")]
        public async Task<ActionResult<Workpacket>> UpdateFromWorkpacket(string workpacketId, [FromBody]UpdateRequest request)
        {
            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            Workpacket updatedWorkpacket;
            try
            {
                var workpacket = await _workpacketSearchService.SearchAllAsync(p360loginResponse.SessionTokenHeader, (request.FormName, workpacketId));
                updatedWorkpacket = await _workpacketCreationService.CreateFromWorkpacketAsync(p360loginResponse.SessionTokenHeader, workpacketId, workpacket, request);
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }

            return updatedWorkpacket;
        }

        [HttpPut("{workpacketId}")]
        public async Task<ActionResult<Workpacket>> UpdateWorkpacket(string workpacketId, [FromBody]CreateWPWithFormDataFilesRequest workpacketUpdateRequest)
        {
            Workpacket result = null;

            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);

            try
            {
                var workpacketToUpdate = await _workpacketSearchService.SearchAllAsync(p360loginResponse.SessionTokenHeader, (null,workpacketId));
                P360WebReference.AttributeValidationError[] updateResult = null;
                if (workpacketToUpdate != null)
                {
                    updateResult = await _workpacketUpdateService.UpdateWorkpacketAsync(p360loginResponse.SessionTokenHeader, workpacketToUpdate.Id, workpacketUpdateRequest);
                }
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }

            return result;
        }

       

    }
}
