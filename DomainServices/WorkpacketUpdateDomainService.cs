using AutoMapper;
using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using CreditOne.P360FormSubmissionService.Extensions;
using CreditOne.P360FormSubmissionService.Infra;
using CreditOne.P360FormSubmissionService.Infra.Strategies.FileFactories;
using P360WebReference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CreditOne.P360FormSubmissionService.DomainServices
{
    public class WorkpacketUpdateDomainService : IWorkpacketUpdateDomainService
    {
        private readonly IFormDataDomainService _formDataDomainService;
        private readonly IMapper _mapper;
        private readonly IFileToWorkpacketUpdateStrategyFactory _fileToWorkpacketUpdateStrategyFactory;

        public WorkpacketUpdateDomainService(
            IFormDataDomainService formDataDomainService,
            IMapper mapper,
            IFileToWorkpacketUpdateStrategyFactory fileToWorkpacketUpdateStrategyFactory)
        {
            this._formDataDomainService = formDataDomainService;
            this._mapper = mapper;
            this._fileToWorkpacketUpdateStrategyFactory = fileToWorkpacketUpdateStrategyFactory;
        }

        public SetPropertiesUpdate CreateFromData(BaseForm formData)
        {
            var receivedDate = new Property { Name = "RECEIVED_DATE", Value = DateTime.UtcNow.ToString() };
            var result =
                new SetPropertiesUpdate
                {
                    Properties = formData.GetType().GetProperties()
                    .Where(p => (p.GetValue(formData) as string) != null)
                    .Select(
                        p =>
                        {
                            if (p.Name == nameof(BaseForm.FormName))
                            {
                                return new Property
                                {
                                    Name = "CS_DOC_TYPE",
                                    Value = _formDataDomainService.FormNameToWorklist((p.GetValue(formData) as string))
                                };
                            }
                            else
                            {
                                return new Property
                                {
                                    Name = p.Name,
                                    Value = p.GetValue(formData)
                                };
                            }
                        })
                        .Concat(new Property[] { receivedDate })
                        .Concat(formData?.Files?.Select((f, i) => new Property { Name = $"DOC_{i + 1}", Value = f.FileName }) ?? new Property[0])
                        .ToArray(),
                    Target = PropertyUpdateTarget.FolderAttributes
                };

            if (formData?.NotesFile != null)
            {
                result.Properties = result.Properties
                    .Concat(new Property[] { new Property { Name = $"DOC_{(formData?.Files?.Count == null ? 1 : formData.Files.Count + 1)}", Value = formData.NotesFile.FileName } }).ToArray();
            }

            return result;
        }

        public WorkpacketUpdate[] CreateFromFiles(Files files)
        {
            WorkpacketUpdate[] workpacketUpdates = files.ToWorkpacketUpdates(_fileToWorkpacketUpdateStrategyFactory);
            return workpacketUpdates;
        }

        /// <summary>
        /// Create SetPropertiesUpdate from new update request and existing workpacket properties
        /// New request will overwrite existing workpacket properties with exception of web_id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="workpacket"></param>
        /// <returns></returns>
        public SetPropertiesUpdate CreateFromDataAndWorkpacket(string workpacketId,UpdateRequest request, P360FormService.Models.Responses.Workpacket workpacket)
        {
       
            var document_name = new Property { Name = "DOCUMENT_NAME", Value = workpacketId.ToString() };

            if (workpacket == null)
                return null;

            var workpacketProperties = workpacket.Folder.Attributes.Properties
                    .Select(p =>
                    {
                        var property = _mapper.Map<Property>(p);
                        var requestProperty = request.DataUpdates.FirstOrDefault(r => r.FieldName == property.Name);
                        if (requestProperty != null)
                            property.Value = requestProperty.FieldValue;
                        return property;
                    });

            var requestProperties = request.DataUpdates
                .Where(fd => !workpacket.Folder.Attributes.Properties.Any(p => p.Name == fd.FieldName))
                .Select(fd => new Property { Name = fd.FieldName, Value = fd.FieldValue })
                .Concat(new Property[] { document_name });

            // handle request data properties
            IEnumerable<Property> properties = workpacketProperties;
            if (requestProperties != null && requestProperties.Count() > 0)
                properties = workpacketProperties.Concat(requestProperties);

            // handle request files' properties
            var requestFiles = request.GetFiles().AllFiles;
            int folderItems = workpacket?.Folder?.FolderItems == null ? 1 : workpacket.Folder.FolderItems.Count() + 1;
            if (requestFiles.Count > 0)
            {
                properties = properties.Concat(
                        requestFiles.Select((rf, i) => new Property
                        {
                            Name = $"DOC_{folderItems + i}",
                            Value = rf.FileName
                        }
                    ));
            }

            SetPropertiesUpdate result = new SetPropertiesUpdate
            {
                Properties = properties.ToArray(),
                Target = PropertyUpdateTarget.FolderAttributes
            };

            return result;
        }

        public SetPropertiesUpdate CreateFromWorkpacket(CreditOne.P360FormService.Models.Responses.Workpacket workpacket)
        {
            if (workpacket == null)
                return null;

            var wrokpacketProperties = workpacket.Folder.Attributes.Properties
                .Select(p => _mapper.Map<Property>(p))
                .ToArray();
            SetPropertiesUpdate result = new SetPropertiesUpdate
            {
                Properties = wrokpacketProperties,
                Target = PropertyUpdateTarget.FolderAttributes
            };

            return result;
        }




    }
}
