using System;
using Microsoft.AspNetCore.Mvc;

namespace CreditOne.P360FormSubmissionService.Controllers
{
    [ApiController]
    [Route("api")]
    public class WorkpacketController : Controller
    {
        public WorkpacketController() { }

        // GET: api/ping
        [HttpGet("ping")]
        public string Ping()
        {
            return $"Up and Running @ {Environment.MachineName}";
        }
       
    }
}
