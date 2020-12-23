using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class BaseFormExtensions
    {
        public static Files GetFiles(this BaseForm baseForm)
        {
            return 
                new Files(baseForm.Files)
                    .Add(baseForm.NotesFile);
        }
    }
}
