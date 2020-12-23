using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;
using CreditOne.P360FormSubmissionService.Infra.Strategies.FileExtensionExtractStrategies;
using CreditOne.P360FormSubmissionService.Infra.Strategies.ImageExtensionExtractStrategies;
using System;

namespace CreditOne.P360FormSubmissionService.Infra.Factories
{
    public class FileExtensionExtractStrategyFactory
    {


        public IFileExtensionExtractStrategy GetStrategy(Document document)
        {
            if(document is DesktopDocument desktopDoc)
            {
                return new DocumentExtensionExtractStrategy(desktopDoc);
            }
            if (document is ImageDocument imageDoc)
            {
                return new ByteExtensionExtractStrategy(imageDoc);
            }
            throw new NotSupportedException($"[Error@{nameof(GetStrategy)}]: Cannot get strategy for document of type {document.GetType().ToString()}");
        }
    }
}
