using CreditOne.P360FormService.Models.Requests;
using CreditOne.P360FormSubmissionService.Infra;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class UpdateRequestExtensions
    {
        public static Files GetFiles(this UpdateRequest updateRequest)
        {
            return
                new Files(updateRequest.FileUpdates)
                    .Add(updateRequest.CommentsFile);
        }
    }
}
