using CreditOne.P360FormService.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Services.Contracts
{
    public interface IWorkpacketSearchService
    {
        Task<Workpacket> SearchAsync(P360WebReference.SessionTokenHeader sessionTokenHeader, string workpacketId);
        
        Task<List<Workpacket>> SearchByAccountAsync(P360WebReference.SessionTokenHeader sessionTokenHeader, long account);

        Task<Workpacket> SearchArchiveAsync(P360WebReference.SessionTokenHeader sessionTokenHeader, (string formName, string workpacketId) request);

        Task<Workpacket> SearchArchiveByAccountAsync(P360WebReference.SessionTokenHeader sessionTokenHeader, long account);

        /// <summary>
        /// This will search both worklists and archive (only if not found in worklists)
        /// </summary>
        /// <param name="sessionTokenHeader">p360 login sessionTokenHeader</param>
        /// <param name="workpacketId">workpacket id to search for</param>
        /// <returns></returns>
        Task<Workpacket> SearchAllAsync(P360WebReference.SessionTokenHeader sessionTokenHeader, (string formName, string workpacketId) request);

        Task<WorkpacketSearchResponse> SearchForClient(P360WebReference.SessionTokenHeader sessionTokenHeader, string workpacketId);
    }
}
