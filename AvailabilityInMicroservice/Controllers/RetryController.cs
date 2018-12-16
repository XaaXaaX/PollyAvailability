using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Retry;

namespace AvailabilityInMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetryController : AppControllerBase
    {
        private readonly RetryPolicy<HttpResponseMessage> retryPolicy;

        public RetryController()
        {
            retryPolicy = Policy
             .HandleResult<HttpResponseMessage>
             (r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
             .WaitAndRetryAsync(MaxRetryAttempts, i => PauseBetweenFailures);
        }

        [HttpGet]
        public async Task<ActionResult<HttpResponseMessage>> Get()
        {

            Func<Task<HttpResponseMessage>> myFun = async () => await CallFailesService();

            var ret = await retryPolicy.ExecuteAsync(async () => await myFun.Invoke());

            return ret;
        }
    }
}
