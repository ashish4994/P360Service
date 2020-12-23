using CreditOne.Core.Logger.Infra;
using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Infra.Factories;
using System.Collections.Generic;
using System.Linq;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class FolderItemExtensions
    {

        public static IEnumerable<File> ToFiles(this FolderItem folderItem)
        {
            var extensionStrategy = new FileExtensionExtractStrategyFactory()
                .GetStrategy(folderItem.Document);

            if (folderItem.Document is DesktopDocument desktopDoc)
            {
                return new List<File>()
                {
                    new File
                    {
                        InputStream = desktopDoc.DesktopFileData,
                        ContentType = desktopDoc.DocumentExtension,
                        ContentLength = (int)folderItem.Document.Size,
                        FileName = $"{folderItem.Description}.{extensionStrategy.GetExtension()}"
                    }
                };
            }
            else if (folderItem.Document is ImageDocument imageDoc)
            {
                return imageDoc.Pages.Select(p =>
                    new File
                    {
                        InputStream = p.ImageData,
                        FileName = $"{p.ImageFileName}.{extensionStrategy.GetExtension()}",
                        ContentLength = p.ImageData.Length,
                        ContentType = p.DocumentExtension
                    }
                );
            }
            else
            {
                Log.Warning($"Calling function with unsupported document type for folder item, {nameof(folderItem)}.");
                return new List<File>();
            }


        }
    }
}
