using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Filters;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Controllers
{
    [ApiController]
    [Route("api/create")]
    public class WorkpacketCreationController : Controller
    {
        private readonly IP360AuthenticationService _p360AuthenticationService;
        private readonly IWorkpacketCreationService _workpacketCreationService;
        private readonly IOptionsMonitor<P360LoginData> _p360LoginData;

        public WorkpacketCreationController(
            IWorkpacketCreationService workpacketCreationService, 
            IP360AuthenticationService p360AuthenticationService, 
            IOptionsMonitor<P360LoginData> p360LoginData)
        {
            this._workpacketCreationService = workpacketCreationService;
            this._p360AuthenticationService = p360AuthenticationService;
            this._p360LoginData = p360LoginData;
        }

        /// <summary>
        /// Creates a workpacket provided form data: <paramref name="workpacketCreateRequest"/>
        /// </summary>
        /// <param name="workpacketCreateRequest"></param>
        /// <returns></returns>
        [WorkpacketRequestValidationFilter(ModelName = "workpacketCreateRequest")]
        [HttpPost("workpacket")]
        public async Task<ActionResult<Workpacket>> CreateAsync([FromBody]BaseForm workpacketCreateRequest)
        {
            Workpacket result = null;

            var p360loginResponse = await _p360AuthenticationService.LoginAsync(_p360LoginData.CurrentValue);
            
            try
            {
                result = await _workpacketCreationService.CreateAsync(p360loginResponse.SessionTokenHeader, workpacketCreateRequest);
            }
            finally
            {
                var p360logoutResponse = await _p360AuthenticationService.LogoutAsync(p360loginResponse.SessionTokenHeader, _p360LoginData.CurrentValue);
            }

            return result;
        }

        byte[] FileToBytes(IFormFile file)
        {
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                    return fileBytes;
                }
            }
            return null;
        }
    }
}
