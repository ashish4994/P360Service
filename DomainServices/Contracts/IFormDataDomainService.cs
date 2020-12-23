using CreditOne.P360FormService.Models.Requests;
using P360WebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.DomainServices.Contracts
{
    public interface IFormDataDomainService
    {
        string FormNameToWorklist(string formName);
        string FormNameToArchive(string formName);
        
    }
}
