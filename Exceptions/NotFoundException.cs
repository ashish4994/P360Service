using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Exceptions
{
    /// <summary>
    /// This type of exception is for a result not found
    /// but not necessarily due to bad input;
    /// could be due to other factors such as system state
    /// </summary>
    public class NotFoundException : Exception
    {
        public string SearchKey { get; private set; }

        public NotFoundException(string exceptionMessage, string searchKey) : base(exceptionMessage)
        {
            this.SearchKey = searchKey;
        }
    }
}
