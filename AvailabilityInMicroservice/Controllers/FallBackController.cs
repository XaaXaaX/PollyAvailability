using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Polly.Wrap;

namespace AvailabilityInMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FallBackController : AppControllerBase
    {
        private FallbackPolicy<HttpResponseMessage> fallbackPolicy;
        private RetryPolicy<HttpResponseMessage> retryPolicy;
        private PolicyWrap<HttpResponseMessage> myResilienceStrategy;
        // GET: api/FallBack
        [HttpGet]
        public async Task<ActionResult<HttpResponseMessage>> Get()
        {
            Func<Task<HttpResponseMessage>> myFun = async () => await CallFailesService();

            retryPolicy = Policy
            .HandleResult<HttpResponseMessage>
            (r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(MaxRetryAttempts, i => PauseBetweenFailures);

            fallbackPolicy = Policy.HandleResult<HttpResponseMessage>
                (f => f.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                 .FallbackAsync(new HttpResponseMessage(HttpStatusCode.OK)
                 {
                     Content = new ObjectContent(typeof(string), "the system under maintentance", new JsonMediaTypeFormatter())
                 });

            myResilienceStrategy = Policy.WrapAsync(fallbackPolicy , retryPolicy );
            var ret = await myResilienceStrategy.ExecuteAsync(async () => await myFun.Invoke());

            return ret;
        }
    }
}
