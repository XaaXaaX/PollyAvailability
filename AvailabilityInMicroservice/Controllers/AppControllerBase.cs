using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvailabilityInMicroservice.Controllers
{
    public class AppControllerBase : ControllerBase
    {
        protected int MaxRetryAttempts = 2;
        protected TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(2);

        HttpClient httpClient = new HttpClient();

        protected AppControllerBase()
        {}

        protected async Task<HttpResponseMessage> CallFailesService()
        {
            var response = await httpClient.GetAsync("http://localhost:44131/api/failedservice/1");
            return response;
        }
    }
}