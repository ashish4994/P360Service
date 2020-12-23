using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;
using CreditOne.P360FormSubmissionService.Infra.Strategies.FileFactories;
using CreditOne.P360FormSubmissionService.Infra.Strategies.FileStrategies;
using System.Text.RegularExpressions;

namespace CreditOne.P360FormSubmissionService.Infra.Factories
{
    /// <summary>
    /// Decides which strategy to choose based on the FileName property of File
    /// </summary>
    public class FileNameFileToWorkpacketUpdateStrategyFactory : IFileToWorkpacketUpdateStrategyFactory
    {
        public static IFileToWorkpacketUpdateStrategy _strategy;

        public IFileToWorkpacketUpdateStrategy GetStrategy(File file)
        {
            Regex extensionRegex = new Regex(@"\.[A-z|0-9]+$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (extensionRegex.IsMatch(file.FileName))
            {
                return new FileNameStrategy(file);
            }
            else
            {
                return new ContentTypeStrategy(file);
            }
        }

    }
}
