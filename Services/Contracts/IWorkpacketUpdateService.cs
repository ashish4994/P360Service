using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormService.Models.Responses;
using P360WebReference;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Services.Contracts
{
    public interface IWorkpacketUpdateService
    {
        Task<AttributeValidationError[]> UpdateWorkpacketAsync(SessionTokenHeader sessionTokenHeader, string workpacketId, WorkpacketBaseRequest workpacketUpdateRequest);
    }
}
