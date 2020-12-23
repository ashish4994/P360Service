using CreditOne.P360FormService.Models.Requests;
using P360WebReference;

namespace CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts
{
    public interface IFileToWorkpacketUpdateStrategy
    {
        WorkpacketUpdate GetWorkpacketUpdate();
    }
}
