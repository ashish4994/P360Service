using AutoMapper;
using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using CreditOne.P360FormSubmissionService.Extensions;
using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.Extensions.Options;
using P360WebReference;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workpacket = P360WebReference.Workpacket;
using FormServiceModels = CreditOne.P360FormService.Models;
using CreditOne.P360FormSubmissionService.Exceptions;
using CreditOne.P360FormService.Models.Responses;

namespace CreditOne.P360FormSubmissionService.Services
{
    public class P360WorkpacketSearchService : P360Service, IWorkpacketSearchService
    {
        private readonly IMapper _mapper;
        private readonly IP360SearchDomainService _p360SearchDomainService;
        private readonly IOptionsMonitor<P360ServiceData> _p360ServiceData;
        private readonly IFormDataDomainService _formDataDomainService;
        private readonly IArchiveDomainService _archiveDomainService;

        public P360WorkpacketSearchService(
            IMapper mapper, 
            IOptionsMonitor<P360ServiceData> p360ServiceData, 
            IP360SearchDomainService p360SearchDomainService, 
            IFormDataDomainService formDataDomainService,
            IArchiveDomainService archiveDomainService,
            IOptionsMonitor<P360LoginData> p360LoginData) : base(p360LoginData.CurrentValue)
        {
            this._mapper = mapper;
            this._p360ServiceData = p360ServiceData;
            this._p360SearchDomainService = p360SearchDomainService;
            this._formDataDomainService = formDataDomainService;
            this._archiveDomainService = archiveDomainService;
        }

        /// <summary>
        /// Searches P360 worklists for workpacket with property ACCOUNT_NUMBER equals to <paramref name="account"/>
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<List<P360FormService.Models.Responses.Workpacket>> SearchByAccountAsync(SessionTokenHeader sessionTokenHeader, long account)
        {
            var searchSettings = new SearchSettings
            {
                SkipCount = 0,
                Limit = 100,
                FullTextScope = FullTextScope.None,
                Settings = new SearchSetting[]
                {
                    new SearchSetting
                    {
                        FieldName = "ACCOUNT_NUMBER",
                        Condition = $"={account}"
                    }
                }
            };

            // get search results
            List<RecordSet> recordSets = new List<RecordSet>();
            var searchWorklistResponses = new ConcurrentBag<RecordSet>();
            var worklists = await Service.GetAvailableWorklistNamesAsync(sessionTokenHeader);

            var searchTasks = worklists.GetAvailableWorklistNamesResult.Select(async worklist =>
            {
                var searchResult = await Service.SearchWorklistAsync(sessionTokenHeader, worklist.Name, searchSettings);
                if (searchResult?.SearchWorklistResult?.Rows != null && searchResult.SearchWorklistResult.Rows.Count() > 0)
                {
                    searchWorklistResponses.Add(searchResult.SearchWorklistResult);
                }
            });
            await Task.WhenAll(searchTasks);

            // get workpackets from search results
            var workpackets = new List<CreditOne.P360FormService.Models.Responses.Workpacket>();
            foreach (var worklistRow in searchWorklistResponses.SelectMany(swr => swr.Rows))
            {
                var wp = await Service.OpenWorkpacketByIdAsync(sessionTokenHeader, worklistRow.RowId, WorklistOpenMode.ReadWrite, true);

                if (wp != null)
                {
                    workpackets.Add(_mapper.Map<CreditOne.P360FormService.Models.Responses.Workpacket>(wp.OpenWorkpacketByIdResult));
                }
            }

            return workpackets;
        }

        /// <summary>
        /// Searches P360 worklists for workpacket with WEB_ID property equals to <paramref name="workpacketId"/>
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="workpacketId"></param>
        /// <returns></returns>
        public async Task<P360FormService.Models.Responses.Workpacket> SearchAsync(SessionTokenHeader sessionTokenHeader, string workpacketId)
        {
            P360FormService.Models.Responses.Workpacket result = null;
            var searchSettings = _p360SearchDomainService.CreateSearchCriteria(new Dictionary<string, string> { { "DOCUMENT_NAME", $"={workpacketId}" } });
            
            // search
            var searchWorklistResponses = new List<RecordSet>();
            var worklists = await Service.GetAvailableWorklistNamesAsync(sessionTokenHeader);
            foreach (var worklist in worklists.GetAvailableWorklistNamesResult)
            {
                var searchResult = await Service.SearchWorklistAsync(sessionTokenHeader, worklist.Name, searchSettings);
                if (searchResult?.SearchWorklistResult?.Rows != null && searchResult.SearchWorklistResult.Rows.Count() > 0)
                {
                    // filter to account for P360 bug whereas if a workpacket is searched by a field it doesn't have it will be a valid match. 
                    if (searchResult.SearchWorklistResult.Rows.SelectMany(r => r.Data).Any(d => d.ToString() == workpacketId))
                    {
                        searchWorklistResponses.Add(searchResult.SearchWorklistResult);
                    }
                }
            }

            if (searchWorklistResponses == null || searchWorklistResponses.Count == 0)
                throw new NotFoundException($"Search for {workpacketId} returned no results.", workpacketId);

            // open
            var workPackets = new ConcurrentBag<Workpacket>();
            var tasks = searchWorklistResponses.SelectMany(swr => swr.Rows).Select(async worklistRow =>
            {
                var wp = await Service.OpenWorkpacketByIdAsync(sessionTokenHeader, worklistRow.RowId, WorklistOpenMode.ReadOnly, true);
                if (wp != null)
                    workPackets.Add(wp.OpenWorkpacketByIdResult);
            });
            await Task.WhenAll(tasks);

            // take most recent one
            var mostRecentWorkpacket = workPackets.OrderByDescending(w => w.GetProperty<DateTime>("SCANNED_DATE")).FirstOrDefault();
            if(mostRecentWorkpacket != null)
                result = _mapper.Map<P360FormService.Models.Responses.Workpacket>(mostRecentWorkpacket);
           
            return result;
        }

        /// <summary>
        /// Searches P360 archives for workpackets with ACCOUNT_NUMBER property equals to <paramref name="creditAccountId"/>
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="creditAccountId"></param>
        /// <returns></returns>
        public async Task<P360FormService.Models.Responses.Workpacket> SearchArchiveByAccountAsync(SessionTokenHeader sessionTokenHeader, long creditAccountId)
        {
            var searchSettings = _p360SearchDomainService.CreateSearchCriteria(new Dictionary<string, string> { { "ACCOUNT_NUMBER", $"={creditAccountId}" } });
            var searchResult = await Service.SearchCatalogAsync(sessionTokenHeader, "TS_A_GLOBAL_SEARCH_C", searchSettings, SearchScope.DocumentsAndData);
            if (searchResult.SearchCatalogResult.Rows.Count() > 0)
            {
                var catalogItem = await Service.GetCatalogItemAsync(sessionTokenHeader, searchResult.SearchCatalogResult.Rows[0].RowId, true);
                return _mapper.Map<P360FormService.Models.Responses.Workpacket>(catalogItem.GetCatalogItemResult);
            }

            return null;
        }

        /// <summary>
        /// Search both archive and worklists for workpacket
        /// </summary>
        /// <param name="sessionTokenHeader">sessionTokenHeader used for login</param>
        /// <param name="workpacketId">WEB_ID property of workpacket</param>
        /// <returns></returns>
        public async Task<P360FormService.Models.Responses.Workpacket> SearchAllAsync(SessionTokenHeader sessionTokenHeader, (string formName, string workpacketId) request)
        {
            // search and open
            var worklistSearchResultTask = SearchAsync(sessionTokenHeader, request.workpacketId);
            var archiveSearchResultTask = SearchArchiveAsync(sessionTokenHeader, (request.formName, request.workpacketId));

            FormServiceModels.Responses.Workpacket worklistSearchResult = null;
            FormServiceModels.Responses.Workpacket archiveSearchResult = null;

            (string searchWorkListError, string searchArchiveError) searchError = (string.Empty, string.Empty);
            try
            {
                worklistSearchResult = await worklistSearchResultTask;
            }
            catch (Exception e)
            { /* do nothing, handled below */
                searchError.searchWorkListError = e.Message ?? string.Empty;
            }

            try
            {
                archiveSearchResult = await archiveSearchResultTask;
            }
            catch(Exception e)
            {
                searchError.searchArchiveError = e.Message ?? string.Empty;
            }

            if (worklistSearchResult == null && archiveSearchResult == null)
                throw new NotFoundException(
                    $"[{nameof(SearchAllAsync)}] No result found in Archive or Worklists for WEB_ID={request.workpacketId}, Error: {searchError.searchWorkListError}.{searchError.searchArchiveError}", 
                    request.workpacketId);

            // sort
            var worklistResultScannedDateStr = worklistSearchResult?.WorklistAttributes.Properties.Where(p => p.Name == "SCANNED_DATE")
                .Select(p => p.Value)
                .FirstOrDefault()
                ?.ToString();
            var archiveResultScannedDateStr = archiveSearchResult?.WorklistAttributes.Properties.Where(p => p.Name == "SCANNED_DATE")
                .Select(p => p.Value)
                .FirstOrDefault()
                ?.ToString();

            if (archiveResultScannedDateStr == null)
                return worklistSearchResult;
            if (worklistResultScannedDateStr == null)
                return archiveSearchResult;
            return Convert.ToDateTime(worklistResultScannedDateStr) >= Convert.ToDateTime(archiveResultScannedDateStr)
                ? worklistSearchResult
                : archiveSearchResult;
        }

        /// <summary>
        /// Search archives for workpacket with WEB_ID equal to <paramref name="workpacketId"/>
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="workpacketId"></param>
        /// <returns></returns>
        public async Task<P360FormService.Models.Responses.Workpacket> SearchArchiveAsync(SessionTokenHeader sessionTokenHeader, (string formName, string workpacketId) request)
        {
            P360FormService.Models.Responses.Workpacket result = null;

            string archiveName = _formDataDomainService.FormNameToArchive(request.formName);

            IEnumerable<CatalogItem> catalogItems = null;

            if (_p360ServiceData.CurrentValue.SearchAllArchives)
            {
                catalogItems = await SearchArchiveForCatalogItemsAsync(sessionTokenHeader, (archiveName, request.workpacketId));

            }
            else
            {
                catalogItems = await SearchArchiveForCatalogItemsAsync(sessionTokenHeader, (archiveName, request.workpacketId));

            }

            if (catalogItems != null && catalogItems.Count() > 0)
            {
                result = _archiveDomainService.ToWorkpacket(catalogItems);
            }
            return result;
        }

        /// <summary>
        /// Searches archive for workpacket matching WEB_ID to <param name="workpacketId"> 
        /// and returns the one with most recent SCANNED_DATE if found
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="workpacketId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<CatalogItem>> SearchArchiveForCatalogItemsAsync(SessionTokenHeader sessionTokenHeader, (string archiveName, string workpacketId) request)
        {
            var searchSettings = _p360SearchDomainService.CreateSearchCriteria(new Dictionary<string, string> { { "DOCUMENT_NAME", $"={request.workpacketId}" } });
            
            var searchResult = await Service.SearchCatalogAsync(sessionTokenHeader, request.archiveName, searchSettings, SearchScope.DocumentsAndData);
            var matchedResults = searchResult.SearchCatalogResult.Rows.Where(r => r.Data.Any(d => d.ToString() == request.workpacketId));
            IEnumerable<Task<GetCatalogItemResponse>> catalogItemTasks = matchedResults
                .Select(r => Service.GetCatalogItemAsync(sessionTokenHeader, r.RowId, true));
            
            var catalogItems = (await Task.WhenAll(catalogItemTasks))
                .Select(cir => cir.GetCatalogItemResult);
            return catalogItems;
        }

        /// <summary>
        /// Searches all archives for workpacket matching Document Name to <param name="workpacketId"> 
        /// and returns the one with most recent SCANNED_DATE if found
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="workpacketId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<CatalogItem>> SearchAllArchivesAsync(SessionTokenHeader sessionTokenHeader, (string archiveName, string workpacketId) request)
        {
            var searchSettings = _p360SearchDomainService.CreateSearchCriteria(new Dictionary<string, string> { { "DOCUMENT_NAME", $"={request.workpacketId}" } });
            var searchResult = await Service.SearchCatalogAsync(sessionTokenHeader, _p360ServiceData.CurrentValue.GlobalArchiveName, searchSettings, SearchScope.DocumentsAndData);
            var matchedResults = searchResult.SearchCatalogResult.Rows.Where(r => r.Data.Any(d => d.ToString() == request.workpacketId));
            IEnumerable<Task<GetCatalogItemResponse>> catalogItemTasks = matchedResults
                .Select(r => Service.GetCatalogItemAsync(sessionTokenHeader, r.RowId, true));

            var catalogItems = (await Task.WhenAll(catalogItemTasks))
                .Select(cir => cir.GetCatalogItemResult);
            return catalogItems;
        }

        /// <summary>
        /// Seaarch both worklist and archive for clients. 
        /// </summary>
        /// <param name="sessionTokenHeader"></param>
        /// <param name="workpacketId"></param>
        /// <returns></returns>
        public async Task<WorkpacketSearchResponse> SearchForClient(SessionTokenHeader sessionTokenHeader, string workpacketId)
        {
            //A dummy formname should be passed to Search method. So NAME_CHANGE value is passed.
            var workpacket = await SearchAllAsync(sessionTokenHeader, ("NAME_CHANGE", workpacketId));

            if (workpacket == null)
            {
                return null;
            }

            var searchResponse = new WorkpacketSearchResponse
            {
                TrackingId = workpacket.TrackingId,
                Id = workpacket.Id,
                Properties = workpacket.Folder.Attributes.Properties,
                Documents = workpacket.Folder.FolderItems.Select(x => x.Document).ToList()
            };

            return searchResponse;
        }
    }
}
