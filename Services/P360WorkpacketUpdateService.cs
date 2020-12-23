using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using CreditOne.P360FormSubmissionService.Infra;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.Extensions.Options;
using P360WebReference;

namespace CreditOne.P360FormSubmissionService.Services
{
    public class P360WorkpacketUpdateService : P360Service, IWorkpacketUpdateService
    {
        private readonly IWorkpacketUpdateDomainService _workpacketUpdateDomainService;
        private readonly IWorkpacketSearchService _workpacketSearchService;
        private readonly IOptionsMonitor<P360ServiceData> _p360ServiceData;
        private readonly IMapper _mapper;

        public P360WorkpacketUpdateService(
            IWorkpacketUpdateDomainService workpacketUpdateDomainService,
            IWorkpacketSearchService workpacketSearchService,
            IOptionsMonitor<P360ServiceData> p360ServiceData,
            IMapper mapper,
            IOptionsMonitor<P360LoginData> p360LoginData) : base(p360LoginData.CurrentValue)
        {
            this._workpacketUpdateDomainService = workpacketUpdateDomainService;
            this._workpacketSearchService = workpacketSearchService;
            this._p360ServiceData = p360ServiceData;
            this._mapper = mapper;
        }

        /// <summary>
        /// Creates new workpacket based on existing workpacket with property WEB_ID equals to <paramref name="workpacketId"/>
        /// if one exists; 
        /// The new workpacket will have all the same attributes as base attributes with whatever changes are in <paramref name="request"/>
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="workpacketId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Workpacket> UpdateFromWorkpacketAsync(SessionTokenHeader sessionTokenHeader, string workpacketId, BaseForm request)
        {
            // search for existing workpacket to update from
            var workpacket = await _workpacketSearchService.SearchAsync(sessionTokenHeader, workpacketId);
            if(workpacket == null)
            {
                workpacket = await _workpacketSearchService.SearchArchiveAsync(sessionTokenHeader, (request.FormName, workpacketId));
                if(workpacket == null)
                    throw new ArgumentException($"{nameof(workpacketId)} could not be found.");
            }

            // map to p360 request
            WorkpacketUpdate[] workpacketUpdates = null;
            var dataUpdates = _workpacketUpdateDomainService.CreateFromData(request);
            
            if(request.Files != null && request.Files.Count > 0)
            {
                var fileUpdates = _workpacketUpdateDomainService.CreateFromFiles(new Files(request.Files));
                workpacketUpdates = new WorkpacketUpdate[fileUpdates.Length + 1];
                fileUpdates.CopyTo(workpacketUpdates, 0);
                workpacketUpdates[workpacketUpdates.Length - 1] = dataUpdates;
            }
            else
            {
                workpacketUpdates = new WorkpacketUpdate[] { dataUpdates };
            }

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

        /// <summary>
        /// Performs in-place update of an existing workpacket with WEB_ID property equals to <paramref name="workpacketId"/>
        /// if workpacket exists
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="workpacketId"></param>
        /// <param name="request">Changes/Updates to perform to workpacket</param>
        /// <returns></returns>
        public async Task<AttributeValidationError[]> UpdateWorkpacketAsync(SessionTokenHeader sessionTokenHeader, string workpacketId, WorkpacketBaseRequest request)
        {
            UpdateAndForwardWorkpacketResponse result = null;

            List<WorkpacketUpdate> workpacketUpdates = new List<WorkpacketUpdate>();

            // add data
            workpacketUpdates.Add(
                new SetPropertiesUpdate
                {
                    Properties = request.FormData.GetType().GetProperties().Select(p => new Property { Name = p.Name, Value = p.GetValue(request.FormData) }).ToArray(),
                    Target = PropertyUpdateTarget.FolderAttributes
                });
            var updateWorkpacketResponse = await Service.UpdateWorkpacketAsync(sessionTokenHeader, workpacketId, workpacketUpdates.ToArray());
            result = await Service.UpdateAndForwardWorkpacketAsync(sessionTokenHeader, workpacketId, workpacketUpdates.ToArray(), ValidationOption.ValidAttributes);

            return result?.UpdateAndForwardWorkpacketResult;
        }

        
    }
}