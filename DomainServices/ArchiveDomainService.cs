using AutoMapper;
using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using P360WebReference;
using System.Collections.Generic;
using System.Linq;

namespace CreditOne.P360FormSubmissionService.DomainServices
{
    public class ArchiveDomainService : IArchiveDomainService
    {
        private readonly IMapper _mapper;

        public ArchiveDomainService(IMapper mapper)
        {
            this._mapper = mapper;
        }


        public P360FormService.Models.Responses.Workpacket ToWorkpacket(IEnumerable<CatalogItem> catalogItems)
        {
            P360FormService.Models.Responses.Workpacket result = new P360FormService.Models.Responses.Workpacket();

            // FolderItems
            result.Folder.FolderItems = 
                catalogItems
                .Select(ci => new P360FormService.Models.Responses.FolderItem
                {
                    Document = _mapper.Map<P360FormService.Models.Responses.Document>(ci.Document)

                }).ToArray();

            // WorklistAttributes
            result.WorklistAttributes = _mapper.Map<P360FormService.Models.Responses.Attributes>( catalogItems.First().Attributes );
            result.Folder.Attributes = result.WorklistAttributes;

            return result;
        }
    }
}
