using System.Linq;
using BaseCoreAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseCoreAPI.Controllers
{
    [Route("/api/[Controller]")]
    public class BaseAPIController : ControllerBase
    {
        private readonly BaseContext _context;

        public BaseAPIController(BaseContext baseContext)
        {
            _context = baseContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test Response Message");
        }

        [Authorize]
        [HttpGet]
        [Route("get-rooms")]
        public IActionResult GetRooms()
        {
            return Ok(_context.Rooms.ToList());
        }
    }
}
