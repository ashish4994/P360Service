using AutoMapper;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using P360WebReference;
using System.Linq;
using System.Threading.Tasks;
using Workpacket = CreditOne.P360FormService.Models.Responses.Workpacket;
using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using Microsoft.Extensions.Options;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Extensions;
using CreditOne.P360FormSubmissionService.Infra;
using System.IO;
using File = CreditOne.P360FormService.Models.Requests.File;

namespace CreditOne.P360FormSubmissionService.Services
{
    public class P360WorkpacketCreationService : P360Service, IWorkpacketCreationService
    {
        private readonly IWorkpacketUpdateService _workpacketUpdateService;
        private readonly IMapper _mapper;
        private readonly IWorkpacketUpdateDomainService _workpacketUpdateDomainService;
        private readonly IFormDataDomainService _formDataDomainService;
        private readonly IOptionsMonitor<P360ServiceData> _p360ServiceData;
        private readonly IOptionsMonitor<DummyFileInformation> _dummyFileInformation;

        public P360WorkpacketCreationService(
            IWorkpacketUpdateService workpacketUpdateService,
            IMapper mapper, 
            IWorkpacketUpdateDomainService workpacketUpdateDomainService,
            IFormDataDomainService formDataDomainService,
            IOptionsMonitor<P360ServiceData> p360ServiceData,
            IOptionsMonitor<P360LoginData> p360LoginData,
            IOptionsMonitor<DummyFileInformation> dummyFileInformation) : base(p360LoginData.CurrentValue)
        {
            _workpacketUpdateService = workpacketUpdateService;
            _mapper = mapper;
            _workpacketUpdateDomainService = workpacketUpdateDomainService;
            _formDataDomainService = formDataDomainService;
            _p360ServiceData = p360ServiceData;
            _dummyFileInformation = dummyFileInformation;
        }

        /// <summary>
        /// Creates a new workpacket using request and existing workpacket
        /// by updating existing workpacket with request into new workpacket 
        /// </summary>
        /// <param name="sessionTokenHeader">p360 login session</param>
        /// <param name="workpacket">model workpacket retrieved from p360</param>
        /// <param name="request">request with updates to apply to workpacket</param>
        /// <returns></returns>
        public async Task<Workpacket> CreateFromWorkpacketAsync(SessionTokenHeader sessionTokenHeader, string workpacketId, Workpacket workpacket, UpdateRequest request)
        {
         
            // Create merged request from workpacket and request
            var fileUpdates = _workpacketUpdateDomainService.CreateFromFiles(new Files(workpacket?.Folder?.FolderItems).Add(request.GetFiles()));
            var dataUpdates = _workpacketUpdateDomainService.CreateFromDataAndWorkpacket(workpacketId, request, workpacket);

            // create p360 request
            WorkpacketUpdate[] workpacketUpdates = new WorkpacketUpdate[fileUpdates.Length + 1];
            fileUpdates.CopyTo(workpacketUpdates, 0);
            workpacketUpdates[workpacketUpdates.Length - 1] = dataUpdates;

            // execute request
            var result = await Service.CreateWorkpacketAsync(
                sessionTokenHeader,
                _p360ServiceData.CurrentValue.ProcessName,
                _p360ServiceData.CurrentValue.NodeName,
                workpacketUpdates
                );
            await Service.ForwardWorkpacketAsync(sessionTokenHeader, result.CreateWorkpacketResult.Id);

            // map
            var response = _mapper.Map<Workpacket>(result.CreateWorkpacketResult);

            return response;
        }

        public async Task<Workpacket> CreateAsync(SessionTokenHeader sessionTokenHeader, BaseForm request)
        {
            //-Adds a dummy file if wp doesn't contain file attachments
            if (!request.Files.Any() && _dummyFileInformation.CurrentValue.EnableDummyFile)
            {
                request.Files.Add(CreateDummyFile(_dummyFileInformation.CurrentValue.FileName));
            }
            // convert to p360 input
            var fileUpdates = _workpacketUpdateDomainService.CreateFromFiles(request.GetFiles());
            var dataUpdates = _workpacketUpdateDomainService.CreateFromData(request);

            // create p360 request
            WorkpacketUpdate[] workpacketUpdates = new WorkpacketUpdate[fileUpdates.Length + 1];
            fileUpdates.CopyTo(workpacketUpdates, 0);
            workpacketUpdates[workpacketUpdates.Length - 1] = dataUpdates;

            // execute request
            var result = await Service.CreateWorkpacketAsync(
                sessionTokenHeader,
                _p360ServiceData.CurrentValue.ProcessName,
                _p360ServiceData.CurrentValue.NodeName,
                workpacketUpdates
                );

            await Service.ForwardWorkpacketAsync(sessionTokenHeader, result.CreateWorkpacketResult.Id);

            // map
            var response = _mapper.Map<Workpacket>(result.CreateWorkpacketResult);

            if (response?.Folder?.Attributes?.Properties?.FirstOrDefault(x => x.Name == "DOCUMENT_NAME") != null)
            {
                response.WorkpacketId = response?.Folder?.Attributes?.Properties?.FirstOrDefault(x => x.Name == "DOCUMENT_NAME").Value.ToString();
            }
            else
            {
                response.WorkpacketId = response?.TrackingId.ToString();
            }
            
            return response;
        }

        public File CreateDummyFile(string fileName)
        {
            byte[] fileBytes = null;
            using (var ms = new MemoryStream())
            {
                TextWriter textWriter = new StreamWriter(ms);
                textWriter.WriteLine(string.Empty);

                textWriter.Flush();
                ms.Position = 0;
                fileBytes = ms.ToArray();
            }

            return new File
            {
                ContentLength = fileBytes.Length,
                InputStream = fileBytes,
                FileName = fileName,
                ContentType = "text/plain"
            };
        }
    }
}
