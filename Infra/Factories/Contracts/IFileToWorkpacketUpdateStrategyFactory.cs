using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;

namespace CreditOne.P360FormSubmissionService.Infra.Strategies.FileFactories
{
    public interface IFileToWorkpacketUpdateStrategyFactory
    {
        IFileToWorkpacketUpdateStrategy GetStrategy(File file);
    }
}
