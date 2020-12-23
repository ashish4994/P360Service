using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormService.Models;
using Microsoft.AspNetCore.Http;
using P360WebReference;
using System.Collections.Generic;
using System.Threading.Tasks;
using Workpacket = CreditOne.P360FormService.Models.Responses.Workpacket;
using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormService.Models.Responses;

namespace CreditOne.P360FormSubmissionService.Services.Contracts
{
    public interface IWorkpacketCreationService
    {
        
        Task<Workpacket> CreateFromWorkpacketAsync(SessionTokenHeader sessionTokenHeader, string workpacketId, Workpacket workpacket, UpdateRequest request);
        Task<Workpacket> CreateAsync(SessionTokenHeader sessionTokenHeader, BaseForm request);
    }
}
