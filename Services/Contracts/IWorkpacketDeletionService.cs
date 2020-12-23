using CreditOne.P360FormService.Models.Responses;
using P360WebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Services.Contracts
{
    public interface IWorkpacketDeletionService
    {
        Task<bool> DeleteWorkPacketAsync(SessionTokenHeader sessionTokenHeader, string workpacketId);
    }
}
