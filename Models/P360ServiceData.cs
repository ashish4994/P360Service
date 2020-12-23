using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Models
{
    public class P360ServiceData
    {
        public string ProcessName { get; set; }
        public string NodeName { get; set; }
        public int TimeBetweenP360LoginsInMs { get; set; }
        public bool SearchAllArchives { get; set; }
        public string GlobalArchiveName { get; set; }
    }
}
