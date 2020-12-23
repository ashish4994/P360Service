using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;

namespace CreditOne.P360FormSubmissionService.Infra.Strategies.FileExtensionExtractStrategies
{
    public class DocumentExtensionExtractStrategy : IFileExtensionExtractStrategy
    {
        private readonly DesktopDocument _desktopDocument;

        public DocumentExtensionExtractStrategy(DesktopDocument desktopDocument)
        {
            this._desktopDocument = desktopDocument;
        }

        public string GetExtension()
        {
            return _desktopDocument.DocumentExtension;
        }
    }
}
