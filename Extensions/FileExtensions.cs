using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra.Strategies.FileFactories;
using P360WebReference;
using System;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class FileExtensions
    {

        public static WorkpacketUpdate ToWorkpacketUpdate(this File formFile, IFileToWorkpacketUpdateStrategyFactory strategyFactory)
        {
            if (formFile == null)
                throw new ArgumentNullException($"{nameof(formFile)} in {nameof(ToWorkpacketUpdate)} cannot be null.");

            var strategy = strategyFactory.GetStrategy(formFile);
            return strategy.GetWorkpacketUpdate();
        }
    }
}
