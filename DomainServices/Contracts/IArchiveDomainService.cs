using P360WebReference;
using System.Collections.Generic;

namespace CreditOne.P360FormSubmissionService.DomainServices.Contracts
{
    public interface IArchiveDomainService
    {
        P360FormService.Models.Responses.Workpacket ToWorkpacket(IEnumerable<CatalogItem> catalogItems);
    }
}
