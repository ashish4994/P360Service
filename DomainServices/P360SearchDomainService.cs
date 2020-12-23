using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using P360WebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.DomainServices
{
    public class P360SearchDomainService : IP360SearchDomainService
    {
        public SearchSettings CreateSearchCriteria(Dictionary<string, string> searchCriteria)
        {
            return new SearchSettings
            {
                SkipCount = 0,
                Limit = 100,
                FullTextScope = FullTextScope.None,
                Settings = searchCriteria.Select(sc => new SearchSetting { FieldName = sc.Key, Condition = sc.Value }).ToArray()
            };

        }
    }
}
