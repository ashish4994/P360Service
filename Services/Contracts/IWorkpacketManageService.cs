using P360WebReference;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Services.Contracts
{
    public interface IWorkpacketManageService
    {
        //Task CloseWorkpacketAsync(SessionTokenHeader sessionTokenHeader, string webId);

        Task<bool> CloseWorkpacketAsync(SessionTokenHeader sessionTokenHeader, Workpacket workpacket);
    }
}
