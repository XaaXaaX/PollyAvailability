using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace AvailabilityInMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircuiteBreakerController : AppControllerBase
    {
        private CircuitBreakerPolicy<HttpResponseMessage> breaker;
        private RetryPolicy<HttpResponseMessage> retryPolicy;
        private PolicyWrap<HttpResponseMessage> myResilienceStrategy;
        public async Task<ActionResult<HttpResponseMessage>> Get()
        {
            Func<Task<HttpResponseMessage>> myFun = async () => await CallFailesService();

            Action<DelegateResult<HttpResponseMessage>, CircuitState, TimeSpan, Context> onBreak = (exception, state, timespan, context)
                =>
            {
              
            };
            Action<Context> onReset = context
                =>
            {

            };
            Action onHalfOpen = ()
             =>
            { };

            retryPolicy = Policy
             .HandleResult<HttpResponseMessage>
             (r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
             .WaitAndRetryAsync(4, i => PauseBetweenFailures);

            breaker = Policy.HandleResult<HttpResponseMessage>
                (r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                .CircuitBreakerAsync(2, PauseBetweenFailures, onBreak, onReset, onHalfOpen);

            myResilienceStrategy = Policy.WrapAsync(retryPolicy, breaker);



           var ret = await myResilienceStrategy.ExecuteAsync(async () => await myFun.Invoke());

            return ret;
        }


    }
}
