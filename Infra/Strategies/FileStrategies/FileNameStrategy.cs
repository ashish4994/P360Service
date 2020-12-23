using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;
using P360WebReference;
using System.Text.RegularExpressions;

namespace CreditOne.P360FormSubmissionService.Infra.Strategies.FileStrategies
{
    /// <summary>
    /// Creates WorkpacketUpdate from File 
    /// </summary>
    public class FileNameStrategy : IFileToWorkpacketUpdateStrategy
    {
        private readonly File _file;

        public FileNameStrategy(File file)
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
            else
            {
                result = new CreateDesktopDocumentUpdate()
                {
                    DesktopFileData = fileBytes,
                    DocumentExtension = _file.FileName.Substring(_file.FileName.LastIndexOf('.')+1),
                    FolderItemIndex = -1
                };
            }

            return result;
        }

        public static bool IsImage(File file)
        {
            Regex imageRegex = new Regex(@"\.(jpe?g|png|gif|bmp|tiff)$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return imageRegex.IsMatch(file.FileName);
        }
    }
}
