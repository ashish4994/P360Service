using CreditOne.P360FormService.Models.Requests;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections;
using System.Linq;

namespace CreditOne.P360FormSubmissionService.Filters
{
    public class WorkpacketRequestValidationFilter : ActionFilterAttribute
    {
        public string ModelName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var request = context.ActionArguments[this.ModelName] as BaseForm;
           
            if (request == null)
            {
                throw new ArgumentNullException($"Error: Null encountered validating input argument: {ModelName}");
            }
            // FormName is required to know which queue/worklist to use
            else if (string.IsNullOrWhiteSpace(request.FormName) ||
                !Enum.GetNames(typeof(FormNames)).Cast<string>().ToList().Any(s => s == request.FormName))
            {
                throw new ArgumentException($"Error: Validation failed for field: {request.FormName ?? ""}, of input argument: '{ModelName}'");
            }
        }


    }
}
