using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AvailabilityInMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FailedServiceController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            if (id == 1)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
            return "value";
        }
    }
}
