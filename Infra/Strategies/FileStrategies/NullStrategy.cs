using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;
using P360WebReference;

namespace CreditOne.P360FormSubmissionService.Infra.Strategies.FileStrategies
{
    public class NullStrategy : IFileToWorkpacketUpdateStrategy
    {
        public WorkpacketUpdate GetWorkpacketUpdate()
        {
            return null;
        }
    }
}
