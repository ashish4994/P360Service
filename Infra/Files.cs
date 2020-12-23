using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Extensions;
using CreditOne.P360FormSubmissionService.Infra.Strategies.FileFactories;
using P360WebReference;
using System.Collections.Generic;
using System.Linq;
using FormServiceModels = CreditOne.P360FormService.Models;

namespace CreditOne.P360FormSubmissionService.Infra
{
    /// <summary>
    /// File Sequence container
    /// </summary>
    public class Files
    {
        public List<File> AllFiles { get; } = new List<File>();

        public Files(IEnumerable<File> files)
        {
            AllFiles = files?.ToList() ?? new List<File>();
        }

        public Files(IEnumerable<FormServiceModels.Responses.FolderItem> folderItems)
        {
            if (folderItems != null && folderItems.Count() > 0)
            {
                AllFiles =
                    folderItems
                        .SelectMany(fi => fi.ToFiles())
                        .ToList();
            }
        }

        public Files Add(File file)
        {
            if (file != null)
                AllFiles.Add(file);
            return this;
        }

        public Files Add(Files files)
        {
            if (files.AllFiles != null && files.AllFiles.Count > 0)
                AllFiles.AddRange(files.AllFiles);
            return this;
        }

        public Files Add(FormServiceModels.Responses.FolderItem[] folderItems)
        {
            if (folderItems == null || folderItems.Count() == 0)
                return this;

            var filesToAdd = folderItems
                .SelectMany(fi => fi.ToFiles());
            
            Add(new Files(filesToAdd));

            return this;
        }

        public WorkpacketUpdate[] ToWorkpacketUpdates(IFileToWorkpacketUpdateStrategyFactory strategyFactory)
        {
            if (AllFiles.Count() == 0)
                return new WorkpacketUpdate[0];

            WorkpacketUpdate[] workpacketUpdates =
                AllFiles
                    .Select(f => f.ToWorkpacketUpdate(strategyFactory))
                    .ToArray();
            return workpacketUpdates;
        }
    }
}
