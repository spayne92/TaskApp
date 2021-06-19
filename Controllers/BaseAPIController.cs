using Microsoft.AspNetCore.Mvc;

namespace BaseCoreAPI.Controllers
{
    [Route("/api/[Controller]")]
    public class BaseAPIController : ControllerBase
    {
        public BaseAPIController()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test Response Message");
        }
    }
}
