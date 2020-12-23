using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra;
using P360WebReference;

namespace CreditOne.P360FormSubmissionService.DomainServices.Contracts
{
    public interface IWorkpacketUpdateDomainService
    {
        SetPropertiesUpdate CreateFromData(BaseForm formData);

        SetPropertiesUpdate CreateFromDataAndWorkpacket(string workpacketId, UpdateRequest request, P360FormService.Models.Responses.Workpacket workpacket);

        WorkpacketUpdate[] CreateFromFiles(Files files);

        SetPropertiesUpdate CreateFromWorkpacket(CreditOne.P360FormService.Models.Responses.Workpacket workpacket);
    }
}
