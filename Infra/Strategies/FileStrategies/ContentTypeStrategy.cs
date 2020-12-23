using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;
using P360WebReference;
using System;

namespace CreditOne.P360FormSubmissionService.Infra.Strategies.FileStrategies
{
    public class ContentTypeStrategy : IFileToWorkpacketUpdateStrategy
    {
        private readonly File _file;

        public ContentTypeStrategy(File file)
        {
            this._file = file;
        }

        public WorkpacketUpdate GetWorkpacketUpdate()
        {
            WorkpacketUpdate result = null;
            var fileBytes = _file.InputStream;

            if (IsImage(this._file))
            {
                result = new CreateImageDocumentUpdate()
                {
                    FolderItemIndex = -1,
                    ImageData = fileBytes
                };
            }
            else if (IsText(this._file))
            {
                result = new CreateDesktopDocumentUpdate()
                {
                    DesktopFileData = fileBytes,
                    DocumentExtension = "text",
                    FolderItemIndex = -1
                };
            }
            else
            {
                result = new CreateDesktopDocumentUpdate()
                {
                    DesktopFileData = fileBytes,
                    DocumentExtension = _file.ContentType.Substring(_file.ContentType.IndexOf('/') + 1),
                    FolderItemIndex = -1
                };
            }
            return result;
        }


        public static bool IsImage(File file)
        {
            if (file == null || file.ContentType == null)
                throw new ArgumentNullException($"{nameof(file)} and {nameof(file.ContentType)} cannot be null");

            return file.ContentType.ToLower().Contains("image") || file.ContentType.ToLower().Contains("img");
        }

        public static bool IsText(File file)
        {
            if (file == null || file.ContentType == null)
                throw new ArgumentNullException($"{nameof(file)} and {nameof(file.ContentType)} cannot be null");

            return file.ContentType.ToLower().Contains("text") || file.ContentType.ToLower().Contains("txt");
        }

    }
}
