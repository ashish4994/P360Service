using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using CreditOne.P360FormSubmissionService.Models;
using Microsoft.Extensions.Options;
using P360WebReference;
using System.Collections.Generic;
using System.Linq;

namespace CreditOne.P360FormSubmissionService.DomainServices
{
    public class FormDataDomainService : IFormDataDomainService
    {
        private readonly IOptionsMonitor<List<FormToQueue>> _formToQueueMappings;

        public FormDataDomainService(IOptionsMonitor<List<FormToQueue>> formToQueueMappings)
        {
            _formToQueueMappings = formToQueueMappings;
        }

        public string FormNameToWorklist(string formName)
        {
            return _formToQueueMappings.CurrentValue
                .Where(ftq => ftq.FormName == formName)
                ?.FirstOrDefault()
                ?.QueueName;
        }

        public string FormNameToArchive(string formName)
        {
            return _formToQueueMappings.CurrentValue
                .Where(ftq => ftq.FormName == formName)
                ?.FirstOrDefault()
                ?.ArchiveName;
        }

    }
}
