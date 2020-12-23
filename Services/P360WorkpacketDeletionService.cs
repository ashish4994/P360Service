using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.Extensions.Options;
using P360WebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workpacket = CreditOne.P360FormService.Models.Responses.Workpacket;

namespace CreditOne.P360FormSubmissionService.Services
{
    public class P360WorkpacketDeletionService :P360Service,IWorkpacketDeletionService
    {
        public P360WorkpacketDeletionService(IOptionsMonitor<P360LoginData> p360LoginData) : base(p360LoginData.CurrentValue)
        {

        }
        public async Task<bool> DeleteWorkPacketAsync(SessionTokenHeader sessionTokenHeader, string workpacketId)
        {
            try
            {
                var result = await Service.DeleteWorkpacketAsync(sessionTokenHeader, workpacketId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
