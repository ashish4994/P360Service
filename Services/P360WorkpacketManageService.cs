using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using P360WebReference;
using System;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Services
{
    public class P360WorkpacketManageService : P360Service, IWorkpacketManageService
    {
        public P360WorkpacketManageService(P360LoginData p360LoginData) : base(p360LoginData)
        {

        }
        public async Task<bool> CloseWorkpacketAsync(SessionTokenHeader sessionTokenHeader, Workpacket workpacket)
        {
            await Service.CloseWorkpacketAsync(sessionTokenHeader, workpacket.Id, true);
            return true;
        }

    }
}
