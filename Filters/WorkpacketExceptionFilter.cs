using CreditOne.Core.Logger.Infra;
using CreditOne.P360FormService.Models.Responses;
using CreditOne.P360FormSubmissionService.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CreditOne.P360FormSubmissionService.Filters
{
    public class WorkpacketExceptionFilter : ExceptionFilterAttribute
    {
        

        public override void OnException(ExceptionContext context)
        {
            if (context != null)
            {
                if (context.Exception is ArgumentException || context.Exception is ArgumentNullException)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else if(context.Exception is NotFoundException)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                context.Result = new JsonResult(new Workpacket() { Error = context.Exception.Message });
                context.ExceptionHandled = true;

                Log.Error(context.Exception, $"Error at action: '{context.ActionDescriptor.DisplayName}'");             
            }
        }

    }
}
