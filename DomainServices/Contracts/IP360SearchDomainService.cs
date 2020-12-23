using P360WebReference;
using System.Collections.Generic;

namespace CreditOne.P360FormSubmissionService.DomainServices.Contracts
{
    public interface IP360SearchDomainService
    {
        SearchSettings CreateSearchCriteria(Dictionary<string, string> searchCriteria);

    }
}
